using MyHTTPServer.Configuration;
using System.Net;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        HttpListener httpListener = new HttpListener(); ;

        bool stopServerRequested = false;
        string currentDirectory = "../../../";


        //_httpListener.Prefixes.Add($"http://{_appSettings.Configuration!.Address}:{_appSettings.Configuration.Port}/");
        try
        {
            if (!File.Exists(currentDirectory + "appsettings.json"))
            {
                Console.WriteLine("Файл appsetting.json не был найден");
                throw new Exception();
            }

            AppSettingsLoader config = new AppSettingsLoader(currentDirectory);
            config.InitializeAppSettings();

            httpListener.Prefixes.Add($"http://{config.Configuration.Address}:{config.Configuration.Port}/");
            httpListener.Start();
            Console.WriteLine("Server started");

            Task.Run(() =>
            {
                while (httpListener.IsListening)
                {
                    var context = httpListener.GetContext();
                    HttpListenerResponse response = context.Response;
                    string filePath = "../../../index.html";
                    var buffer = File.ReadAllBytes(filePath);
                    using var output = response.OutputStream;
                    output.Write(buffer);
                    output.Flush();
                }
            });

            await Task.Run(() =>
            {
                while (true)
                    if (Console.ReadLine()!.Equals("stop"))
                        break;
            });
            httpListener.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            Console.WriteLine("Работа сервера завершена");
        }
    }
}