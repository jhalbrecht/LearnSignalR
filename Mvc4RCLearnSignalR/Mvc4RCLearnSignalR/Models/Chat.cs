﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace Mvc4RCLearnSignalR.Models
{
    // [HubName("chat")]
    public class Chat : Hub
    {
        public void Send(string message)
        {
            // Call the addMessage method on all clients
            Clients.addMessage(message);
        }
    }
}