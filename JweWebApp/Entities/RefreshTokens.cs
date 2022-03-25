using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JweWebApp.Entities
{
    public class RefreshTokens
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
