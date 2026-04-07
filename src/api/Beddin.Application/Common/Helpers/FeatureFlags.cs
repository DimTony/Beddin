// <copyright file="FeatureFlags.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Helpers
{
    /// <summary>
    /// Each constant maps to a key in configuration.
    /// Naming convention: Domain.Action.
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// Feature flag for enabling or disabling the authentication functionality.
        /// </summary>
        public const string Authentication = "Features:Authentication";

        /// <summary>
        /// Feature flag for enabling or disabling the audit log functionality.
        /// </summary>
        public const string AuditLog = "Features:AuditLog";

        /// <summary>
        /// Feature flag for enabling or disabling the admin panel functionality.
        /// </summary>
        public const string AdminPanel = "Features:AdminPanel";
    }
}
