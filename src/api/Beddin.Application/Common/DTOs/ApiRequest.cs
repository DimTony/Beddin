using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.DTOs
{
    public abstract record ApiRequest
    {
        private string _ipAddress = null!;
        private string _userAgent = null!;

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        public string UserAgent
        {
            get => _userAgent;
            set => _userAgent = value;
        }


    }
}
