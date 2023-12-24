using MyHTTPServer;
using System.Diagnostics.Tracing;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Schema;

public class Program
{
    public static async Task Main(string[] args)
    {
        var server = new ServerHandler();
        await server.Start();
    }
}

