using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JweWebApp.Model
{
    public class JweConfiguration
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
        public string EncryptionKey { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public JweConfiguration(IConfigurationSection section)
        {
            ValidAudience = section.GetValue<string>("ValidAudience");
            ValidIssuer = section.GetValue<string>("ValidIssuer");
            Secret = section.GetValue<string>("Secret");
            EncryptionKey = section.GetValue<string>("EncryptionKey");
            AccessTokenExpiration = Convert.ToInt32(section.GetValue<string>("AccessTokenExpiration"));
            RefreshTokenExpiration = Convert.ToInt32(section.GetValue<string>("RefreshTokenExpiration"));
        }
    }
}
