using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Gruppe22.Client
{

    public enum PacketTypes
    {
        LOGIN,
        MOVE,
        UPDATEMAP,
        REDUCEHEALTH,
        ADDITEM,
        REMOVEITEM,
        TOGGLETILE
    }

    public class NetPlayer
    {

        private NetPeerConfiguration _config;
        private NetClient _client;
        private Dictionary<short, ClientData> _clients;
        private NetConnectionStatus _lastStatus;
        private Backend.IHandleEvent _parent;
        private string _server;
        private string _playername;

        public string server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
                NetOutgoingMessage outmsg = _client.CreateMessage();
                outmsg.Write((byte)PacketTypes.LOGIN);
                outmsg.Write(_playername);

                            _client.Connect(_server, 666,outmsg);

            }
        }


        public string playername{
            get
            {
                return _playername;
            }
            set
            {
                _playername = value;
            }
    }
        public void Start()
        {
            if (_client == null)
            {
                _parent.HandleEvent(false, Backend.Events.ShowMessage, "Starting client...");
                _client = new NetClient(_config);
                _client.Start();
                _parent.HandleEvent(false, Backend.Events.ShowMessage, "Client started on " + _client.Port + " @ " + _client.Socket.LocalEndPoint);
            }
            else
            {
                _parent.HandleEvent(false, Backend.Events.ShowMessage, "Already running - stopping first");
                Stop();
                Start();
            }
        }

        public void Stop()
        {
            if (_client != null)
            {
                _client.Shutdown("Goodbye!");
                _client = null;
            }
        }

        public void Update(GameTime gameTime)
        {

            if (_client != null)
            {
                switch (_client.ConnectionStatus)
                {
                    case NetConnectionStatus.None:
                        break;
                    case NetConnectionStatus.InitiatedConnect:
                        break;
                    case NetConnectionStatus.RespondedConnect:
                        break;
                    case NetConnectionStatus.Connected:
                        NetIncomingMessage message;
                        while ((message = _client.ReadMessage()) != null)
                        {
                            switch (message.MessageType)
                            {
                                case NetIncomingMessageType.WarningMessage:
                                    _parent.HandleEvent(false, Backend.Events.ShowMessage, message.ReadString(), Color.Orange);
                                    break;
                                case NetIncomingMessageType.ErrorMessage:
                                    _parent.HandleEvent(false, Backend.Events.ShowMessage, message.ReadString(), Color.Red);
                                    break;
                                case NetIncomingMessageType.DiscoveryResponse:
                                    //Server hat seine Existenz bestätigt

                                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Found server '" + message.ReadString() + "'!");
                                    _client.Connect(message.SenderEndpoint);
                                    break;
                                case NetIncomingMessageType.Data:
                                    //Bei ankommenden Daten, diese verarbeiten.
                                    byte type = message.ReadByte();
                                    ProcessMessage(type, message);
                                    break;
                            }
                        }
                        break;
                    case NetConnectionStatus.Disconnecting:
                        _clients.Clear();
                        break;
                    case NetConnectionStatus.Disconnected:
                        if (_lastStatus == NetConnectionStatus.None)
                        {
                            _parent.HandleEvent(false, Backend.Events.ShowMessage, "Connection lost.", Color.Red);
                            _parent.HandleEvent(false, Backend.Events.Pause, true);
                        }
                        _client.DiscoverLocalPeers(666);
                        break;
                }
                _lastStatus = _client.ConnectionStatus;

            }


        }


        public void DoMove(int CharID, Backend.Direction dir, Backend.Coords pos)
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage message = _client.CreateMessage();
                //Eine Message senden, dass die Position geändert wurde
                message.Write((byte)0x0);
                message.Write(CharID);

                message.Write(pos.ToString());
                message.Write((int)dir);

                _client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            }
        }



        private void ProcessMessage(byte type, NetIncomingMessage message)
        {
            switch (type)
            {
                case 0x00: //Client connected
                    short addClientId = message.ReadInt16();
                    if (!_clients.ContainsKey(addClientId))
                        _clients.Add(addClientId, null);
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Client " + addClientId + " connected!", 3f, Color.Orange);

                    break;
                case 0x01: //Client disconnect
                    short disconnectingClientID = message.ReadInt16();
                    if (_clients.ContainsKey(disconnectingClientID))
                        _clients.Remove(disconnectingClientID);
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Client " + disconnectingClientID + " has disconnected!", 3f, Color.Orange);
                    break;
                case 0x02: //Clientinformation: ID und eventuelle Abfrage aller weiteren Clients
                    //_self = new ClientData(message.ReadInt16()) { Position = _self.Position, LastPosition = _self.LastPosition };
                    short playersOnline = message.ReadInt16();
                    /*_output.ShowMessage("Connected with ID '" + _self.ID + "'. Currently " + playersOnline +
                                        " Players online!");
                    */
                    if (playersOnline > 1)
                    {
                        NetOutgoingMessage getListMessage = _client.CreateMessage();
                        getListMessage.Write((byte)0x1);
                        _client.SendMessage(getListMessage, NetDeliveryMethod.ReliableUnordered);
                    }
                    break;
                case 0x10: //Positionsänderung
                    short changerID = message.ReadInt16();
                    /* if (_clients.ContainsKey(changerID))
                     {
                         _clients[changerID].LastPosition = _clients[changerID].Position;
                         _clients[changerID].Position = message.ReadVector2();
                         _clients[changerID].Direction = message.ReadVector2();
                         _clients[changerID].Speed = message.ReadFloat();
                     } */
                    break;
                case 0x11: //Client-Liste vom Server
                    short id = message.ReadInt16();
                    if (_clients.ContainsKey(id))
                        break;

                    ClientData newClient = new ClientData(id);
                    /*
                    newClient.Position = message.ReadVector2();
                    newClient.Direction = message.ReadVector2();
                    newClient.Speed = message.ReadFloat(); */
                    _clients.Add(id, newClient);
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Retrieved client '" + id + "'");
                    break;
            }
        }

        public NetPlayer(Backend.IHandleEvent parent)
        {
            //Netzwerk vorbereiten und configurieren
            _config = new NetPeerConfiguration("DungeonCrawler");
            _config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            _parent = parent;
            _clients = new Dictionary<short, ClientData>();
            Start();
        }




    }
}
