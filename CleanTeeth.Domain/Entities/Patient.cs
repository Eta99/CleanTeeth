using CleanTeeth.Domain.Abstractions;
using CleanTeeth.Domain.Exceptions;
using CleanTeeth.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTeeth.Domain.Entities
{
    public class Patient : IAuditable
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Email Email { get; private set; } = null!;

        public string? CreatedBy { get; private set; }
        public string? ModifiedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ModifiedAt { get; private set; }

        string? IAuditable.CreatedBy { get => CreatedBy; set => CreatedBy = value; }
        string? IAuditable.ModifiedBy { get => ModifiedBy; set => ModifiedBy = value; }
        DateTime IAuditable.CreatedAt { get => CreatedAt; set => CreatedAt = value; }
        DateTime IAuditable.ModifiedAt { get => ModifiedAt; set => ModifiedAt = value; }

        private Patient() { }

        public Patient(string name, Email email)
        {
            EnforceNameBusinessRules(name);
            EnforceEmailBusinessRules(email);

            Name = name;
            Email = email;
            Id = Guid.CreateVersion7();
        }

        public void UpdateName(string name)
        {
            EnforceNameBusinessRules(name);
            Name = name;
        }

        private void EnforceNameBusinessRules(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessRuleException($"The {nameof(name)} is required");
            }
        }

        public void UpdateEmail(Email email)
        {
            EnforceEmailBusinessRules(email);
            Email = email;
        }

        private void EnforceEmailBusinessRules(Email email)
        {
            if (email is null)
            {
                throw new BusinessRuleException($"The {nameof(email)} is required");
            }
        }
    }
}
