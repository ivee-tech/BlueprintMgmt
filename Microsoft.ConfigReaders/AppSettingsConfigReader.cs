using Microsoft.CommonLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ConfigReaders
{
    public class AppSettingsConfigReader : IConfigReader
    {
        public string this[string name]
        {
            get
            {
                return ConfigurationManager.AppSettings[name];
            }
        }
        public string GetValue(string name)
        {
            return this[name];
        }

        public async Task<string> GetValueAsync(string name)
        {
            return await Task.Run(() => this[name]);
        }

    }
}
