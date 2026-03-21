using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beddin.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IReadDbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();

        IQueryable<UserSession> IReadDbContext.UserSessions => UserSessions;
        IQueryable<Property> IReadDbContext.Properties => Properties;
        IQueryable<Favorite> IReadDbContext.Favorites => Favorites;
        IQueryable<Amenity> IReadDbContext.Amenities => Amenities;
        IQueryable<PropertyAmenity> IReadDbContext.PropertyAmenities => PropertyAmenities;
        IQueryable<PropertyImage> IReadDbContext.PropertyImages => PropertyImages;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.Ignore<UserId>();
            modelBuilder.Ignore<UserSessionId>();
            modelBuilder.Ignore<PropertyId>();
            modelBuilder.Ignore<FavoriteId>();
            modelBuilder.Ignore<AmenityId>();
            modelBuilder.Ignore<PropertyAmenityId>();
            modelBuilder.Ignore<PropertyImageId>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            RegisterStronglyTypedIdConverters(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
        private static void RegisterStronglyTypedIdConverters(ModelBuilder modelBuilder)
        {
            var userIdConverter = new ValueConverter<UserId, Guid>(
                id => id.Value,
                value => new UserId(value));

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

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
            
                    if (property.ClrType == typeof(UserId))
                        property.SetValueConverter(userIdConverter);
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
                }
            }
        }

    }
}
