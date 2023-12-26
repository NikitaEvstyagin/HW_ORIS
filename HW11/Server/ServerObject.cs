using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using PointGame.Paths;
using Server;
using Server.Paths;

class ServerObject
{
    readonly TcpListener _tcpListener = new(IPAddress.Any, 8888); // сервер для прослушивания
    readonly List<ClientObject> _clients = new(); // все подключения

    protected internal void RemoveConnection(string id)
    {
        // получаем по id закрытое подключение
        var client = _clients.FirstOrDefault(c => c.Id.Equals(id));
        // и удаляем его из списка подключений
        if (client != null) _clients.Remove(client);
        client?.Close();
    }
    // прослушивание входящих подключений
    protected internal async Task ListenAsync()
    {
        try
        {
            _tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();

                var clientObject = new ClientObject(tcpClient, this);
                _clients.Add(clientObject);
                Task.Run(clientObject.ProcessAsync);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }

    // трансляция сообщения подключенным клиентам


    private string GenerateRandomColor()
    {
        var random = new Random();
        var color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        var hexColor = ColorTranslator.ToHtml(color);
        while (_clients.Select(i => i.Color).Contains(hexColor))
        {
            color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        return ColorTranslator.ToHtml(color);
    }
    protected internal async Task SendListAsync()
    {
        var sb = new StringBuilder();
        sb.Append("SendList ");
        sb.Append(JsonSerializer.Serialize(_clients.Select(x =>  new AddUser { UserName = x.UserName, Color = x.Color }).ToList()));

        foreach (var client in _clients)
        {
            await client.Writer.WriteLineAsync(sb); //передача данных
            await client.Writer.FlushAsync();
        }
    }
    protected internal async Task BroadcastColoredMessageAsync(AddUser addUser, string id)
    {
        var color = GenerateRandomColor(); // Генерируем случайный цвет
        addUser.Color = color;
        _clients.Last().Color = color;

        var sb = new StringBuilder();
        sb.Append("AddUser ");
        var message = JsonSerializer.Serialize(addUser);
        sb.Append(message);
        {
            try
            {
                await _clients.Last().Writer.WriteLineAsync(sb);
                await _clients.Last().Writer.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке сообщения клиенту: " + ex.Message);
                // Можно добавить дополнительную обработку ошибки, например, удалить клиента из списка, если отправка не удалась
            }
        }
    }
    protected internal async Task BroadcastSquereAsync(string posAndColor, string id) {
        var X = int.Parse(posAndColor.Split()[0]);
        var Y = int.Parse(posAndColor.Split()[1]);
        var color = posAndColor.Split()[2];
        var sendPoint = new SendPoint();
        sendPoint.Color = color;
        sendPoint.Point = new Point(X, Y);
        var sb = new StringBuilder();
        sb.Append("SendPoint ");
        var message = JsonSerializer.Serialize(sendPoint);
        sb.Append(message);
        
        foreach (var client in _clients)
        {
            await client.Writer.WriteLineAsync(sb); //передача данных
            await client.Writer.FlushAsync();
        }

    }


    // отключение всех клиентов
    protected internal void Disconnect()
    {
        foreach (var client in _clients)
        {
            client.Close(); //отключение клиента
        }
        _tcpListener.Stop(); //остановка сервера
    }

    protected internal async Task BroadcastMessageAsync(string message, string id)
    {
        var usersToJson = JsonSerializer.Serialize(_clients.Select(x => x.UserName).ToList());
        foreach (var client in _clients)
        {
            await client.Writer.WriteLineAsync(usersToJson); //передача данных
            await client.Writer.FlushAsync();
        }
    }
}