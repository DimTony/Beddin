using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string? message = null)
            : base(message ?? "You do not have permission to perform this action.") { }
    }
}
