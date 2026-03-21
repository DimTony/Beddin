using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when an operation conflicts with current system state.
    /// e.g. duplicate membership number, duplicate journal entry number.
    /// Maps to HTTP 409.
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
