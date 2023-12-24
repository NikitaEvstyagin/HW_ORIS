using MyHTTPServer.Configuration;
using MyHTTPServer_1.Servises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyHTTPServer
{
    public class ServerHandler
    {
        private readonly HttpListener _httpListener;
        private readonly AppSettingsLoader _appSettings;
        private bool _stopServerRequested;
        private readonly string _currentDirectory = "../../../";
        private string _notFoundHtml;
        private string staticFolder;

        public ServerHandler()
        {
            _httpListener = new HttpListener();
            _appSettings = new AppSettingsLoader(_currentDirectory);
        }

        public async Task Start()
        {
            _appSettings.InitializeAppSettings();
            _httpListener.Prefixes.Add($"http://{_appSettings.Configuration!.Address}:{_appSettings.Configuration.Port}/");
            staticFolder = _currentDirectory + _appSettings.Configuration.StaticFilesPath;
            _notFoundHtml = _currentDirectory + "notFound.html";
            var email = new EmailSenderService(_appSettings.Configuration.MailSender, _appSettings.Configuration.PasswordSender, _appSettings.Configuration.ToEmail, _appSettings.Configuration.SmtpServerHost, _appSettings.Configuration.SmtpServerPort);

            try
            {
                _httpListener.Start();
                Console.WriteLine($"Server started on port {_appSettings.Configuration.Port}");
                var stopThread = new Thread(() =>
                {
                    while (!_stopServerRequested)
                    {
                        var read = Console.ReadLine();
                        // Останавливает через +1 запрос
                        if (read == "stop")
                            _stopServerRequested = true;
                    }
                });
                stopThread.Start();

                if (!CheckIfStaticFolderExists(_appSettings.Configuration.StaticFilesPath))
                    Directory.CreateDirectory(staticFolder);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            while (!_stopServerRequested)
            {

                var context = await _httpListener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                if (request.HttpMethod.Equals("post", StringComparison.OrdinalIgnoreCase)
                    && request.Url.LocalPath.Equals("/send-email"))
                {
                    var stream = new StreamReader(request.InputStream);
                    var streamRead = stream.ReadToEnd();
                    string decodedData = HttpUtility.UrlDecode(streamRead, System.Text.Encoding.UTF8);
                    Console.WriteLine(decodedData);
                    string[] temp_array = decodedData.Split("&");

                    Console.WriteLine(temp_array.Length);
                    if (temp_array.Length == 7)
                        await email.SendEmailAsync(temp_array[0], temp_array[1], temp_array[2], temp_array[3], temp_array[4], temp_array[5], temp_array[6]);
                    else
                        await email.SendEmailAsync(temp_array[0], temp_array[1], temp_array[2], temp_array[3], temp_array[4], temp_array[5], temp_array[6], temp_array[7]);
                }
                Console.WriteLine(request.Url.LocalPath + " " + request.HttpMethod);
                byte[] buffer = null;
                buffer = Router(request.Url);
                var contentType = DetermineContentType(request.Url);
                context.Response.ContentType = $"{contentType}; charset=utf-8";
                response.ContentLength64 = buffer.Length;
                await using Stream output = response.OutputStream;

                await output.WriteAsync(buffer);
                await output.FlushAsync();
            }

            Console.WriteLine("Server stop requested");
            _httpListener.Stop();
        }

        private bool CheckIfStaticFolderExists(string staticFolderPath)
        {
            return Directory.Exists(_currentDirectory + staticFolderPath);
        }

        private bool CheckIfFileExists(string url)
        {
            return File.Exists(url);
        }

        private byte[] NotFoundHtml()
        {
            return File.ReadAllBytes(_notFoundHtml);
        }

        private byte[] Router(Uri url)
        {
            var localPath = url.LocalPath;
            var pathSeparated = localPath.Split("/");
            switch (pathSeparated[1])
            {
                case "":
                    {
                        return CheckIfFileExists(staticFolder + "/" + "index.html")
                            ? File.ReadAllBytes(staticFolder + "/" + "index.html")
                            : NotFoundHtml();
                    }
                case "static":
                    {
                        if (pathSeparated.Length < 3)
                            return NotFoundHtml();
                        return CheckIfFileExists(staticFolder + "/" + pathSeparated[2])
                            ? File.ReadAllBytes(staticFolder + "/" + pathSeparated[2])
                            : NotFoundHtml();
                    }
                case "send-email":
                    {
                        return CheckIfFileExists(staticFolder + "/" + "index.html")
                            ? File.ReadAllBytes(staticFolder + "/" + "index.html")
                            : NotFoundHtml();
                    }
                default:
                    return CheckIfFileExists(staticFolder + localPath)
                        ? File.ReadAllBytes(staticFolder + localPath)
                        : NotFoundHtml();
            }

            return Array.Empty<byte>();
        }

        private string DetermineContentType(Uri url)
        {
            var stringUrl = url.ToString();
            var extension = "";

            try
            {
                extension = stringUrl.Substring(stringUrl.LastIndexOf('.'));
            }
            catch (Exception e)
            {
                extension = "text/html";
                return extension;
            }

            var contentType = "";

            switch (extension)
            {
                case ".htm":
                case ".html":
                    contentType = "text/html";
                    break;
                case ".css":
                    contentType = "text/css";
                    break;
                case ".js":
                    contentType = "text/javascript";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".jpeg":
                case ".png":
                case ".gif":
                    contentType = "image/" + extension.Substring(1);
                    break;
                case ".svg":
                    contentType = "image/svg+xml";
                    break;
                default:
                    if (extension.Length > 1)
                    {
                        contentType = "application/" + extension.Substring(1);
                    }
                    else
                    {
                        contentType = "application/unknown";
                    }
                    break;
            }

            return contentType;
        }
    }
}
