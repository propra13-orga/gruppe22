using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gruppe22.Client;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{
    public class NetLogic : Logic
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
        public NetLogic(IHandleEvent parent, NetPlayer network, Map map = null, Random _random = null)
            : base(parent, map, _random)
        {
            _network = network;
        }
    }
}
