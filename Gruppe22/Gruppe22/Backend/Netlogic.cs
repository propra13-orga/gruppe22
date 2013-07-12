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
    /// <summary>
    /// All Game Logic events are handled server side -> send / receive events via network instead of handling them locally
    /// </summary>
    public class NetLogic : Logic, IHandleEvent
    {
        #region Private Fields
        /// <summary>
        /// An object representing the network connection
        /// </summary>
        protected NetPlayer _network = null;
        #endregion

        #region Public Fields
        /// <summary>
        /// Get the Network Connection (read only, NetPlayer class)
        /// </summary>
        public NetPlayer network
        {
            get
            {
                return _network;
            }
        }
        #endregion

        /// <summary>
        /// Exchange events with server
        /// </summary>
        /// <param name="gameTime">Current elapsed time</param>
        public override void Update(GameTime gameTime)
        {

            if (_network != null)
            {
                _network.Update(gameTime);
            }

        }

        /// <summary>
        /// Update local version of map
        /// </summary>
        public void RequestMap()
        {
            _network.SendMessage(PacketType.UpdateMap);
        }

        /// <summary>
        /// Exchange text messages
        /// </summary>
        /// <param name="text">Text to send to other users</param>
        public virtual void SendChat(string text)
        {
            //            _network.SendMessage();
            _network.SendMessage(PacketType.Chat, text);
        }

        /// <summary>
        /// React to incoming events
        /// </summary>
        /// <param name="DownStream"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (DownStream) // Received from Frontend
            {
                switch (eventID)
                {
                    case Events.ChangeMap:
                        _map.FromXML((string)data[0]);
                        _parent.HandleEvent(true, Events.ChangeMap, (int)data[1]);
                        break;
                    case Events.FinishedAnimation:
                        _network.SendMessage(PacketType.FinishedAnim, (int)data[0], (int)(Activity)data[1]);
                        _map.actors[(int)data[0]].locked = false;
                        break;

                    case Events.TileEntered:
                        _network.SendMessage(PacketType.FinishedMove, (int)data[0], (int)(Direction)data[1]);
                        _map.actors[(int)data[0]].locked = false;
                        break;
                    case Events.MoveActor:
                        if (!_map.actors[(int)data[0]].locked)
                        {
                            _network.SendMessage(PacketType.Move, (int)data[0], (int)(Direction)data[1]);
                        }
                        break;
                    case Events.Chat:
                        _network.SendMessage(PacketType.Chat, (string)data[0]);
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

                    case Events.ContinueGame:
                        _parent.HandleEvent(true, Backend.Events.ContinueGame, true);
                        break;

                    case Events.Chat:
                        _parent.HandleEvent(false, Backend.Events.ShowMessage, data);
                        break;
                    case Events.ShowMessage:
                        _parent.HandleEvent(false, Backend.Events.ShowMessage, data);
                        break;
                    case Events.MoveActor:
                        _map.actors[(int)data[0]].direction = (Direction)data[2];
                        if (data.Length > 4)
                        {
                            _map.PositionActor(_map.actors[(int)data[0]], new Coords((int)data[3], (int)data[4]));
                        }
                        _parent.HandleEvent(false, Events.MoveActor, data);
                        break;
                }

            };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">A local event handler to send messages to</param>
        /// <param name="network">The network connection to use for handling events</param>
        public NetLogic(IHandleEvent parent, NetPlayer network)
            : base(parent, null, null)
        {
            _network = network;
            network.parent = this;
            _map = new Map();
        }
    }
}
