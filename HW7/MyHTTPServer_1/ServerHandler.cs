using MyHTTPServer_1.Handlers;
using MyHTTPServer_1.Servises;
using MyHTTPServer_1.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyHttpServer.Handlers;

namespace MyHTTPServer
{
    public class ServerHandler
    {
        private HttpListener Listener { get; }

        private readonly string _prefix;

        private readonly StaticFilesHandler _staticFilesHandler = new();

        private Handler? _controllersHandler = new ControllersHandler();

        private AppSettingsClass Config { get; set; }

        public ServerHandler(HttpListener listener)
        {
            Listener = listener;
            var appSettingLoader = new AppSettingsLoader("../../../");
            appSettingLoader.InitializeAppSettings();
            Config = appSettingLoader.Configuration!;
            _prefix = $"http://{Config.Address}:{Config.Port}/";
        }

        public async Task Start()
        {
            try
            {

                Listener.Prefixes.Add(_prefix);
                Listener.Start();
                Console.WriteLine("Server started");

                Task.Run(async () =>
                {
                    while (Listener.IsListening)
                    {
                        var context = await Listener.GetContextAsync();
                        _staticFilesHandler.Successor = _controllersHandler;
                        _staticFilesHandler.HandleRequest(context);
                    }
                });


                await Task.Run(() =>
                {
                    while (true)
                        if (Console.ReadLine()!.Equals("stop"))
                            break;
                });

                Listener.Stop();
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
}
