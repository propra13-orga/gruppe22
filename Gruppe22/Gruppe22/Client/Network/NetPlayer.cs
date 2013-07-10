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
        private NetConnectionStatus _lastStatus;
        private Backend.IHandleEvent _parent;
        private string _server = "";
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
                if (_server != "")
                {
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Disconnecting from Server " + _server + "...");
                    _client.Disconnect("Goodbye!");
                    _server = "";
                }
                if (value != "")
                {
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, "Connecting to Server " + value + "...");
                    _server = value;
                    NetOutgoingMessage outmsg = _client.CreateMessage();
                    // outmsg.Write((byte)PacketType.Connect);
                    outmsg.Write(_playername);
                    _client.Connect(_server, 666, outmsg);
                }
            }
        }

        public IHandleEvent parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public bool connected
        {
            get
            {
                return _client.ConnectionStatus == NetConnectionStatus.Connected;
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

        public void Disconnect()
        {
            server = "";
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
            if ((_client != null) && (_lastStatus != _client.ConnectionStatus))
            {
                switch (_client.ConnectionStatus)
                {
                    case NetConnectionStatus.None:
                        break;
                    case NetConnectionStatus.InitiatedConnect:
                        _parent.HandleEvent(false, Backend.Events.ShowMessage, "Asked for connection.", Color.Green);

                        break;
                    case NetConnectionStatus.RespondedConnect:
                        _parent.HandleEvent(false, Backend.Events.ShowMessage, "Server responded to Connect Request.", Color.Green);

                        break;
                    case NetConnectionStatus.Connected:
                        _parent.HandleEvent(false, Backend.Events.ShowMessage, "Connected.", Color.Green);
                        break;
                    case NetConnectionStatus.Disconnected:
                        _parent.HandleEvent(false, Backend.Events.ShowMessage, "Disconnected.", Color.Red);
                        _client.DiscoverLocalPeers(666);
                        break;
                }
                _lastStatus = _client.ConnectionStatus;

            }


        }

        public void SendMessage(PacketType type, params object[] data)
        {
            NetOutgoingMessage response;
            response = _client.CreateMessage();
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
            _client.SendMessage(response, NetDeliveryMethod.ReliableOrdered);
        }


        private void ProcessMessage(byte type, NetIncomingMessage message)
        {
            PacketType x = (PacketType)type;
            switch (x)
            {
                case PacketType.UpdateMap: //Client connected
                    string map = message.ReadString();
                    int addClientId = message.ReadInt16();
                    _parent.HandleEvent(true, Backend.Events.ChangeMap, map, addClientId);
                    System.Diagnostics.Debug.WriteLine(map);
                    break;
                case PacketType.Chat:
                    _parent.HandleEvent(false, Backend.Events.ShowMessage, message.ReadString(), Color.Pink);
                    break;
                case PacketType.UpdateClients:
                    break;
                case PacketType.Move:
                    _parent.HandleEvent(false, Backend.Events.MoveActor, message.ReadInt32(), new Coords(message.ReadInt32(), message.ReadInt32()),(Direction)message.ReadInt32());
                    break;
            }
        }

        public NetPlayer(Backend.IHandleEvent parent)
        {
            _config = new NetPeerConfiguration("DungeonCrawler");
            _config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            _parent = parent;
            _servers = new Dictionary<string, string>();
            Start();
        }




    }
}
