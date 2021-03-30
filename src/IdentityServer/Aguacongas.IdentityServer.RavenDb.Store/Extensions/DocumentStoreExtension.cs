﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Reflection;

namespace Raven.Client.Documents
{
    public static class DocumentStoreExtension
    {
        public static IDocumentStore SetFindIdentityPropertyForIdentityServerStores(this IDocumentStore store)
        {
            store.SetFindIdentityPropertyForIdentityModel();
            var findId = store.Conventions.FindIdentityProperty;
            store.Conventions.FindIdentityProperty = memberInfo => SetConventions(memberInfo, findId);
            return store;
        }

        private static bool SetConventions(MemberInfo memberInfo, Func<MemberInfo, bool> findId)
        {
            if (memberInfo.DeclaringType.Assembly == typeof(ProtectResource).Assembly)
            {
                return false;
            }
            if (memberInfo.DeclaringType == typeof(SchemeDefinition))
            {
                return false;
            }
            return findId(memberInfo);
        }
    }
}