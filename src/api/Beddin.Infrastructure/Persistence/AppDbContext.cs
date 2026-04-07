// <copyright file="AppDbContext.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.AuditLog;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beddin.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the application's database context, providing access to all entities and handling persistence logic.
    /// </summary>
    public class AppDbContext : DbContext, IReadDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets the <see cref="DbSet{Role}"/> representing the collection of roles in the database.
        /// </summary>
        public DbSet<Role> Roles => this.Set<Role>();

        /// <summary>
        /// Gets the <see cref="DbSet{User}"/> representing the collection of users in the database.
        /// </summary>
        public DbSet<User> Users => this.Set<User>();

        /// <summary>
        /// Gets the <see cref="DbSet{PasswordResetToken}"/> representing the collection of password reset tokens in the database.
        /// </summary>
        public DbSet<PasswordResetToken> PasswordResetTokens => this.Set<PasswordResetToken>();

        /// <summary>
        /// Gets the <see cref="DbSet{AuditLog}"/> representing the collection of audit logs in the database.
        /// </summary>
        public DbSet<AuditLog> AuditLogs => this.Set<AuditLog>();

        /// <summary>
        /// Gets the <see cref="DbSet{SavedSearch}"/> representing the collection of saved searches in the database.
        /// </summary>
        public DbSet<SavedSearch> SavedSearches => this.Set<SavedSearch>();

        /// <summary>
        /// Gets the <see cref="DbSet{UserSession}"/> representing the collection of user sessions in the database.
        /// </summary>
        public DbSet<UserSession> UserSessions => this.Set<UserSession>();

        /// <summary>
        /// Gets the <see cref="DbSet{Property}"/> representing the collection of properties in the database.
        /// </summary>
        public DbSet<Property> Properties => this.Set<Property>();

        /// <summary>
        /// Gets the <see cref="DbSet{Favorite}"/> representing the collection of favorites in the database.
        /// </summary>
        public DbSet<Favorite> Favorites => this.Set<Favorite>();

        /// <summary>
        /// Gets the <see cref="DbSet{Amenity}"/> representing the collection of amenities in the database.
        /// </summary>
        public DbSet<Amenity> Amenities => this.Set<Amenity>();

        /// <summary>
        /// Gets the <see cref="DbSet{PropertyAmenity}"/> representing the collection of property amenities in the database.
        /// </summary>
        public DbSet<PropertyAmenity> PropertyAmenities => this.Set<PropertyAmenity>();

        /// <summary>
        /// Gets the <see cref="DbSet{PropertyImage}"/> representing the collection of property images in the database.
        /// </summary>
        public DbSet<PropertyImage> PropertyImages => this.Set<PropertyImage>();

        /// <summary>
        /// Gets the <see cref="DbSet{Inquiry}"/> representing the collection of inquiries in the database.
        /// </summary>
        public DbSet<Inquiry> Inquiries => this.Set<Inquiry>();

        /// <summary>
        /// Gets the <see cref="IQueryable{Role}"/> representing the collection of roles in the database for read operations.
        /// </summary>
        IQueryable<Role> IReadDbContext.Roles => this.Roles;

        /// <summary>
        /// Gets the <see cref="IQueryable{User}"/> representing the collection of users in the database for read operations.
        /// </summary>
        IQueryable<User> IReadDbContext.Users => this.Users;

        /// <summary>
        /// Gets the <see cref="IQueryable{PasswordResetToken}"/> representing the collection of password reset tokens in the database for read operations.
        /// </summary>
        IQueryable<PasswordResetToken> IReadDbContext.PasswordResetTokens => this.PasswordResetTokens;

        /// <summary>
        /// Gets the <see cref="IQueryable{AuditLog}"/> representing the collection of audit logs in the database for read operations.
        /// </summary>
        IQueryable<AuditLog> IReadDbContext.AuditLogs => this.AuditLogs;

        /// <summary>
        /// Gets the <see cref="IQueryable{SavedSearch}"/> representing the collection of saved searches in the database for read operations.
        /// </summary>
        IQueryable<SavedSearch> IReadDbContext.SavedSearches => this.SavedSearches;

        /// <summary>
        /// Gets the <see cref="IQueryable{UserSession}"/> representing the collection of user sessions in the database for read operations.
        /// </summary>
        IQueryable<UserSession> IReadDbContext.UserSessions => this.UserSessions;

        /// <summary>
        /// Gets the <see cref="IQueryable{Property}"/> representing the collection of properties in the database for read operations.
        /// </summary>
        IQueryable<Property> IReadDbContext.Properties => this.Properties;

        /// <summary>
        /// Gets the <see cref="IQueryable{Favorite}"/> representing the collection of favorites in the database for read operations.
        /// </summary>
        IQueryable<Favorite> IReadDbContext.Favorites => this.Favorites;

        /// <summary>
        /// Gets the <see cref="IQueryable{Amenity}"/> representing the collection of amenities in the database for read operations.
        /// </summary>
        IQueryable<Amenity> IReadDbContext.Amenities => this.Amenities;

        /// <summary>
        /// Gets the <see cref="IQueryable{PropertyAmenity}"/> representing the collection of property amenities in the database for read operations.
        /// </summary>
        IQueryable<PropertyAmenity> IReadDbContext.PropertyAmenities => this.PropertyAmenities;

        /// <summary>
        /// Gets the <see cref="IQueryable{PropertyImage}"/> representing the collection of property images in the database for read operations.
        /// </summary>
        IQueryable<PropertyImage> IReadDbContext.PropertyImages => this.PropertyImages;

        /// <summary>
        /// Gets the <see cref="IQueryable{Inquiry}"/> representing the collection of inquiries in the database for read operations.
        /// </summary>
        IQueryable<Inquiry> IReadDbContext.Inquiries => this.Inquiries;

        /// <summary>
        /// Saves all changes made in this context to the database asynchronously, applying soft delete logic.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Configures the schema needed for the context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.Ignore<AuditLogId>();
            modelBuilder.Ignore<RoleId>();
            modelBuilder.Ignore<UserId>();
            modelBuilder.Ignore<PasswordResetTokenId>();
            modelBuilder.Ignore<SavedSearchId>();
            modelBuilder.Ignore<UserSessionId>();
            modelBuilder.Ignore<PropertyId>();
            modelBuilder.Ignore<FavoriteId>();
            modelBuilder.Ignore<AmenityId>();
            modelBuilder.Ignore<PropertyAmenityId>();
            modelBuilder.Ignore<PropertyImageId>();
            modelBuilder.Ignore<InquiryId>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            RegisterStronglyTypedIdConverters(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(GenerateSoftDeleteFilter(entityType.ClrType));
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Generates a query filter expression for soft-deletable entities.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>A lambda expression representing the filter.</returns>
        private static System.Linq.Expressions.LambdaExpression GenerateSoftDeleteFilter(Type entityType)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var body = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
            return System.Linq.Expressions.Expression.Lambda(body, parameter);
        }

        /// <summary>
        /// Registers value converters for strongly-typed IDs.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        private static void RegisterStronglyTypedIdConverters(ModelBuilder modelBuilder)
        {
            var auditLogIdConverter = new ValueConverter<AuditLogId, Guid>(
                id => id.Value,
                value => new AuditLogId(value));

            var roleIdConverter = new ValueConverter<RoleId, Guid>(
                id => id.Value,
                value => new RoleId(value));

            var userIdConverter = new ValueConverter<UserId, Guid>(
                id => id.Value,
                value => new UserId(value));

            var passwordResetTokenIdConverter = new ValueConverter<PasswordResetTokenId, Guid>(
                id => id.Value,
                value => new PasswordResetTokenId(value));

            var savedSearchIdConverter = new ValueConverter<SavedSearchId, Guid>(
               id => id.Value,
               value => new SavedSearchId(value));

            var userSessionIdConverter = new ValueConverter<UserSessionId, Guid>(
                id => id.Value,
                value => new UserSessionId(value));

            var propertyIdConverter = new ValueConverter<PropertyId, Guid>(
                id => id.Value,
                value => new PropertyId(value));

            var favoriteIdConverter = new ValueConverter<FavoriteId, Guid>(
                id => id.Value,
                value => new FavoriteId(value));

            var amenityIdConverter = new ValueConverter<AmenityId, Guid>(
                id => id.Value,
                value => new AmenityId(value));

            var propertyAmenityIdConverter = new ValueConverter<PropertyAmenityId, Guid>(
                id => id.Value,
                value => new PropertyAmenityId(value));

            var propertyImageIdConverter = new ValueConverter<PropertyImageId, Guid>(
                id => id.Value,
                value => new PropertyImageId(value));

            var inquiryIdConverter = new ValueConverter<InquiryId, Guid>(
                id => id.Value,
                value => new InquiryId(value));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(UserId))
                    {
                        property.SetValueConverter(userIdConverter);
                    }
                    else if (property.ClrType == typeof(RoleId))
                    {
                        property.SetValueConverter(roleIdConverter);
                    }
                    else if (property.ClrType == typeof(AuditLogId))
                    {
                        property.SetValueConverter(auditLogIdConverter);
                    }
                    else if (property.ClrType == typeof(PasswordResetTokenId))
                    {
                        property.SetValueConverter(passwordResetTokenIdConverter);
                    }
                    else if (property.ClrType == typeof(SavedSearchId))
                    {
                        property.SetValueConverter(savedSearchIdConverter);
                    }
                    else if (property.ClrType == typeof(UserSessionId))
                    {
                        property.SetValueConverter(userSessionIdConverter);
                    }
                    else if (property.ClrType == typeof(PropertyId))
                    {
                        property.SetValueConverter(propertyIdConverter);
                    }
                    else if (property.ClrType == typeof(FavoriteId))
                    {
                        property.SetValueConverter(favoriteIdConverter);
                    }
                    else if (property.ClrType == typeof(AmenityId))
                    {
                        property.SetValueConverter(amenityIdConverter);
                    }
                    else if (property.ClrType == typeof(PropertyAmenityId))
                    {
                        property.SetValueConverter(propertyAmenityIdConverter);
                    }
                    else if (property.ClrType == typeof(PropertyImageId))
                    {
                        property.SetValueConverter(propertyImageIdConverter);
                    }
                    else if (property.ClrType == typeof(InquiryId))
                    {
                        property.SetValueConverter(inquiryIdConverter);
                    }
                }
            }
        }

        /// <summary>
        /// Applies soft delete logic to entities implementing <see cref="ISoftDeletable"/>.
        /// </summary>
        private void ApplySoftDelete()
        {
            foreach (var entry in this.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.Delete();
                }
            }
        }
    }
}
