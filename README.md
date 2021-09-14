# TheIdServer

[OpenID/Connect](https://openid.net/connect/), [OAuth2](https://oauth.net/2/) and [WS-Federation](https://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html) server based on [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=aguacongas_TheIdServer)](https://sonarcloud.io/dashboard?id=aguacongas_TheIdServer)

[![Build status](https://ci.appveyor.com/api/projects/status/hutfs4sy38fy9ca7?svg=true)](https://ci.appveyor.com/project/aguacongas/theidserver) [[![Docker](https://github.com/Aguafrommars/TheIdServer/actions/workflows/docker.yml/badge.svg)](https://github.com/Aguafrommars/TheIdServer/actions/workflows/docker.yml)]
  [![Artifact HUB](https://img.shields.io/endpoint?url=https://artifacthub.io/badge/repository/aguafrommars)](https://artifacthub.io/packages/search?repo=aguafrommars)


### [Try it now at https://theidserver.herokuapp.com/](https://theidserver.herokuapp.com/)

**login**: alice  
**pwd**: Pass123$

[An in-memory](https://theidserver.herokuapp.com/) database version is available on [Heroku](https://www.heroku.com/).

### Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

Or if you're feeling really generous, we support sponsorships.

Choose your favorite:

* [issuehunts](https://issuehunt.io/r/Aguafrommars/TheIdServer/issues/170)
* [github sponsor](https://github.com/sponsors/aguacongas),
* [liberapay](https://liberapay.com/aguacongas)

## Main features

### Admin app
![home](https://raw.githubusercontent.com/Aguafrommars/TheIdServer/master/doc/assets/home.png)

* [Users management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/USER.md)
* [Roles management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/ROLE.md)
* [Clients management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CLIENT.md)
* [Apis management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/API.md)
* [Api Scopes management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/SCOPE.md)
* [Identities management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/IDENTITY.md)
* [Relying parties management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/RELYING-PARTY.md)
* [External providers management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/PROVIDER.md)
* [Localizable](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/LOCALIZATION.md)
* [Export/import configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/EXPORT_IMPORT.md)
* [Keys management](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/KEYS.md)

### Server

* [OpenID/Connect](https://openid.net/connect/), [OAuth2](https://oauth.net/2/) and [WS-Federation](https://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html) server
* [Large choice of database](https://github.com/Aguafrommars/TheIdServer/blob/master/src/Aguacongas.TheIdServer/README.md#using-entity-framework-core)
* [Dynamic external provider configuration](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#configure-the-provider-hub)
* [Public / Private installation](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#using-the-api)
* [Docker support](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#from-docker)
* [Claims providers](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/CLAIMS_PROVIDER.md)
* [External claims mapping](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/EXTERNAL_CLAIMS_MAPPING.md)
* [Localizable](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/LOCALIZATION.md)
* [OpenID Connect Dynamic Client Registration](https://openid.net/specs/openid-connect-registration-1_0.html)
* [Auto remove expired tokens](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md#configure-token-cleaner)
* [Keys rotation](https://github.com/Aguafrommars/TheIdServer/tree/master/doc/KEYS_ROTATION.md)

## Setup

Read the [server README](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer/README.md) for server configuration.  
Read the [application README](https://github.com/Aguafrommars/TheIdServer/tree/master/src/Aguacongas.TheIdServer.BlazorApp/README.md) for application configuration.  

## Build from source

You can build the solution with Visual Studio or use the `dotnet build` command.  
To build docker images launch at solution root: 

```bash
docker build -t aguacongas/theidserver:dev -f "./src/Aguacongas.TheIdServer/Dockerfile" .
docker build -t aguacongas/theidserverapp:dev -f "./src/Aguacongas.TheIdServer.BlazorApp/Dockerfile" .
```

## Contribute

We warmly welcome contributions. You can contribute by opening an issue, suggest new a feature, or submit a pull request.

Read [How to contribute](https://github.com/Aguafrommars/TheIdServer/tree/master/CONTRIBUTING.md) and [Contributor Covenant Code of Conduct](https://github.com/Aguafrommars/TheIdServer/tree/master/CODE_OF_CONDUCT.md) for more information.

## OIDC Certification test result

The server pass the [oidcc-basic-certification-test-plan](
https://www.certification.openid.net/plan-detail.html?plan=ZKco5LJhicIlT&public=true) with some warnings. It is anticipated that it will pass the certification process, but we need your assistance. Please sponsor this project to help us pay the required [certification fee](https://openid.net/certification/fees/).

Choose your favorite:

* [issuehunts](https://issuehunt.io/r/Aguafrommars/TheIdServer/issues/170)
* [github sponsor](https://github.com/sponsors/aguacongas),
* [liberapay](https://liberapay.com/aguacongas)
