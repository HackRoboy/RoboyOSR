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
using Newtonsoft.Json.Linq;
using WpfAnimatedGif;

namespace BrainGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientWebSocket senderSock;


        public MainWindow()
        {
            InitializeComponent();

            prepareForm();

            startSocketConnection();
        }

        private void prepareForm()
        {
            this.TextBlockStatus.Text = "";
            this.TextBlockConnection.Text = "Disconnected";
        }


        private async void listenToSocket()
        {
            try
            {
                var buffer = new byte[1024];
                while (true)
                {
                    var segment = new ArraySegment<byte>(buffer);

                    var result = await senderSock.ReceiveAsync(segment, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await senderSock.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "I don't do binary",
                            CancellationToken.None);
                        return;
                    }

                    int count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            await senderSock.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long",
                                CancellationToken.None);
                            return;
                        }

                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await senderSock.ReceiveAsync(segment, CancellationToken.None);
                        count += result.Count;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, count);


                    message = RemoveSpecialCharacters(message).Trim();


                    displayAnswer(message);
                    Console.WriteLine(">" + message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("{0}", exception.Message);
            }
        }

        public void displayAnswer(string answer)
        {
            if (answer == "")
            {
                TextBlockConnection.Text = "Disconnected";
                TextBlockStatus.Text += "Waiting for signals..." + "\n";
            }
            else
            {
                dynamic stuff = JObject.Parse(answer);

                int workersAvailable = stuff.num_workers_available;
                int requestsProcessed = stuff.num_requests_processed;

                TextBlockStatus.Text += answer + "\n";

                setGif(isRoboyThinking: workersAvailable <= 0);
                TextBlockConnection.Text = "Connected";
                
            }
        }


        private void setGif(bool isRoboyThinking)
        {
            var controller = ImageBehavior.GetAnimationController(Gif);
            if (isRoboyThinking)
            {
                // Resume the animation (or restart it if it was completed)
                controller.Play();
            }
            else
            {
                // Pause the animation
                controller.Pause();

                // Go to the last frame
                controller.GotoFrame(0);
            }
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ':' ||
                    c == ';' || c == '{' || c == '}' || c == '"' || c == '_' || c == ',')
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

            try
            {
                string host = Properties.Settings.Default.ip;
                senderSock = new ClientWebSocket();
                await senderSock.ConnectAsync(new Uri(host), CancellationToken.None);

                listenToSocket();
                //dispatcherTimer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}