using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string resource, object key)
            : base($"{resource} with key '{key}' was not found.") { }
    }
}
