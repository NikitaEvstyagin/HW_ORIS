using System.Net.Sockets;
using Game.utils;
using PointGame.Paths;

namespace Server;

class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }
    public string? UserName { get; set; }
    public string Color { get; set; }

    TcpClient client;
    ServerObject server; // объект сервера

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        // получаем NetworkStream для взаимодействия с сервером
        var stream = client.GetStream();
        // создаем StreamReader для чтения данных
        Reader = new StreamReader(stream);
        // создаем StreamWriter для отправки данных
        Writer = new StreamWriter(stream);
    }



    public async Task ProcessAsync()
    {
        try
        {
            // получаем имя пользователя
            UserName = await Reader.ReadLineAsync();
            // создаем объект AddUser с передачей имени пользователя и цвета
            var addUserMessage = new AddUser { UserName = UserName };
            await server.BroadcastColoredMessageAsync(addUserMessage, Id);

            var message = $"{UserName} вошел в чат";
            // посылаем сообщение о входе в чат всем подключенным пользователям
            await server.SendListAsync();
            
            Console.WriteLine(message);
            // в бесконечном цикле получаем сообщения от клиента
            while (true)
            {
                await Task.Delay(2000);

                try
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    message = $"{message}";
                    Console.WriteLine(message);
                    await server.BroadcastSquereAsync(message, Id);
                }
                catch
                {
                    message = $"{UserName} покинул чат";
                    Console.WriteLine(message);
                    server.RemoveConnection(Id);
                    await server.SendListAsync();
                    await server.BroadcastMessageAsync(message, Id);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            // в случае выхода из цикла закрываем ресурсы
            server.RemoveConnection(Id);
        }
    }
    // закрытие подключения
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}