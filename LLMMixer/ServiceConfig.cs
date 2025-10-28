using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace LLMMixer
{
    public class ServiceInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
        public int Order { get; set; }
        public double Width { get; set; } = 1.0;
    }

    public class AppSettings
    {
        public List<ServiceInfo> Services { get; set; } = new List<ServiceInfo>();

        public static AppSettings Load()
        {
            try
            {
                string settingsPath = GetSettingsPath();
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                    return settings ?? GetDefaultSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            return GetDefaultSettings();
        }

        public void Save()
        {
            try
            {
                string settingsPath = GetSettingsPath();
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private static string GetSettingsPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "LLMMixer");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            return Path.Combine(appFolder, "settings.json");
        }

        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings
            {
                Services = new List<ServiceInfo>
                {
                    new ServiceInfo { Name = "ChatGPT", Url = "https://chat.openai.com", IsVisible = true, Order = 0, Width = 1.0 },
                    new ServiceInfo { Name = "Claude", Url = "https://claude.ai", IsVisible = true, Order = 1, Width = 1.0 },
                    new ServiceInfo { Name = "DeepSeek", Url = "https://chat.deepseek.com", IsVisible = true, Order = 2, Width = 1.0 },
                    new ServiceInfo { Name = "Gemini", Url = "https://gemini.google.com", IsVisible = true, Order = 3, Width = 1.0 },
                    new ServiceInfo { Name = "Grok", Url = "https://grok.com", IsVisible = true, Order = 4, Width = 1.0 },
                    new ServiceInfo { Name = "Kimi", Url = "https://www.kimi.com/en/", IsVisible = true, Order = 5, Width = 1.0 },
                    new ServiceInfo { Name = "Mistral", Url = "https://chat.mistral.ai", IsVisible = true, Order = 6, Width = 1.0 },
                    new ServiceInfo { Name = "Qwen", Url = "https://chat.qwen.ai", IsVisible = true, Order = 7, Width = 1.0 }
                }
            };
        }
    }
}
