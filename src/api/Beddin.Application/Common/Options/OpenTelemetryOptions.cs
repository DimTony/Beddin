using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Options
{
    public static class BeddinActivitySource
    {
        public const string Name = "Beddin";
        public static readonly ActivitySource Instance = new(Name, "1.0.0");
    }
}
