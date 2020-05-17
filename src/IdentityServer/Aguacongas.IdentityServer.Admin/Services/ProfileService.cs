﻿using Aguacongas.IdentityServer.Abstractions;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Defines profile service properties key constantes
    /// </summary>
    public static class ProfileServiceProperties
    {
        /// <summary>
        /// The claim builder assembly path key
        /// </summary>
        public static readonly string ClaimProviderAssemblyPathKey = "ClaimProviderAssemblyPath";
        /// <summary>
        /// The claim builder type key
        /// </summary>
        public static readonly string ClaimProviderTypeKey = "ClaimProviderType";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DefaultProfileService" />
    public class ProfileService<TUser> : IdentityServer4.AspNetIdentity.ProfileService<TUser> where TUser : class
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="logger">The logger.</param>
        public ProfileService(UserManager<TUser> userManager, 
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            ILogger<ProfileService<TUser>> logger) : base(userManager, claimsFactory, logger)
        { }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await FindUserAsync(context.Subject.GetSubjectId()).ConfigureAwait(false);
            var principal = user != null ? await GetUserClaimsAsync(user).ConfigureAwait(false) : context.Subject;

            foreach (var resource in context.RequestedResources.IdentityResources)
            {
                var claims = await GetClaimsFromResource(resource, principal, context.Client, context.Caller).ConfigureAwait(false);
                context.AddRequestedClaims(claims);
            }
            foreach (var resource in context.RequestedResources.ApiResources)
            {
                var claims = await GetClaimsFromResource(resource, principal, context.Client, context.Caller).ConfigureAwait(false);
                context.AddRequestedClaims(claims);
            }
            await base.GetProfileDataAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the claims for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual async Task<ClaimsPrincipal> GetUserClaimsAsync(TUser user)
        {
            var principal = await ClaimsFactory.CreateAsync(user);
            if (principal == null)
            {
                throw new InvalidOperationException("ClaimsFactory failed to create a principal");
            }

            return principal;
        }

        /// <summary>
        /// Loads the user by the subject id.
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        protected virtual async Task<TUser> FindUserAsync(string subjectId)
        {
            var user = await UserManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                Logger?.LogWarning("No user found matching subject Id: {subjectId}", subjectId);
            }

            return user;
        }

        private Task<IEnumerable<Claim>> GetClaimsFromResource(Resource resource, ClaimsPrincipal subject, Client client, string caller)
        {
            if (!resource.Properties.TryGetValue(ProfileServiceProperties.ClaimProviderTypeKey, out string builderTypeName))
            {
                return Task.FromResult(Array.Empty<Claim>() as IEnumerable<Claim>);
            }

            var type = Type.GetType(builderTypeName, assembyName =>
            {
                if (resource.Properties.TryGetValue(ProfileServiceProperties.ClaimProviderAssemblyPathKey, out string path))
                {
#pragma warning disable S3885 // "Assembly.Load" should be used
                    return Assembly.LoadFrom(path);
#pragma warning restore S3885 // "Assembly.Load" should be used
                }
                
                return Assembly.Load(assembyName);
            }, (assembly, name, throwOnError) =>
            {
                return assembly?.GetType(name, throwOnError);
            }, true);

            var provider = Activator.CreateInstance(type) as IProvideClaims;

            return provider.ProvideClaims(subject, client, caller, resource);
        }
    }
}