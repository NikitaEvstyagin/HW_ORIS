using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHTTPServer_1.Configuration
{
    public class AppSettingsLoader
    {
        public string Path { get; private set; }
        public string CurrentDirectory { get; private set; }
        public AppSettingsClass? Configuration { get; private set; }

        private static bool _isInitialized;

        private static AppSettingsLoader? _instance;

        public AppSettingsLoader(string currentDirectory)
        {
            CurrentDirectory = currentDirectory;
            Path = CurrentDirectory + "appsettings.json";
        }

        private AppSettingsLoader(string currentDirectory, AppSettingsClass configuration)
        {
            CurrentDirectory = currentDirectory;
            Path = CurrentDirectory + "appsettings.json";
            Configuration = configuration;
        }

        public void InitializeAppSettings()
        {
            try
            {
                using var sr = new StreamReader(Path);
                var json = sr.ReadToEnd();
                Configuration = JsonSerializer.Deserialize<AppSettingsClass>(json);
                _isInitialized = true;
                _instance = new AppSettingsLoader(Path, Configuration);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (Configuration is null)
                throw new ArgumentException("appsettings.json not found");
        }


        public static AppSettingsLoader? Instance()
        {
            if (_isInitialized)
                return _instance;
            throw new InvalidOperationException("DataServer Singleton is not initialized");
        }

    }
}
