using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using System.Threading;
using Gruppe22.Backend;


namespace DungeonServer
{
    /// <summary>
    /// This class represents the main console application
    /// Current version copied almost verbatim from http://www.indiedev.de/wiki/Netzwerk-Basics_mit_Lidgren/Erstellen_des_Servers 
    /// </summary>
    public class Server
    {
        #region Private Fields
        /// <summary>
        /// Configuration info used by Lidgren
        /// </summary>
        private static NetPeerConfiguration _config;
        /// <summary>
        /// Server object provided by Lidgren
        /// </summary>
        private static NetServer _server;
        private static string _servername = "Crawler 2000";
        /// <summary>
        /// A List of CLients and their respective data
        /// </summary>
        private static Dictionary<IPEndPoint, ClientData> _clients;
        private static List<string> discoveredBy;
        /// <summary>
        /// A thread to separate user interface and network activity
        /// </summary>
        private static Thread _serverThread;
        #endregion

        #region Private Methods

        /// <summary>
        /// Broadcast a message to all clients
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method"></param>
        /// <param name="impossibleClients"></param>
        private static void SendMessageToAll(NetOutgoingMessage message, NetDeliveryMethod method, params IPEndPoint[] impossibleClients)
        {
            if (impossibleClients == null)
            {
                if (_clients.Count == 0)
                    return;
                List<NetConnection> connections = (from client in _clients select client.Value.Connection).ToList();
                _server.SendMessage(message, connections, method, 0);
            }
            else
            {
                foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
                {
                    if (!impossibleClients.Contains(client.Key))
                        _server.SendMessage(message, client.Value.Connection, method);
                }
            }
        }

        /// <summary>
        /// Work on the message queue (reacting to all incoming messages)
        /// </summary>
        /// <param name="param"></param>
        private static void WorkMessages(object param)
        {

            NetServer server = (NetServer)param;

            //Message-Cycle: Dauerhaft Messages verarbeiten, bis das Programm beendet wird bzw. ein Fehler auftritt
            NetIncomingMessage message;
            NetOutgoingMessage response;
            while (_serverThread.ThreadState != ThreadState.AbortRequested)
            {
                if ((message = server.ReadMessage()) == null)
                    continue;
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                        Log(message.ReadString(), ConsoleColor.Magenta);
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        Log(message.ReadString(), ConsoleColor.Yellow);
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        Log(message.ReadString(), ConsoleColor.Red);
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:

                        response = server.CreateMessage(_servername);

                        server.SendDiscoveryResponse(response, message.SenderEndpoint);
                        if (!discoveredBy.Contains(message.SenderEndpoint.Address.ToString()))
                        {
                            Log(message.SenderEndpoint + " has discovered the server!");
                            discoveredBy.Add(message.SenderEndpoint.Address.ToString());
                        }
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        //Clients die Connecten wollen dies erlauben
                        message.SenderConnection.Approve();
                        Log(message.SenderEndpoint + " approved.");

                        break;
                    case NetIncomingMessageType.Data:
                        //Sämtliche Daten die Clients senden annehmen
                        byte type = message.ReadByte();
                        ProcessMessage(type, message); //Wir leiten die Message weiter und verarbeiten sie, je nach Typ

                        break;
                    case NetIncomingMessageType.StatusChanged:
                        //Falls ein Client connected / disconnected
                        NetConnectionStatus state = (NetConnectionStatus)message.ReadByte();
                        if (state == NetConnectionStatus.Disconnected || state == NetConnectionStatus.Disconnecting)
                        {
                            if (!_clients.ContainsKey(message.SenderEndpoint))
                                break;

                            response = server.CreateMessage();
                            response.Write((byte)PacketType.Disconnect);
                            response.Write(_clients[message.SenderEndpoint].ID); //ID mitteilen
                            SendMessageToAll(response, NetDeliveryMethod.ReliableUnordered, message.SenderEndpoint);
                            Log("Sent Disconnect to all except " + _clients[message.SenderEndpoint].ID, ConsoleColor.Cyan);
                            //Allen anderen dies mitteilen

                            _clients.Remove(message.SenderEndpoint);
                            Log(message.SenderEndpoint + " disconnected!");
                        }
                        else if (state == NetConnectionStatus.Connected)
                        {
                            if (_clients.ContainsKey(message.SenderEndpoint)) break;

                            ClientData newClient = new ClientData(message.SenderConnection, ClientData.GetFreeID(_clients)); //Neuen Clienten erstellen
                            //Nun fügen wir den Client zur Liste hinzu (IP, ClienData):
                            _clients.Add(message.SenderEndpoint, newClient);
                            Console.WriteLine("Created client with id '" + newClient.ID + "'!");
                            response = server.CreateMessage();
                            response.Write((byte)PacketType.Connect); //0x02: Clientinformation um neuen Clienten seine ID mitzuteilen
                            response.Write(newClient.ID);
                            response.Write((short)_clients.Count); //Anzahl aktueller Clients senden
                            server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableUnordered);
                            Log("Sent Connect to " + newClient.ID, ConsoleColor.Cyan);

                            //Nun alle aktiven Clients informieren, dass ein neuer Client connected ist
                            response = server.CreateMessage();
                            response.Write((byte)PacketType.UpdateClients); //0x00: Neuer Client connected
                            response.Write(newClient.ID); //Seine ID mitteilen
                            // response.Write(newClient.Position);
                            SendMessageToAll(response, NetDeliveryMethod.ReliableUnordered, message.SenderEndpoint);
                            Log("Sent UpdateClients to all", ConsoleColor.Cyan);

                            Log(message.SenderEndpoint + " connected!");
                        }
                        break;
                    default:
                        Log("Unhandled Messagetype: " + message.MessageType, ConsoleColor.Red);
                        break;
                }
                //Weiterer Code
            }
        }

        /// <summary>
        /// Deal with an individual message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        private static void ProcessMessage(byte id, NetIncomingMessage message)
        {
            short senderID = _clients[message.SenderEndpoint].ID;
            NetOutgoingMessage response;
            switch ((PacketType)id)
            {



                case PacketType.Chat:
                    response = _server.CreateMessage();
                    response.Write((byte)PacketType.Chat); //0x10: Positionsänderung eines Clients
                    response.Write(senderID); //Dessen ID
                    response.Write(message.ReadString());
                    SendMessageToAll(response, NetDeliveryMethod.Unreliable, message.SenderEndpoint);
                    break;




                case PacketType.Move: //0x0 steht für Positions-Informationen eines Clienten
                    //  Vector2 _position = message.ReadVector2();
                    //_clients[message.SenderEndpoint].Position = _position;
                    //  Vector2 _direction = message.ReadVector2();
                    // _clients[message.SenderEndpoint].Direction = _direction;

                    //Wir haben die Position, also an alle anderen Clients senden
                    response = _server.CreateMessage();
                    response.Write((byte)0x10); //0x10: Positionsänderung eines Clients
                    response.Write(senderID); //Dessen ID
                    //   response.Write(_position); //Seine neue Position
                    //    response.Write(_direction);
                    //    response.Write(_clients[message.SenderEndpoint].Speed);
                    SendMessageToAll(response, NetDeliveryMethod.Unreliable, message.SenderEndpoint);
                    break;
                case PacketType.UpdateClients:
                    //Client fordert eine komplette Liste aller Clients mit deren ID und Position
                    List<ClientData> clients =
                        (from client in _clients
                         where (client).Value.ID != _clients[message.SenderEndpoint].ID
                         select client.Value).ToList();

                    foreach (ClientData client in clients)
                    {
                        response = _server.CreateMessage();
                        response.Write((byte)PacketType.UpdateClients);
                        response.Write(client.ID);
                        //     response.Write(client.Position);
                        //     response.Write(client.Direction);
                        //   response.Write(client.Speed);
                        _server.SendMessage(response, _clients[message.SenderEndpoint].Connection,
                                            NetDeliveryMethod.ReliableUnordered);
                    }
                    Log("Sent UpdateClients to " + _clients[message.SenderEndpoint].ID, ConsoleColor.Cyan);
                    break;
            }
        }

        /// <summary>
        /// Add text to log (in specified color)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        private static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now + "]:  " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Main loop used for console UI
        /// </summary>
        private static void WaitForInput()
        {
            Console.WriteLine("Enter exit to quit; clear to clear screen or stats for statistics.");
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "exit":
                        return;
                    case "clear":
                        Console.Clear();
                        break;
                    case "stats":
                        Log(_server.Statistics.ToString(), ConsoleColor.Magenta);
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }
        #endregion

        #region Main Method (public)

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            bool error = false;
            Log("Setting up server ...");

            _clients = new Dictionary<IPEndPoint, ClientData>();
            discoveredBy = new List<string>();
            if (args.Length > 0)
            {
                _servername = args[0];
            }
            try
            {
                _config = new NetPeerConfiguration("DungeonCrawler");
                _config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                _config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                _config.Port = 666;
                _config.UseMessageRecycling = true;
                _config.EnableUPnP = true;

                _server = new NetServer(_config);
                _server.Start();
            }
            catch (Exception ex)
            {
                Log("Error during startup: " + ex.Message);
                error = true;
            }

            if (!error)
            {
                _serverThread = new Thread(WorkMessages);
                _serverThread.Start(_server);
                IPAddress mask;
                Log("Server started successfully on " + NetUtility.GetMyAddress(out mask).ToString() + " (" + _server.UPnP.GetExternalIP() + ")");

                while (!error)
                {
                    WaitForInput();
                    Console.WriteLine("Press Enter to exit server or enter 'abort' to cancel.");
                    if (Console.ReadLine() == "abort")
                    {
                        Console.WriteLine("Aborted exit.");
                    }
                    else
                    {
                        error = true;
                    }
                }
                _serverThread.Abort();
                _serverThread.Join();
                _server.Shutdown("Server shutdown.");
                Log("Server shutdown complete!", ConsoleColor.Yellow);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
        }
        #endregion

    }

}

