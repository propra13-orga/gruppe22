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
        public bool _paused = true;
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
            List<NetConnection> Recipients = new List<NetConnection>();
            foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
            {
                if (client.Key != exclude)
                    Recipients.Add(client.Value.connection);
            }
            if (Recipients.Count > 0)
                _server.SendMessage(response, Recipients, method, 0);


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
                if (_paused)
                {
                    if (_pausedCount == 0)
                    {
                        _logic.paused = false;
                        _lastUpdate = DateTime.Now;
                        _paused = false;
                        foreach (Actor a in _logic.map.actors)
                        {
                            a.locked = false;
                        }
                    }
                }
                else
                {
                    if (_pausedCount != 0)
                    {
                        _paused = true;
                        _logic.paused = true;

                    }
                    else
                    {
                        Update();
                    }
                }
                if ((message = _server.ReadMessage()) == null)
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
                        {
                            bool allow = true;

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
                                    {
                                        message.SenderConnection.Deny("The GUID is already in use.");
                                        Log(message.SenderEndpoint + " denied (GUID in use).");

                                        allow = false;
                                    }
                                    break;
                                }
                            }
                            if (allow)
                            {
                                message.SenderConnection.Approve();

                                ClientData newClient = new ClientData(message.SenderConnection, 0, guid);
                                if (_pausedCount < 0)
                                {
                                    _pausedCount = 1;
                                }
                                else
                                {
                                    _pausedCount += 1;
                                }
                                newClient.actorID = _logic.map.AssignPlayer(guid);
                                _clients.Add(message.SenderEndpoint, newClient);
                                SendMessageToAll(PacketType.AddActor, NetDeliveryMethod.ReliableOrdered, message.SenderEndpoint, newClient.actorID, _logic.map.actors[newClient.actorID].tile.coords.x, _logic.map.actors[newClient.actorID].tile.coords.y, _logic.map.actors[newClient.actorID].ToXML());

                                Log(message.SenderEndpoint + " approved.");
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        byte type = message.ReadByte();
                        ProcessMessage(type, message);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            Log("Status Changed", ConsoleColor.Magenta);
                            NetConnectionStatus state = (NetConnectionStatus)message.ReadByte();
                            switch (state)
                            {
                                case NetConnectionStatus.Disconnected:
                                case NetConnectionStatus.Disconnecting:
                                    if (!_clients.ContainsKey(message.SenderEndpoint))
                                        break;

                                    if (_clients[message.SenderEndpoint].paused)
                                    {
                                        _pausedCount -= 1;
                                    }
                                    if (_clients.Count == 1)
                                    {
                                        _pausedCount = -1;
                                        ResetActors();
                                    }
                                    _logic.map.actors[_clients[message.SenderEndpoint].actorID].online = false;
                                    SendMessageToAll(PacketType.DisableActor, NetDeliveryMethod.ReliableOrdered, message.SenderEndpoint, _clients[message.SenderEndpoint].actorID);

                                    _clients.Remove(message.SenderEndpoint);
                                    Log(message.SenderEndpoint + " disconnected!");
                                    break;
                                case NetConnectionStatus.Connected:
                                    if (_clients.ContainsKey(message.SenderEndpoint))
                                    {
                                        SendMessage(PacketType.Connect, message.SenderConnection); // Send connect confirmation to client
                                        Log("Sent Connect to " + _clients[message.SenderEndpoint].guid, ConsoleColor.Cyan);
                                    }
                                    break;
                            }
                            break;
                        }
                        break;
                    default:
                        Log("Unhandled Messagetype: " + message.MessageType, ConsoleColor.Red);
                        break;
                }
                _server.Recycle(message);
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
                            Log("Client " + _clients[message.SenderEndpoint].guid + " needlessly asked to pause game " + _pausedCount.ToString() + " request remaining", ConsoleColor.Green);

                    }
                    else
                    {
                        if (state)
                        {
                            _pausedCount += 1;
                            Log("Client " + _clients[message.SenderEndpoint].guid + " paused game - " + _pausedCount.ToString() + " requests total", ConsoleColor.Red);
                            _clients[message.SenderEndpoint].paused = true;

                        }
                        else
                            Log("Client " + _clients[message.SenderEndpoint].guid + " needlessly asked to unpause game " + _clients[message.SenderEndpoint].paused + ": " + _pausedCount.ToString() + " request remaining", ConsoleColor.Green);

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

                case PacketType.Move:
                    int actorID = message.ReadInt32();
                    if (actorID < _logic.map.actors.Count)
                    {
                        Direction dir = (Direction)message.ReadInt32();
                        _logic.map.actors[actorID].moveIndex = message.ReadInt32();
                        _logic.map.PositionActor(_logic.map.actors[actorID], new Coords(message.ReadInt32(), message.ReadInt32()));
                        _logic.map.actors[actorID].locked = false;
                        _logic.HandleEvent(true, Events.MoveActor, actorID, dir, _logic.map.actors[actorID].moveIndex, _logic.map.actors[actorID].tile.coords);
                    }
                    break;
                case PacketType.FinishedMove:
                    {
                        int actor = message.ReadInt32();
                        if (actor < _logic.map.actors.Count)
                        {
                            Direction direction = (Direction)message.ReadInt32();
                            if (message.ReadInt32() == _logic.map.actors[actor].moveIndex)
                            {
                                _logic.HandleEvent(true, Events.TileEntered, actor, direction);
                            }
                        }
                    }
                    break;
                case PacketType.FinishedAnim:
                    {
                        int actor = message.ReadInt32();
                        if (actor < _logic.map.actors.Count)
                        {
                            Activity activity = (Activity)message.ReadInt32();
                            if (message.ReadInt32() == _logic.map.actors[actor].moveIndex)
                            {
                                _logic.HandleEvent(true, Events.FinishedAnimation, actor, activity);
                                _logic.map.actors[actor].locked = false;
                            }
                        }
                    }
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
                /*    while (!Console.KeyAvailable)
                   {
                        Console.CursorLeft = 0;
                         Console.CursorTop = 4;
                         Console.CursorVisible = false;
                         Console.WriteLine(_logic.map.ToString());
                
                   } */
                string input = Console.ReadLine();
                Console.CursorVisible = true;
                switch (input)
                {
                    case "exit":
                        return;
                    case "clear":
                        Console.Clear();
                        break;
                    case "actors":
                        for (int i = 0; i < _logic.map.actors.Count; ++i)
                        {
                            Console.WriteLine(i.ToString() + " " + _logic.map.actors[i].ToString() + " " + _logic.map.actors[i].id + ": " + _logic.map.actors[i].online);
                        }
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
                        foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
                        {
                            client.Value.actorID = _logic.map.AssignPlayer(client.Value.guid);
                            _logic.map.actors[client.Value.actorID].online = true;
                            SendMessage(PacketType.UpdateMap, client.Value.connection, _logic.map.ToXML(), client.Value.actorID);
                        }
                        Console.WriteLine("New Maps created.");
                        // TODO: Karte laden!

                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }
        #endregion

        #region Event Handling (incoming / outgoing commands)

        public void ResetActors()
        {
            foreach (Actor actor in _logic.map.actors)
            {
                actor.locked = false;
                actor.moveIndex = 0;
            }
        }
        public void HandleEvent(bool incoming, Events eventID, params object[] data)
        {
            if (!incoming)
            {
                // pass along to client
                switch (eventID)
                {
                    case Events.TrapActivate:
                        SendMessageToAll(PacketType.Trap, NetDeliveryMethod.ReliableOrdered, null, ((Coords)data[0]).x, ((Coords)data[0]).y, (int)data[1]);
                        break;
                    case Events.RejectMove:
                        SendMessageToAll(PacketType.Move, NetDeliveryMethod.ReliableOrdered, null, (int)data[0], (int)((Direction)data[1]), _logic.map.actors[(int)data[0]].moveIndex, ((Coords)data[3]).x, ((Coords)data[3]).y, ((Coords)data[4]).x, ((Coords)data[4]).y);
                        break;
                    case Events.MoveActor:
                        _logic.map.actors[(int)data[0]].moveIndex += 1;
                        //      if ((int)data[0] == 1)
                        SendMessageToAll(PacketType.Move, NetDeliveryMethod.ReliableOrdered, null, (int)data[0], (int)((Direction)data[1]), _logic.map.actors[(int)data[0]].moveIndex, ((Coords)data[3]).x, ((Coords)data[3]).y, ((Coords)data[4]).x, ((Coords)data[4]).y);
                        break;
                    case Events.Disconnect:
                        HandleEvent(true, Events.Network);
                        break;
                    case Events.Attack:
                        _logic.map.actors[(int)data[0]].moveIndex += 1;
                        SendMessageToAll(PacketType.Animate, NetDeliveryMethod.ReliableOrdered, null, (int)data[0], (int)Activity.Attack, _logic.map.actors[(int)data[0]].moveIndex, (int)_logic.map.actors[(int)data[0]].direction);
                        break;
                    case Events.ActorText:
                        SendMessageToAll(PacketType.ActorText, NetDeliveryMethod.ReliableOrdered, null, ((int)data[0]),
                            ((Coords)data[1]).x, ((Coords)data[1]).y, ((string)data[2]));

                        // defender, _map.actors[defender].tile.coords, "Evade")
                        break;
                    case Events.DamageActor:
                        _logic.map.actors[(int)data[0]].moveIndex += 1;
                        // , defender, _map.actors[defender].tile.coords, _map.actors[defender].health, damage);
                        SendMessageToAll(PacketType.DamageActor, NetDeliveryMethod.ReliableOrdered, null, (int)data[0], ((Coords)data[1]).x, ((Coords)data[1]).y, ((int)data[2]), ((int)data[3]), _logic.map.actors[(int)data[0]].moveIndex, (int)_logic.map.actors[(int)data[0]].direction);
                        break;
                    case Events.KillActor:
                        _logic.map.actors[(int)data[0]].moveIndex += 1;
                        SendMessageToAll(PacketType.KillActor, NetDeliveryMethod.ReliableOrdered, null, (int)data[0], ((Coords)data[1]).x, ((Coords)data[1]).y, ((int)data[3]), _logic.map.actors[(int)data[0]].moveIndex, (int)_logic.map.actors[(int)data[0]].direction);
                        break;
                    case Events.ChangeStats:
                        break;
                    case Events.FireProjectile:
                        break;
                    case Events.PlaySound:
                        SendMessageToAll(PacketType.PlaySound, NetDeliveryMethod.ReliableOrdered, null, (int)((SoundFX)data[0]));
                        break;
                    case Events.ActivateAbility:
                        /*   if ((int)data[0] == _playerID)
                           {
                               if ((int)data[1] < 0)
                               {
                                   int item = (int)data[1] + 1;

                               }
                               else
                               {
                                   if ((int)data[1] > 0)
                                   {
                                       int ability = (int)data[1] - 1;
                                   }
                               }
                           }*/
                        break;
                    case Events.Dialog:
                        //from, to, message, new Backend.DialogLine[] { new Backend.DialogLine("Goodbye", -1) }
                        foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
                        {
                            if ((client.Value.actorID == (int)(((Actor)data[0]).id)
                                || (client.Value.actorID == (int)(((Actor)data[1]).id))))
                            {
                                client.Value.paused = true;
                                _pausedCount += 1;
                            }
                        }


                        SendMessageToAll(PacketType.Dialog, NetDeliveryMethod.ReliableOrdered, null, (int)(((Actor)data[0]).id), (int)(((Actor)data[1]).id), (string)data[2]);
                        break;
                    case Events.Shop:
                        foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
                        {
                            if ((client.Value.actorID == (int)(((Actor)data[0]).id)
                                || (client.Value.actorID == (int)(((Actor)data[1]).id))))
                            {
                                client.Value.paused = true;
                                _pausedCount += 1;
                            }
                        }

                        SendMessageToAll(PacketType.Shop, NetDeliveryMethod.ReliableOrdered, null, (int)(((Actor)data[0]).id), (int)(((Actor)data[1]).id));
                        break;
                    case Events.SetItemTiles:
                        break;
                    case Events.Checkpoint:
                        break;
                    case Events.GameOver:
                        SendMessageToAll(PacketType.GameOver, NetDeliveryMethod.ReliableOrdered, null, null);
                        break;
                    case Events.FinishedAnimation:
                        break;
                    case Events.ShowMessage:
                        SendMessageToAll(PacketType.Chat, NetDeliveryMethod.ReliableOrdered, null, (string)(data[0]));
                        break;


                    case Events.ChangeMap:
                        _logic.ChangeMap((string)data[0], (Coords)data[1]);
                        break;
                }
            }
            else
            {// pass along to client
                switch (eventID)
                {
                    case Events.ChangeMap:
                        // Todo: Remap actors / clients
                        foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
                        {
                            client.Value.paused = true;
                            _pausedCount += 1;
                            client.Value.actorID = _logic.map.AssignPlayer(client.Value.guid);
                            _logic.map.actors[client.Value.actorID].online = true;
                        }
                        foreach (KeyValuePair<IPEndPoint, ClientData> client in _clients)
                        {
                            SendMessage(PacketType.UpdateMap, client.Value.connection, _logic.map.ToXML(), client.Value.actorID);
                        }

                        break;
                }
            }

        }

        #endregion
        #region Constructor
        public Server(string[] args)
        {
            Log("Setting up server ..");
            _random = new Random(Guid.NewGuid().GetHashCode());
            _logic = new PureLogic(this, null, _random);
            foreach (Actor a in _logic.map.actors)
            {
                a.online = false;
            }
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
                _config.PingInterval = 0.25f;
                _config.ConnectionTimeout = 20f;

                _config.MaximumHandshakeAttempts = 3;
                _config.ResendHandshakeInterval = 5;
                _config.UseMessageRecycling = true;
                _config.EnableUPnP = true;
                _config.AcceptIncomingConnections = false;


                _server = new NetServer(_config);

                _server.Start();
                _server.UPnP.ForwardPort(666, "Dungeon Crawler");

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

                if (passed.Milliseconds > 200)
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


        }
        #endregion

    }

}

