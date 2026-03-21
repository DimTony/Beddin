using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Persistence
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid UserId { get; set; } // Link to DDD User
    }
}
