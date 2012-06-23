using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SignalR.Client.Hubs;
using Newtonsoft.Json;

namespace Wp7SignalR
{
    public partial class MainPage : PhoneApplicationPage
    {
        public HubConnection hubConnection;
        public IHubProxy chatHub;

        public MainPage()
        {
            InitializeComponent();

            textBlockMessages.Dispatcher.BeginInvoke(new Action(() => textBlockMessages.Text = "This program, SignalRWp7, begins\n"));

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

        private void buttonBroadcast_Click(object sender, RoutedEventArgs e)
        {
            chatHub.Invoke("Send", textBoxBroadcast.Text);// .Wait();
            textBoxBroadcast.Text = "";
        }

        void UpdateMessages(string msg)
        {
            textBlockMessages.Dispatcher.BeginInvoke(new Action(() => textBlockMessages.Text += msg.ToString() + "\n"));
        }
    }

    public class HubBub
    {
        public string Hub { get; set; }
        public string Method { get; set; }
        public List<string> Args { get; set; }
    }
}