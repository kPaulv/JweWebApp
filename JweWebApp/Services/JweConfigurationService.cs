using JweWebApp.Interfaces;
using JweWebApp.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JweWebApp.Services
{
    public class JweConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        public JweConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public JweConfiguration GenerateConfiguration()
        {
            return new JweConfiguration(_configuration.GetSection("JWT"));
        }
    }
}
