using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Interfaces
{
    public interface IPasswordService
    {
        string GenerateTemporaryPassword(int length = 12);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
