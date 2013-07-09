using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gruppe22.Client;
using Microsoft.Xna.Framework;
using Lidgren;
using Lidgren.Network;

namespace Gruppe22.Backend
{
    public class NetLogic : Logic, IHandleEvent
    {
        private bool _ready = false;
        protected NetPlayer _network = null;


        public override void Update(GameTime gameTime)
        {

            if (_network != null)
            {
                _network.Update(gameTime);
            }

        }

        public void RequestMap()
        {
            _network.SendMessage(PacketType.UpdateMap);
        }

        public virtual void SendChat(string text)
        {
            //            _network.SendMessage();
            _parent.HandleEvent(false, Events.ShowMessage, text, Color.Pink);
        }

        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (DownStream) // Received from Frontend
            {
                switch (eventID)
                {
                    case Events.ChangeMap:
                        _map.FromXML((string)data[0]);
                        _ready = true;
                        _parent.HandleEvent(true, Events.ChangeMap, (int)data[1]);
                        break;

                    case Events.MoveActor:
                        _network.SendMessage(PacketType.Move, (int)data[0], (Coords)data[1]);
                        break;

                    case Events.ContinueGame:
                        _network.SendMessage(PacketType.Pause, false);
                        break;

                }
            }
            else // Received from Network
            {

                switch (eventID)
                {
                    case Events.MoveActor:
                        _parent.HandleEvent(true, Events.MoveActor, (int)data[0], (int)data[1]);
                        break;
                }
            };
        }


        public NetLogic(IHandleEvent parent, NetPlayer network)
            : base(parent, null, null)
        {
            _network = network;
            _map = new Map();
            RequestMap();
        }
    }
}
