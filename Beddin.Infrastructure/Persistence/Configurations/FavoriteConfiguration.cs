using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, v => new FavoriteId(v));

            builder.HasIndex(f => new { f.UserId, f.PropertyId })
                .IsUnique();

            builder.HasIndex(f => f.UserId);     // fast "my favorites"
            builder.HasIndex(f => f.PropertyId); // fast "who favorited this"

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
