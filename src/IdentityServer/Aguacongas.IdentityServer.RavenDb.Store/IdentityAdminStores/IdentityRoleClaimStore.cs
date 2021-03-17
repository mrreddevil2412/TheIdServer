﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.Identity.RavenDb;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class IdentityRoleClaimStore<TUser, TRole> : IAdminStore<RoleClaim>
        where TRole : IdentityRole, new()
        where TUser : IdentityUser
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<IdentityRoleClaimStore<TUser, TRole>> _logger;
        public IdentityRoleClaimStore(RoleManager<TRole> roleManager,
            IAsyncDocumentSession session,
            ILogger<IdentityRoleClaimStore<TUser, TRole>> logger)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RoleClaim> CreateAsync(RoleClaim entity, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(entity.RoleId)
                .ConfigureAwait(false);
            var claimList = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
            var claim = entity.ToRoleClaim().ToClaim();
            var result = await _roleManager.AddClaimAsync(role, claim)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                entity.Id = $"{role.Id}@{claimList.Count}";
                _logger.LogInformation("Entity {EntityId} created", entity.Id, entity);
                return entity;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as RoleClaim, cancellationToken)
                .ConfigureAwait(false);
        }


        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(id, null, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(RoleClaim).Name} at id {id} is not found");
            }

            var role = await GetRoleAsync(claim.RoleId)
                .ConfigureAwait(false);
            var result = await _roleManager.RemoveClaimAsync(role, claim.ToClaim())
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
            _logger.LogInformation("Entity {EntityId} deleted", claim.Id, claim);
        }

        public async Task<RoleClaim> UpdateAsync(RoleClaim entity, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(entity.Id, null, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(RoleClaim).Name} at id {entity.Id} is not found");
            }

            var role = await GetRoleAsync(claim.RoleId)
                .ConfigureAwait(false);
            var result = await _roleManager.RemoveClaimAsync(role, claim.ToClaim())
                .ConfigureAwait(false);
            ChechResult(result, entity);
            result = await _roleManager.AddClaimAsync(role, entity.ToRoleClaim().ToClaim())
                .ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return ChechResult(result, entity);
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as RoleClaim, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<RoleClaim> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(id, request?.Expand, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                return null;
            }
            var entity = claim.ToEntity();
            if (request?.Expand == nameof(RoleClaim.Role))
            {
                var role = await _session.LoadAsync<TRole>($"role/{entity.RoleId}", cancellationToken).ConfigureAwait(false);
                entity.Role = role.ToEntity();                
            }
            
            return entity;
        }

        public async Task<PageResponse<RoleClaim>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _session.Query<IdentityRoleClaim<string>>();
            var odataQuery = query.GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);
            
            var page = odataQuery.GetPage(request);

            var claimList = await page.ToListAsync(cancellationToken).ConfigureAwait(false);
            
            return new PageResponse<RoleClaim>
            {
                Count = count,
                Items = claimList.Select(c => c.ToEntity())
            };
        }

        private async Task<TRole> GetRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId)
                .ConfigureAwait(false);
            if (role == null)
            {
                throw new IdentityException($"Role {roleId} not found");
            }

            return role;
        }

        private Task<IdentityRoleClaim<string>> GetClaimAsync(string id, string expand, CancellationToken cancellationToken)
        {
            return _session.LoadAsync<IdentityRoleClaim<string>>($"roleclaim/{id}", builder =>
            {
                if (expand == nameof(RoleClaim.Role))
                {
                    builder.IncludeDocuments(c => $"role/{c.RoleId}");
                }
            }, cancellationToken);
        }

        private static TValue ChechResult<TValue>(IdentityResult result, TValue value)
        {
            if (result.Succeeded)
            {
                return value;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

    }
}
