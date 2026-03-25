using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Options
{
    public class EmailOptions
    {
        public const string SectionName = "EmailOptions";
        public string BaseUrl { get; set; } = default!;
        public string TestJwtSecret { get; set; } = default!;
        public string SmtpServer { get; init; } = default!;
        public int Port { get; init; } = default!;
        public string FromEmail { get; init; } = default!;
        public string FromName { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}
