using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : base("One or more validation failures occurred.")
        {
            Errors = failures
                .GroupBy(f => f.PropertyName)
                .Select(g => new ValidationError(
                    g.Key,
                    g.Select(f => f.ErrorMessage).ToArray()))
                .ToList();
        }
    }

    public record ValidationError(string Field, string[] Messages);
}
