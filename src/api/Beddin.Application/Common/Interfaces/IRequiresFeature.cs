// <copyright file="IRequiresFeature.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Interfaces
{
    /// <summary>
    /// Commands or queries that implement this interface
    /// will be blocked by the pipeline if their feature flag is disabled.
    /// Handlers themselves stay completely clean.
    /// </summary>
    public interface IRequiresFeature
    {
        /// <summary>
        /// Gets the name of the feature flag required to execute the command or query.
        /// </summary>
        string FeatureFlag { get; }
    }
}
