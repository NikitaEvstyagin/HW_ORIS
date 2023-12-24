using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHTTPServer.Configuration
{
    public class AppSettingsLoader
    {
        public string Path { get; private set; }
        public string CurrentDirectory { get; private set; }
        public AppSettingsClass? Configuration { get; private set; }

        public AppSettingsLoader(string currentDirectory)
        {
            CurrentDirectory = currentDirectory;
            Path = CurrentDirectory + "appsettings.json";
        }

        public void InitializeAppSettings()
        {
            try
            {
                using var sr = new StreamReader(Path);
                var json = sr.ReadToEnd();
                Configuration = JsonSerializer.Deserialize<AppSettingsClass>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (Configuration is null)
                throw new ArgumentException("appsettings.json not found");
        }
    }
}
