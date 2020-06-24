﻿using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class ResourceStoreTest
    {
        [Fact]
        public async Task GetAllResourcesAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock, 
                out ResourceStore sut);

            await sut.GetAllResourcesAsync().ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => 
                p.Expand == "ApiClaims,Secrets,Scopes,Properties,Resources"), default));
            identityStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => 
                p.Expand == "IdentityClaims,Properties,Resources"), default));
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock, 
                out ResourceStore sut);

            await sut.FindIdentityResourcesByScopeNameAsync(new string[] { "test" }).ConfigureAwait(false);

            identityStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == "Id eq 'test'"), default));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock, 
                out ResourceStore sut);

            await sut.FindApiResourcesByScopeNameAsync(new string[] { "test" }).ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == "Scopes/any(s:s/Scope eq 'test')"), default));
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out ResourceStore sut);

            await sut.FindApiResourcesByNameAsync(new string[] { "test" }).ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Expand == "ApiClaims,Secrets,Scopes,Properties,Resources"), default));
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out ResourceStore sut);

            await sut.FindApiScopesByNameAsync(new string[] { "test" }).ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Expand == "ApiScopeClaims,Resources"), default));
        }

        private static void CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock, 
            out Mock<IAdminStore<IdentityResource>> identityStoreMock, 
            out Mock<IAdminStore<ApiScope>> apiScopeStoreMock, 
            out ResourceStore sut)
        {
            apiStoreMock = new Mock<IAdminStore<ProtectResource>>();
            identityStoreMock = new Mock<IAdminStore<IdentityResource>>();
            apiScopeStoreMock = new Mock<IAdminStore<ApiScope>>();
            sut = new ResourceStore(apiStoreMock.Object, identityStoreMock.Object, apiScopeStoreMock.Object);

            apiStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ProtectResource>
                {
                    Items = new List<ProtectResource>(0)
                }).Verifiable();

            identityStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<IdentityResource>
                {
                    Items = new List<IdentityResource>(0)
                }).Verifiable();

            apiScopeStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ApiScope>
                {
                    Items = new List<ApiScope>(0)
                }).Verifiable();
        }
    }
}
