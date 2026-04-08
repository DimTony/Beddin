// <copyright file="UserRole.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Users
{
    /// <summary>
    /// Represents a user role in the system.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public sealed class Role : AggregateRoot<RoleId>
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// Public constructor for EF Core.
        /// </summary>
        public Role()
        {
        }

        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        public string Name { get; private set; } = default!;

        /// <summary>
        /// Gets the description of the role.
        /// </summary>
        public string Description { get; private set; } = default!;

        /// <summary>
        /// Gets the date and time when the role was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the role was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Role"/> instance.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="description">The role description.</param>
        /// <returns>A new <see cref="Role"/> instance.</returns>
        public static Role Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Name and description are required.", nameof(name));
            }

            var role = new Role
            {
                Id = RoleId.New(),
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            role.RaiseDomainEvent(new RoleCreatedEvent(
                    role.Id,
                    name,
                    description));

            return role;
        }
    }
}
