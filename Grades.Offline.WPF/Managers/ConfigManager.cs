using Grades.Offline.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grades.Offline.WPF.Managers
{
    public class ConfigManager
    {
        private static readonly string configFilePath = Path.Combine(Environment.CurrentDirectory, "config.json");

        public static AppConfig GetOrCreateFile()
        {
            // Create configuration file if it doesn't exist
            if (!File.Exists(configFilePath))
            {
                File.Create(configFilePath).Close();
                File.WriteAllText(configFilePath, JsonSerializer.Serialize(new AppConfig
                {
                    Theme = AppTheme.Default,
                    Language = AppLanguage.English
                }));
            }

            // Get Json content
            var config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(configFilePath));

            return config;
        }

        public static void UpdateConfigFile(AppConfig config)
        {
            File.WriteAllText(configFilePath, string.Empty);
            File.WriteAllText(configFilePath, JsonSerializer.Serialize(config));
        }
    }
}
