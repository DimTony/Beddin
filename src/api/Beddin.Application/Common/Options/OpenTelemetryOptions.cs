// <copyright file="OpenTelemetryOptions.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Beddin.Application.Common.Options
{
    /// <summary>
    /// Provides the activity source for Beddin telemetry.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public static class BeddinActivitySource
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Gets the name of the Beddin activity source.
        /// </summary>
        public const string Name = "Beddin";

        /// <summary>
        /// Gets the singleton instance of the <see cref="ActivitySource"/> for Beddin.
        /// </summary>
        public static readonly ActivitySource Instance = new(Name, "1.0.0");
    }
}
