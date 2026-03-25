using Beddin.Domain.Common;
using Beddin.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Aggregates.Users
{
    public sealed class Role : AggregateRoot<RoleId>
    {
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Role() { }

        public static Role Create(
            string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Name and description are required.", nameof(name));

            var role = new Role
            {
                Id = RoleId.New(),
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            role.RaiseDomainEvent(new RoleCreatedEvent(
                    role.Id,
                    name,
                    description));

            return role;
        }
    }
}
