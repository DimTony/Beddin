using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Aggregates.Users
{
    public static class UserRoles
    {
        public const string Buyer = "Buyer";
        public const string Owner = "Owner";
        public const string Admin = "Admin";

        public static readonly IReadOnlyList<string> All = new[]
        {
            Buyer, Owner, Admin
        };

        public static bool IsValid(string role) => All.Contains(role);
    }
}
