﻿using System;
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

        protected NetPlayer _network = null;

        public override void Update(GameTime gameTime)
        {

            if (_network != null)
            {
                _network.Update(gameTime);
            }

        }



        public virtual void SendChat(string text)
        {
            //            _network.SendMessage();
            _parent.HandleEvent(false, Events.ShowMessage, text, Color.Pink);
        }

        void IHandleEvent.HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (DownStream) // Received from Frontend
            {
                switch (eventID)
                {
                    case Events.MoveActor:
                        _network.SendMessage(PacketType.Move, (int)data[0], (int)data[1]);
                        break;

                }
            }
            else // Received from Network
            {

            };
        }


        public NetLogic(IHandleEvent parent, NetPlayer network)
            : base(parent, null, null)
        {
            _network = network;
        }
    }
}
