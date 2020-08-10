using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RaGae.BootstrapLib
{
    namespace Loader
    {
        public partial class Loader
        {
            public static T LoadConfig<T>(string fileName, bool optional = false, bool reload = false) where T : new()
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Path.IsPathRooted(fileName) ? Path.GetDirectoryName(fileName) : Directory.GetCurrentDirectory())
                    .AddJsonFile(Path.IsPathRooted(fileName) ? Path.GetFileName(fileName) : fileName, optional, reload)
                    .Build()
                    .Get<T>();
            }

            public static T LoadConfigSection<T>(string fileName, string section = null, bool optional = false, bool reload = false) where T : new()
            {
                T config = new T();

                new ConfigurationBuilder()
                    .SetBasePath(Path.IsPathRooted(fileName) ? Path.GetDirectoryName(fileName) : Directory.GetCurrentDirectory())
                    .AddJsonFile(Path.IsPathRooted(fileName) ? Path.GetFileName(fileName) : fileName, optional, reload)
                    .Build()
                    .GetSection(section ?? typeof(T).Name).Bind(config);

                return config;
            }
        }
    }
}
