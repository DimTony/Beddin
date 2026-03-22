using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Beddin.Domain.Aggregates.AuditLog;

namespace Beddin.Infrastructure.Persistence
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>, IReadDbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<User> AppUsers => Set<User>();
        public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<Inquiry> Inquiries => Set<Inquiry>();

        IQueryable<AuditLog> IReadDbContext.AuditLogs => AuditLogs;
        IQueryable<User> IReadDbContext.AppUsers => AppUsers;
        IQueryable<SavedSearch> IReadDbContext.SavedSearches => SavedSearches;
        IQueryable<UserSession> IReadDbContext.UserSessions => UserSessions;
        IQueryable<Property> IReadDbContext.Properties => Properties;
        IQueryable<Favorite> IReadDbContext.Favorites => Favorites;
        IQueryable<Amenity> IReadDbContext.Amenities => Amenities;
        IQueryable<PropertyAmenity> IReadDbContext.PropertyAmenities => PropertyAmenities;
        IQueryable<PropertyImage> IReadDbContext.PropertyImages => PropertyImages;
        IQueryable<Inquiry> IReadDbContext.Inquiries => Inquiries;
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.Ignore<AuditLogId>();
            modelBuilder.Ignore<UserId>();
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
        private void ApplySoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.Delete();
                }
            }
        }
        private static System.Linq.Expressions.LambdaExpression GenerateSoftDeleteFilter(Type entityType)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var body = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
            return System.Linq.Expressions.Expression.Lambda(body, parameter);
        }
        private static void RegisterStronglyTypedIdConverters(ModelBuilder modelBuilder)
        {
            var auditLogIdConverter = new ValueConverter<AuditLogId, Guid>(
                id => id.Value,
                value => new AuditLogId(value));

            var userIdConverter = new ValueConverter<UserId, Guid>(
                id => id.Value,
                value => new UserId(value));

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
                        property.SetValueConverter(userIdConverter);
                    else if (property.ClrType == typeof(AuditLogId))
                        property.SetValueConverter(auditLogIdConverter);
                    else if (property.ClrType == typeof(SavedSearchId))
                        property.SetValueConverter(savedSearchIdConverter);
                    else if (property.ClrType == typeof(UserSessionId))
                        property.SetValueConverter(userSessionIdConverter);
                    else if (property.ClrType == typeof(PropertyId))
                        property.SetValueConverter(propertyIdConverter);
                    else if (property.ClrType == typeof(FavoriteId))
                        property.SetValueConverter(favoriteIdConverter);
                    else if (property.ClrType == typeof(AmenityId))
                        property.SetValueConverter(amenityIdConverter);
                    else if (property.ClrType == typeof(PropertyAmenityId))
                        property.SetValueConverter(propertyAmenityIdConverter);
                    else if (property.ClrType == typeof(PropertyImageId))
                        property.SetValueConverter(propertyImageIdConverter);
                    else if (property.ClrType == typeof(InquiryId))
                        property.SetValueConverter(inquiryIdConverter);
                }
            }
        }

    }
}
