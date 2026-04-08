// <copyright file="PasswordResetTokenConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// The <c>PasswordResetTokenConfiguration</c> class is responsible for configuring the entity mapping for the <see cref="PasswordResetToken"/> entity in the Entity Framework Core context. It implements the <see cref="IEntityTypeConfiguration{TEntity}"/> interface, allowing it to define how the properties of the <see cref="PasswordResetToken"/> entity should be mapped to the database schema. This includes specifying primary keys, property conversions, relationships, indexes, and other constraints. By using this configuration class, you can ensure that the database schema is properly aligned with the domain model for password reset tokens, facilitating efficient data storage and retrieval while maintaining data integrity and consistency.
    /// </summary>
    public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new PasswordResetTokenId(value))
                .ValueGeneratedNever();

            builder.Property(p => p.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId);

            builder.Property(p => p.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(p => p.Token)
                .IsUnique()
                .HasDatabaseName("IX_PasswordResetTokens_Token");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(p => p.ExpiresAt)
                .IsRequired();

            builder.Property(p => p.UsedAt)
                .IsRequired(false);

            builder.Property(p => p.RevokedAt)
                .IsRequired(false);

            builder.Property(p => p.IpAddress)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(p => p.UserAgent)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(p => p.UserId)
                .HasFilter("\"UsedAt\" IS NULL")
                .HasDatabaseName("IX_PasswordResetTokens_UserId");
        }
    }
}
