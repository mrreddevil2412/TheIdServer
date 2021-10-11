﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Contains shared constants.
    /// </summary>
    public static class SharedConstants
    {
        /// <summary>
        /// The writer
        /// </summary>
        public const string WRITERPOLICY = "Is4-Writer";
        /// <summary>
        /// The reader
        /// </summary>
        public const string READERPOLICY = "Is4-Reader";

        /// <summary>
        /// The registration
        /// </summary>
        public const string REGISTRATIONPOLICY = "Is4-Registration";

        /// <summary>
        /// The registration
        /// </summary>
        public const string TOKENPOLICY = "Is4-Token";

        /// <summary>
        /// The admon scope
        /// </summary>
        public const string ADMINSCOPE = "theidserveradminapi";

        /// <summary>
        /// The token scope
        /// </summary>
        public const string TOKENSCOPES = "theidservertokenapi";
    }
}
