using PointGame.Paths;
using Server.Paths;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text.Json;

using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PointGame
{
    public partial class Form1 : Form
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        public Form1()
        { 
            InitializeComponent();
            listOfUsers.Visible = false;
            testLabel.Visible = false;
            color.Visible = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void btn_signIn_Click(object sender, EventArgs e)
        {
            string host = "127.0.0.1";
            int port = 8888;
            string userName = enterName.Text;

            try
            {
                _client = new TcpClient();
                _client.Connect(host, port);

                _reader = new StreamReader(_client.GetStream());
                _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };

                // запускаем новый поток для получения данных
                Task.Run(() => ReceiveMessageAsync(_reader));

                // отправляем имя пользователя
                await EnterUserAsync(_writer, userName);

                // обновляем интерфейс
                testLabel.Text = userName;
                label1.Visible = false;
                enterName.Visible = false;
                btn_signIn.Visible = false;
                listOfUsers.Visible = true;
                testLabel.Visible = true;
                color.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    // считываем ответ в виде строки
                    string message = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;

                    // обновляем интерфейс с использованием Invoke, так как это происходит в отдельном потоке
                    Invoke((MethodInvoker)delegate
                    {
                        Print(message);
                    });
                }
                catch (IOException)
                {
                    // Исключение возникает, если считывание из закрытого потока
                    // может произойти, если сервер отключил клиента
                    MessageBox.Show("Сервер отключил клиента.");
                    break;
                }
                catch (Exception)
                {
                    // Любые другие исключения, которые могут возникнуть при считывании
                }
            }
        }

        // чтобы полученное сообщение не накладывалось на ввод нового сообщения
        private void Print(string message)
        {
            var messageSplit = message.Split();
            var messageType = messageSplit[0];
            var messageJson = messageSplit[1];

            switch (messageType)
            {
                case "SendList":
                    {
                        var users = JsonSerializer.Deserialize<List<AddUser>>(messageJson)
                    ?? throw new ArgumentNullException(nameof(messageJson));

                        listOfUsers.Items.Clear();
                        for (int i = 0; i < users.Count; i++)
                        {
                            listOfUsers.Items.Add(users[i].UserName);
                            listOfUsers.Items[i].BackColor = ColorTranslator.FromHtml(users[i].Color);
                        }
                        break;
                    }
                case "AddUser":
                    {
                        var addUser = JsonSerializer.Deserialize<AddUser>(messageJson)
                      ?? throw new ArgumentNullException(nameof(messageJson));

                        Console.WriteLine(addUser.Color);
                        label1.Text = addUser.UserName;
                        color.BackColor = ColorTranslator.FromHtml(addUser.Color);
                        color.Visible = true;
                        InitializeCircleGrid();
                        break;
                    }
                case "SendPoint":
                    {
                        var sendPoint = JsonSerializer.Deserialize<SendPoint>(messageJson)
                            ?? throw new ArgumentNullException(nameof(messageJson));

                        _panels[sendPoint.Point.X, sendPoint.Point.Y].BackColor = ColorTranslator.FromHtml(sendPoint.Color);


                        break;
                    }
            }
            
         
        }



        async Task EnterUserAsync(StreamWriter writer, string userName)
        {
            // сначала отправляем имя
            await writer.WriteLineAsync(userName);
            await writer.FlushAsync();
            //Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");

            //while (true)
            //{
            //    string? message = Console.ReadLine();
            //    await writer.WriteLineAsync(message);
            //    await writer.FlushAsync();
            //}
        }

        private async Task Form1_FormClosingAsync(object sender, FormClosingEventArgs e)
        {
            // Закрытие ресурсов
            _reader?.Close();
            _writer?.Close();
            _client?.Close();
        }
        
        private static int gridSize = 15;
        Panel[,] _panels = new Panel[gridSize, gridSize];
        private void InitializeCircleGrid()
        {

           
            int size = 20;
            int initialX = 20;
            int initialY = 20;
            

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    _panels[i, j] = new Panel();
                    _panels[i, j].Location = new Point(initialX + j*(size+5), initialY + i*(size+5));
                    _panels[i, j].Size = new Size(size, size);
                    _panels[i, j].Click += (sender, e) => ChangeColorAsync(sender, e);
                    _panels[i, j].Tag = new Point(i, j); // Позиция
                    _panels[i, j].BorderStyle = BorderStyle.FixedSingle; // Добавляем границу для лучшей видимости
                    this.Controls.Add(_panels[i, j]);
                }
            }

            this.Size = new Size(initialX + (size + 5) * gridSize, initialY + (size + 5) * gridSize);
        }

        private void ChangeColorAsync(object sender, EventArgs e)
        {
            Panel selected = (Panel)sender;
            selected.BackColor = color.BackColor;
            var position = (Point)selected.Tag;
            SendPanelCoordinateToServer(_writer, position.X, position.Y, ColorTranslator.ToHtml(color.BackColor));
        }
        void SendPanelCoordinateToServer(StreamWriter writer, int x, int y, string color)
        {
            try
            {
                string coordinateData = $"{x} {y} {color}"; // Формируем строку с координатами
                writer.WriteLine(coordinateData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки данных на сервер: {ex.Message}");
            }
        }

    }

}