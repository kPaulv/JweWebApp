using JweWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JweWebApp.Interfaces
{
    public interface IConfigurationService
    {
        public JweConfiguration GenerateConfiguration();
    }
}
