using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Gruppe22.Backend;

namespace Gruppe22.Client
{


    public class NetPlayer
    {

        private NetPeerConfiguration _config;
        private NetClient _client;
        private Dictionary<short, ClientData> _clients;
        private NetConnectionStatus _lastStatus;
        private Backend.IHandleEvent _parent;
        private string _server;
        private string _playername;
        public Dictionary<string, string> _servers;
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
                outmsg.Write((byte)PacketType.Connect);
                outmsg.Write(_playername);

                _client.Connect(_server, 666, outmsg);

            }
        }


        public string playername
        {
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
                System.Net.IPAddress mask;
                _parent.HandleEvent(false, Backend.Events.ShowMessage, "Client started on " + _client.Port + " @ " + NetUtility.GetMyAddress(out mask).ToString());
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
                        if (!_servers.ContainsKey(message.SenderEndpoint.Address.ToString()))
                        {
                            string text = message.ReadString();
                            _parent.HandleEvent(false, Backend.Events.ShowMessage, "Found server '" + text + "' at IP " + message.SenderEndpoint.Address.ToString());
                            _servers.Add(message.SenderEndpoint.Address.ToString(), text);
                        }
                        // _client.Connect(message.SenderEndpoint);
                        break;
                    case NetIncomingMessageType.Data:
                        //Bei ankommenden Daten, diese verarbeiten.
                        byte type = message.ReadByte();
                        ProcessMessage(type, message);
                        break;
                }
            }
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
                message.Write((byte)PacketType.Move);
                message.Write(CharID);

                message.Write(pos.ToString());
                message.Write((int)dir);

                _client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            }
        }



        private void ProcessMessage(byte type, NetIncomingMessage message)
        {
            switch ((PacketType)type)
            {
                case PacketType.Connect: //Client connected
                    short addClientId = message.ReadInt16();
                    if (!_clients.ContainsKey(addClientId))
                        _clients.Add(addClientId, null);
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Connected to " + message.SenderEndpoint.Address.ToString() + " as " +_playername+" (ID "+ addClientId.ToString()+")", Color.Orange);

                    break;
                case PacketType.Disconnect: //Client disconnect
                    short disconnectingClientID = message.ReadInt16();
                    if (_clients.ContainsKey(disconnectingClientID))
                        _clients.Remove(disconnectingClientID);
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Client " + disconnectingClientID + " has disconnected!", Color.Orange);
                    break;
                case PacketType.Chat:
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, message.ReadString(), Color.Pink);
                    break;
                case PacketType.UpdateClients:
                    break;
            }
        }

        public NetPlayer(Backend.IHandleEvent parent)
        {
            _config = new NetPeerConfiguration("DungeonCrawler");
            _config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            _parent = parent;
            _servers = new Dictionary<string, string>();
            _clients = new Dictionary<short, ClientData>();
            Start();
        }




    }
}
