﻿using System;
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
    /// </summary>
    public class Server : IHandleEvent
    {
        #region Private Fields
        /// <summary>
        /// Configuration info used by Lidgren
        /// </summary>
        private NetPeerConfiguration _config;
        /// <summary>
        /// Server object provided by Lidgren
        /// </summary>
        private NetServer _server;
        private int _pausedCount = -1;
        private string _servername = "Crawler 2000";
        /// <summary>
        /// A List of CLients and their respective data
        /// </summary>
        private Dictionary<IPEndPoint, ClientData> _clients;
        private List<string> discoveredBy;
        /// <summary>
        /// A thread to separate user interface and network activity
        /// </summary>
        private Thread _serverThread;
        private PureLogic _logic;
        private Random _random;
        private bool _updating = true;
        private DateTime _lastUpdate = DateTime.Now;
        Microsoft.Xna.Framework.GameTime _gameTime;
        public static bool error = false;

        #endregion

        #region Private Methods

        /// <summary>
        /// Broadcast a message to all clients
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method"></param>
        /// <param name="impossibleClients"></param>
        public void SendMessageToAll(PacketType type, NetDeliveryMethod method, IPEndPoint exclude, params object[] data)
        {
            if (_clients.Count == 0)
                return;
            NetOutgoingMessage response = _server.CreateMessage();
            response.Write((byte)type);
            foreach (object element in data)
            {
                if (element is int) response.Write((int)element);
                else if (element is bool) response.Write((bool)element);
                else
                {
                    if (element is string) response.Write((string)element);
                    else
                        response.Write(element.ToString());
                }
            }

            foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
            {
                if (client.Key != exclude)
                    _server.SendMessage(response, client.Value.Connection, method);
            }
        }

        public void SendMessage(PacketType type, NetConnection target, params object[] data)
        {
            NetOutgoingMessage response = _server.CreateMessage();
            response.Write((byte)type);
            foreach (object element in data)
            {

                if (element is int) response.Write((int)element);
                else if (element is bool) response.Write((bool)element);
                else
                {
                    if (element is string) response.Write((string)element);
                    else
                        response.Write(element.ToString());
                }
            }
            _server.SendMessage(response, target, NetDeliveryMethod.ReliableUnordered);
        }

        /// <summary>
        /// Work on the message queue (reacting to all incoming messages)
        /// </summary>
        /// <param name="param"></param>
        private void WorkMessages(object param)
        {
            _updating = false;
            NetServer server = (NetServer)param;

            //Message-Cycle: Dauerhaft Messages verarbeiten, bis das Programm beendet wird bzw. ein Fehler auftritt
            NetIncomingMessage message;
            while (_serverThread.ThreadState != ThreadState.AbortRequested)
            {
                if (_pausedCount == 0)
                {
                    Update();
                    _logic.paused = false;
                }
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
                        server.SendDiscoveryResponse(server.CreateMessage(_servername), message.SenderEndpoint);
                        if (!discoveredBy.Contains(message.SenderEndpoint.Address.ToString()))
                        {
                            Log(message.SenderEndpoint + " has discovered the server!");
                            discoveredBy.Add(message.SenderEndpoint.Address.ToString());
                        }
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        message.SenderConnection.Approve();
                        Log(message.SenderEndpoint + " approved.");
                        break;
                    case NetIncomingMessageType.Data:
                        Log("Data received", ConsoleColor.Red);
                        byte type = message.ReadByte();
                        ProcessMessage(type, message);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Log("Status Changed", ConsoleColor.Magenta);
                        NetConnectionStatus state = (NetConnectionStatus)message.ReadByte();
                        switch (state)
                        {
                            case NetConnectionStatus.Disconnected:
                            case NetConnectionStatus.Disconnecting:
                                if (!_clients.ContainsKey(message.SenderEndpoint))
                                    break;
                                SendMessageToAll(PacketType.Disconnect, NetDeliveryMethod.ReliableUnordered, message.SenderEndpoint, _clients[message.SenderEndpoint].ID);
                                Log("Sent Disconnect to all except " + _clients[message.SenderEndpoint].ID, ConsoleColor.Cyan);

                                if (_clients[message.SenderEndpoint].paused)
                                {
                                    _pausedCount -= 1;
                                }
                                if (_clients.Count == 1)
                                {
                                    _pausedCount = -1;
                                }

                                _clients.Remove(message.SenderEndpoint);
                                Log(message.SenderEndpoint + " disconnected!");
                                break;
                            case NetConnectionStatus.Connected:
                                if (_clients.ContainsKey(message.SenderEndpoint)) break;
                                string guid = "";
                                if (message.SenderConnection.RemoteHailMessage != null)
                                {
                                    guid = message.SenderConnection.RemoteHailMessage.ReadString();
                                }
                                if (guid != "")
                                {
                                    for (int i = 0; i < _clients.Count; ++i)
                                    {
                                        if (_clients.ElementAt(i).Value.guid == guid)
                                            return;
                                    }
                                    ClientData newClient = new ClientData(message.SenderConnection, ClientData.GetFreeID(_clients), guid);
                                    if (_pausedCount < 0)
                                    {
                                        _pausedCount = 1;
                                    }
                                    else
                                    {
                                        _pausedCount += 1;
                                    }
                                    _clients.Add(message.SenderEndpoint, newClient);
                                    Log("Created client '" + guid + "' (ID:" + newClient.ID + ")!");
                                    newClient.actorID = _logic.map.AssignPlayer(guid);
                                    SendMessage(PacketType.Connect, message.SenderConnection, _logic.map.ToXML(), newClient.actorID); // Send map to client
                                    Log("Sent Connect to " + newClient.ID, ConsoleColor.Cyan);

                                    //Nun alle aktiven Clients informieren, dass ein neuer Client connected ist

                                    SendMessageToAll(PacketType.UpdateClients, NetDeliveryMethod.ReliableUnordered, message.SenderEndpoint, newClient.ID);
                                    Log("Sent UpdateClients to all", ConsoleColor.Cyan);

                                    Log(message.SenderEndpoint + " connected!");

                                }
                                break;
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
        private void ProcessMessage(byte id, NetIncomingMessage message)
        {
            switch ((PacketType)id)
            {
                case PacketType.Pause:
                    bool state = message.ReadBoolean();
                    if (_clients[message.SenderEndpoint].paused)
                    {
                        if (!state)
                        {
                            _pausedCount -= 1;
                            Log("Client " + _clients[message.SenderEndpoint].guid + " unpaused game " + _pausedCount.ToString() + " request remaining", ConsoleColor.Green);
                            _clients[message.SenderEndpoint].paused = false;
                        }
                        else
                            Log("Client " + _clients[message.SenderEndpoint].guid + " needlessly asked to unpause game " + _pausedCount.ToString() + " request remaining", ConsoleColor.Green);

                    }
                    else
                    {
                        if (state)
                        {
                            _pausedCount += 1;
                            _clients[message.SenderEndpoint].paused = true;
                            Log("Client " + _clients[message.SenderEndpoint].guid + " paused game - " + _pausedCount.ToString() + " requests total", ConsoleColor.Red);
                        }
                        else
                            Log("Client " + _clients[message.SenderEndpoint].guid + " needlessly asked to pause game " + _pausedCount.ToString() + " request remaining", ConsoleColor.Green);

                    }
                    break;
                case PacketType.UpdateMap: //Client connected
                    Log("Requested Map Update");
                    SendMessage(PacketType.UpdateMap, message.SenderConnection, _logic.map.ToXML(), _clients[message.SenderEndpoint].actorID);
                    break;
                case PacketType.Chat:
                    Log("Chat");
                    SendMessageToAll(PacketType.Chat, NetDeliveryMethod.Unreliable, null, _logic.map.actors[_clients[message.SenderEndpoint].actorID].name + ": " + message.ReadString());
                    break;

                case PacketType.Move: //0x0 steht für Positions-Informationen eines Clienten
                    int actorID = message.ReadInt32();
                    Direction dir = (Direction)message.ReadInt32();
                    _logic.HandleEvent(true, Events.MoveActor, actorID, dir, _logic.map.actors[actorID].tile.coords.x, _logic.map.actors[actorID].tile.coords.y);
                    break;
                case PacketType.FinishedMove:
                    _logic.HandleEvent(true, Events.TileEntered, message.ReadInt32(), (Direction)message.ReadInt32());

                    break;
                case PacketType.FinishedAnim:
                    _logic.HandleEvent(true, Events.FinishedAnimation, message.ReadInt32(), (Activity)message.ReadInt32());

                    break;
                default:
                    Log(((PacketType)id).ToString());
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
        private void WaitForInput()
        {
            Console.WriteLine("Enter exit to quit; clear to clear screen or stats for statistics.");
            while (true)
            {
                while (!Console.KeyAvailable)
                {
                    /*  Console.CursorLeft = 0;
                      Console.CursorTop = 4;
                      Console.CursorVisible = false;
                      Console.WriteLine(_logic.map.ToString());
                
                     */
                }
                string input = Console.ReadLine();
                Console.CursorVisible = true;
                switch (input)
                {
                    case "exit":
                        return;
                    case "clear":
                        Console.Clear();
                        break;
                    case "map":
                        Console.WriteLine("Displaying current map:");
                        Console.WriteLine(_logic.map.ToString());
                        Console.WriteLine("#: Wall - @: Player - X: Enemy - !: Trap - 0: NPC - >: Teleport - *: Treasure");
                        break;
                    case "stats":
                        Log(_server.Statistics.ToString(), ConsoleColor.Magenta);
                        break;
                    case "new":
                        _logic.GenerateMaps();
                        Console.WriteLine("New Maps created.");
                        // Karte laden!
                        // Karte an Spieler pushen!
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
                Console.ReadLine();
            }
        }
        #endregion

        #region Event Handling (incoming / outgoing commands)


        public void HandleEvent(bool incoming, Events eventID, params object[] data)
        {
            if (!incoming)
            {
                // pass along to client
                switch (eventID)
                {
                    case Events.MoveActor:
                        SendMessageToAll(PacketType.Move, NetDeliveryMethod.Unreliable, null, (int)data[0], ((Coords)data[1]).x, ((Coords)data[1]).y, (int)((Direction)data[2]));
                        break;

                }
            }

        }

        #endregion
        #region Constructor
        public Server(string[] args)
        {
            Log("Setting up server ...");
            _random = new Random(Guid.NewGuid().GetHashCode());
            _logic = new PureLogic(this, null, _random);
            _clients = new Dictionary<IPEndPoint, ClientData>();
            _gameTime = new Microsoft.Xna.Framework.GameTime();

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
                _config.AcceptIncomingConnections = false;

                _server = new NetServer(_config);
                _server.Start();
            }
            catch (Exception ex)
            {
                Log("Error during startup: " + ex.Message);
                error = true;
            }

            Console.CursorTop = 0;
            Console.CursorLeft = 0;
        }

        private void Update()
        {
            if (!_updating)
            {
                _updating = true;
                TimeSpan passed = DateTime.Now - _lastUpdate;

                if (passed.Milliseconds > 100)
                {
                    _lastUpdate = DateTime.Now;
                    _gameTime.ElapsedGameTime += passed;
                    _gameTime.TotalGameTime += passed;
                    _logic.Update(_gameTime);
                }
                _updating = false;
            }

        }

        public void Run()
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
        #endregion
        #region Main Method (public)

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Server tmp = new Server(args);

            if (!error)
            {
                tmp.Run();
            }


            Console.WriteLine("Press any key to exit.");
            Console.CursorVisible = false;
            Console.ReadLine();

        }
        #endregion

    }

}

