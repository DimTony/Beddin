using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Helpers
{
    /// <summary>
    /// Each constant maps to a key in configuration.
    /// Naming convention: Domain.Action
    /// </summary>
    public static class FeatureFlags
    {
        // ── Core Feature Modules ──────────────────────────────────
        public const string Authentication = "Features:Authentication";
        public const string AdminPanel = "Features:AdminPanel";
        

        // ── Cross-Cutting Features ────────────────────────────────
        public const string AuditLog = "Features:AuditLog";

    }
}
