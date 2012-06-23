using System;
using System.Windows;
using SignalR.Client.Hubs;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignalRWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HubConnection hubConnection;
        public IHubProxy chatHub;

        public MainWindow()
        {
            InitializeComponent();

            textBlockMessages.TextWrapping = TextWrapping.WrapWithOverflow;

            textBlockMessages.Dispatcher.BeginInvoke(new Action(() => textBlockMessages.Text = "This program, SignalRWpf, begins\n"));

            hubConnection = new HubConnection("http://localhost:49522/");

            hubConnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("Failed to start: {0}", task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Success! Connected with client connection id {0}", hubConnection.ConnectionId);
                    // Do more stuff here
                }
            });

            hubConnection.Received += data =>
                 {
                     HubBub deserializedHubBub = JsonConvert.DeserializeObject<HubBub>(data);
                     var args0 = deserializedHubBub.Args[0];
                     UpdateMessages(args0);
                 };

            chatHub = hubConnection.CreateProxy("Chat");
        }

        private void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {
            chatHub.Invoke("Send", textBoxBroadcast.Text).Wait();
        }

        void UpdateMessages(string msg)
        {
            textBlockMessages.Dispatcher.BeginInvoke(new Action(() => textBlockMessages.Text += msg.ToString()+"\n"));
        }
    }

    public class HubBub
    {
        public string Hub { get; set; }
        public string Method { get; set; }
        public List<string> Args { get; set; }
    }

}
