using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Interfaces
{
    /// <summary>
    /// Commands or queries that implement this interface
    /// will be blocked by the pipeline if their feature flag is disabled.
    /// Handlers themselves stay completely clean.
    /// </summary>
    public interface IRequiresFeature
    {
        string FeatureFlag { get; }
    }
}
