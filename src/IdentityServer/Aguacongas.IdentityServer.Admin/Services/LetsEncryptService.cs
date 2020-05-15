﻿using Aguacongas.IdentityServer.Admin.Models;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Manage SSL certificates.
    /// </summary>
    public class LetsEncryptService
    {
        private readonly IAcmeContext _context;
        private readonly IOptions<CertesAccount> _options;
        private IChallengeContext _challengeContext;
        private IOrderContext _orderContext;

        /// <summary>
        /// Gets the key authz.
        /// </summary>
        /// <value>
        /// The key authz.
        /// </value>
        public string KeyAuthz { get; private set; }

        /// <summary>
        /// Gets the on certificate ready action.
        /// </summary>
        /// <value>
        /// The on certificate ready.
        /// </value>
        public Action OnCertificateReady { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LetsEncryptService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">
        /// context
        /// or
        /// options
        /// </exception>
        public LetsEncryptService(IAcmeContext context, IOptions<CertesAccount> options)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Creates the certificate.
        /// </summary>
        /// <param name="host">The host.</param>
        public void CreateCertificate(IWebHost host)
        {
            if (!_options.Value.Enable)
            {
                return;
            }

            host.Start();
            var resetEvent = new ManualResetEvent(false);
            OnCertificateReady = async () =>
            {
                await host.StopAsync().ConfigureAwait(false);
                await CreateCredentialFileAsync().ConfigureAwait(false);
                resetEvent.Set();
            };
            CreateNewAuthaurizationKeyAsync().GetAwaiter().GetResult();

            resetEvent.WaitOne(60000);
        }

        private async Task CreateNewAuthaurizationKeyAsync()
        {
            var settings = _options.Value;
            _orderContext = await _context.NewOrder(new[] { settings.Domain })
                .ConfigureAwait(false);
            var authorizations = await _orderContext.Authorizations().ConfigureAwait(false);
            foreach(var authorizationContext in authorizations)
            {
                var authorization = await authorizationContext.Resource().ConfigureAwait(false);
                if (authorization.Status == AuthorizationStatus.Pending)
                {
                    _challengeContext = await authorizationContext.Http().ConfigureAwait(false);
                    KeyAuthz = _challengeContext.KeyAuthz;
                    await _challengeContext.Validate().ConfigureAwait(false);
                    break;
                }
            }            
        }

        private async Task CreateCredentialFileAsync()
        {            
            var challenge = await _challengeContext.Resource().ConfigureAwait(false);

            if (challenge.Status == ChallengeStatus.Invalid)
            {
                return;
            }

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
            var certificationRequestBuilder = await _orderContext.CreateCsr(privateKey).ConfigureAwait(false);
            var csr = certificationRequestBuilder.Generate();

            await _orderContext.Finalize(csr).ConfigureAwait(false);

            var certificateChain = await _orderContext.Download().ConfigureAwait(false);

            var pfxBuilder = certificateChain.ToPfx(privateKey);

            var pfxName = string.Format(CultureInfo.InvariantCulture, "[certes] {0:yyyyMMddhhmmss}", DateTime.UtcNow);

            var settings = _options.Value;
            var pfx = pfxBuilder.Build(pfxName, settings.PfxPassword);

            await File.WriteAllBytesAsync(settings.PfxPath, pfx).ConfigureAwait(false);
        }
    }
}
