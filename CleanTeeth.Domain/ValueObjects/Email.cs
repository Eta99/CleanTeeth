using CleanTeeth.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTeeth.Domain.ValueObjects
{
    public record Email
    {
        public string Value { get; } = null!;

        private Email() { }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleException($"The {nameof(value)} is required");
            }

            if (!value.Contains("@"))
            {
                throw new BusinessRuleException($"The {nameof(value)} is not valid");
            }

            Value = value;
        }
    }
}
