using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrainGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        ClientWebSocket senderSock;


        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);


            prepareForm();

            startSocketConnection();
        }

        private void prepareForm()
        {
            this.TextBlockStatus.Text = "";
            this.TextBlockConnection.Text = "Disconnected";
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var buffer = WebSocket.CreateClientBuffer(512, 512);
            try
            {
                // Get reply from the server.
                var result = senderSock.ReceiveAsync(buffer, CancellationToken.None);

                var answer = Encoding.UTF8.GetString(buffer.ToArray());

                answer = RemoveSpecialCharacters(answer).Trim();

                if (result.Result.EndOfMessage && answer != "")
                {
                    Console.WriteLine(answer);
                    displayAnswer(answer);
                    startSocketConnection();
                }

            }
            catch (SocketException exception)
            {
                Console.WriteLine("{0} Error code: {1}.", exception.Message, exception.ErrorCode);


                throw exception;
            }
        }

        public void displayAnswer(string answer)
        {
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ':' || c == ';' || c == '{' || c == '}' || c == '"' || c == '_' || c == ',')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        private async void startSocketConnection()
        {
            if (senderSock != null)
            {
                try
                {
                    await senderSock.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
                
            }


            const string host = "ws://10.177.254.60:8080/client/ws/status";
            senderSock = new ClientWebSocket();
            await senderSock.ConnectAsync(new Uri(host), CancellationToken.None);
            dispatcherTimer.Start();
        }

        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length != 2) throw new FormatException("Invalid endpoint format");

            IPAddress ip;
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }

            int port;
            if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }
    }
}