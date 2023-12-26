using MyHttpServer.Handlers;
using MyHTTPServer_1.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyHTTPServer_1.Handlers
{
    public class ControllersHandler : Handler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                using var response = context.Response;

                var strParams = request.Url!
                    .Segments
                    .Skip(1)
                    .Select(s => s.Replace("/", ""))
                    .ToArray();

                var controllerName = strParams[0];
                var methodName = strParams[1];
                var assembly = Assembly.GetExecutingAssembly();

                var controller = assembly.GetTypes()
                    .Where(t => Attribute.IsDefined(t, typeof(ControllerAttribute)))
                    .FirstOrDefault(c =>
                        ((ControllerAttribute)Attribute.GetCustomAttribute(c, typeof(ControllerAttribute))!)
                        .ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase));

                var method = (controller?.GetMethods()!)
                    .FirstOrDefault(x => x.GetCustomAttributes(true)
                        .Any(attr => attr.GetType().Name.Equals($"{request.HttpMethod}Attribute",
                                         StringComparison.OrdinalIgnoreCase) &&
                                     ((HttpMethodAttribute)attr).ActionName.Equals(methodName,
                                         StringComparison.OrdinalIgnoreCase)));

                var queryParams = Array.Empty<object>();


                switch (request.HttpMethod.ToLower())
                {
                    case "post":
                        {
                            using var streamReader = new StreamReader(context.Request.InputStream);
                            var tempOfData = streamReader.ReadToEnd();
                            var formData = new[] { "" };

                            if (!string.IsNullOrEmpty(tempOfData))
                            {
                                formData = WebUtility
                                        .UrlDecode(tempOfData)
                                        .Split('&')
                                        .Select(param => param.Split('=')[1])
                                        .ToArray();
                            }

                            if (formData.Length > 1)
                            {
                                queryParams = method?.GetParameters()
                                    .Select((p, i) => Convert.ChangeType(formData[i], p.ParameterType))
                                    .ToArray();
                            }
                            break;
                        }
                }

                switch (method.Name.ToLower()) {
                    case "add":
                        {
                            var query = request.Url.Query; // Получаем часть URL с параметрами ?login=evs@mail.ru&password=qwertyui

                            if (!string.IsNullOrEmpty(query))
                            {
                                var parseQueryParams = HttpUtility.ParseQueryString(query); // Парсим query-параметры

                                var login = parseQueryParams["login"]; // Получаем значение параметра "login"
                                var password = parseQueryParams["password"]; // Получаем значение параметра "password"
                                var formData = new List<string>() { login, password};
                                if (formData.Count > 0)
                                {
                                    queryParams = method?.GetParameters()
                                        .Select((p, i) => Convert.ChangeType(formData[i], p.ParameterType))
                                        .ToArray();
                                }

                            }
                            break;                        
                        }
                    case "delete":
                        {
                            var query = request.Url.Query; // Получаем часть URL с параметрами ?login=evs@mail.ru&password=qwertyui

                            if (!string.IsNullOrEmpty(query))
                            {
                                var parseQueryParams = HttpUtility.ParseQueryString(query); // Парсим query-параметры
                                var formData = new List<string>() { parseQueryParams["id"] };
                                if (formData.Count > 0)
                                {
                                    queryParams = method?.GetParameters()
                                        .Select((p, i) => Convert.ChangeType(formData[i], p.ParameterType))
                                        .ToArray();
                                }
                            }
                                break;
                        }
                    case "getbyid":
                        {
                            var query = request.Url.Query;
                            if (!string.IsNullOrEmpty(query))
                            {
                                var parseQueryParams = HttpUtility.ParseQueryString(query); // Парсим query-параметры
                                var formData = new List<string>() { parseQueryParams["id"] };
                                if (formData.Count > 0)
                                {
                                    queryParams = method?.GetParameters()
                                        .Select((p, i) => Convert.ChangeType(formData[i], p.ParameterType))
                                        .ToArray();
                                }
                            }

                            break;
                        }
                    case "update":
                        {

                            var query = request.Url.Query;
                            if (!string.IsNullOrEmpty(query))
                            {
                                var parseQueryParams = HttpUtility.ParseQueryString(query); // Парсим query-параметры
                                var formData = new List<string>() { parseQueryParams["id"], parseQueryParams["email"], parseQueryParams["password"] };
                                if (formData.Count > 0)
                                {
                                    queryParams = method?.GetParameters()
                                        .Select((p, i) => Convert.ChangeType(formData[i], p.ParameterType))
                                        .ToArray();
                                }
                            }
                            break;
                        }
                }

                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), queryParams);

                if (!(method!.ReturnType == typeof(void)))
                    ProcessResult(resultFromMethod, response, context);

                else
                    response.Redirect("http://127.0.0.1:2323/");
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ProcessResult<T>(T result, HttpListenerResponse response, HttpListenerContext context)
        {
            switch (result)
            {
                case string resultOfString:
                    {
                        response.ContentType = StaticFilesHandler.GetContentType(context.Request.Url!.LocalPath);
                        var buffer = Encoding.UTF8.GetBytes(resultOfString);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        break;
                    }
                case not null:
                    {
                        response.ContentType = StaticFilesHandler.GetContentType(context.Request.Url!.LocalPath);
                        var json = JsonConvert.SerializeObject(result);
                        var buffer = Encoding.UTF8.GetBytes(json);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        break;
                    }
            }
        }
    }
}
