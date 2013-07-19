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
                        _map.ClearActors();
                        _map.FromXML((string)data[0], null, true);
                        _parent.HandleEvent(true, Events.ChangeMap, (int)data[1]);
                        break;
                    case Events.FinishedAnimation:
                        _network.SendMessage(PacketType.FinishedAnim, (int)data[0], (int)(Activity)data[1], (int)_map.actors[(int)data[0]].moveIndex);
                        _map.actors[(int)data[0]].locked = false;
                        break;

                    case Events.TileEntered:
                        _network.SendMessage(PacketType.FinishedMove, (int)data[0], (int)(Direction)data[1], (int)_map.actors[(int)data[0]].moveIndex);
                        _map.actors[(int)data[0]].locked = false;
                        break;
                    case Events.MoveActor:
                        if (!_map.actors[(int)data[0]].locked)
                        {
                            _map.actors[(int)data[0]].locked = true;
                            _network.SendMessage(PacketType.Move,
                                (int)data[0], (int)(Direction)data[1],
                                (int)data[2],
                                (int)_map.actors[(int)data[0]].tile.coords.x,
                                (int)_map.actors[(int)data[0]].tile.coords.y
                                );
                        }
                        break;
                    case Events.Chat:
                        _network.SendMessage(PacketType.Chat, (string)data[0]);
                        break;
                    case Events.ContinueGame:
                        _network.SendMessage(PacketType.Pause, false);
                        break;
                    case Events.Pause:
                        _network.SendMessage(PacketType.Pause, true);
                        break;

                }
            }
            else // Received from Network
            {

                switch (eventID)
                {
                    case Events.TrapActivate:
                        _map[(Coords)data[0]].trap.status = (TrapState)data[1];
                        break;
                    case Events.Disconnect:
                        _parent.HandleEvent(false, eventID, data);
                        break;
                    case Events.RemovePlayer:
                        _map.actors[(int)data[0]].online = false;
                        _parent.HandleEvent(false, Events.AddPlayer);
                        break;
                    case Events.AddPlayer:
                        Actor actor = (Actor)data[2];
                        actor.id = (int)data[0];
                        ActorTile actortile = new ActorTile(_map[(Coords)data[1]], actor);
                        actor.tile = actortile;
                        actortile.enabled = (actor.health > 0);
                        actortile.parent = _map[(Coords)data[1]];
                        _map[(Coords)data[1]].Add(actortile);
                        _map.updateTiles.Add((Coords)data[1]);


                        if (_map.actors.Count <= (int)data[0])
                        {
                            _map.actors.Add(actor);
                        }
                        else
                        {
                            if (_map.actors[(int)data[0]].tile != null)
                            {
                                Coords source = ((FloorTile)_map.actors[(int)data[0]].tile.parent).coords;
                                ((FloorTile)_map.actors[(int)data[0]].tile.parent).Remove(_map.actors[(int)data[0]].tile);
                                // Remove old tile from updatelist (if no other actor or trap)
                                if (!((_map[source].hasEnemy)
                    || (_map[source].hasPlayer)
                    || (_map[source].hasTrap)))
                                    _map.updateTiles.Remove(source);
                            }
                            _map.actors[(int)data[0]] = actor;
                        }
                        _map.actors[(int)data[0]].online = true;
                        _parent.HandleEvent(false, Events.AddPlayer);
                        break;
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
                        if ((int)data[0] < _map.actors.Count)
                        {
                            _map.actors[(int)data[0]].moveIndex = (int)data[2];
                            _map.actors[(int)data[0]].direction = (Direction)data[1];

                            _map.PositionActor(_map.actors[(int)data[0]], (Coords)data[3]);
                            if ((Coords)data[3] != (Coords)data[4])
                            {
                                _parent.HandleEvent(false, Events.MoveActor, data);
                            }
                            else
                            {
                                _map.actors[(int)data[0]].locked = false;
                            }
                        }
                        break;




                    case Events.AnimateActor:
                        if ((int)data[0] < _map.actors.Count)
                        {
                            _map.actors[(int)data[0]].locked = true;
                            _map.actors[(int)data[0]].moveIndex = (int)data[2];
                            _map.actors[(int)data[0]].direction = (Direction)data[3];
                            _parent.HandleEvent(false, Backend.Events.AnimateActor, (int)data[0], (Activity)data[1]);
                        }
                        break;
                    case Events.ActorText:
                        _parent.HandleEvent(false, Backend.Events.ActorText, (int)data[0], data[1], data[2]);


                        // defender, _map.actors[defender].tile.coords, "Evade")
                        break;
                    case Events.DamageActor:
                        if ((int)data[0] < _map.actors.Count)
                        {
                            _map.actors[(int)data[0]].locked = false;
                            _map.actors[(int)data[0]].health = (int)data[2];
                            _map.actors[(int)data[0]].moveIndex = (int)data[4];
                            // , defender, _map.actors[defender].tile.coords, _map.actors[defender].health, damage);

                            _map.actors[(int)data[0]].direction = (Direction)data[5];
                        }
                        _parent.HandleEvent(false, Backend.Events.DamageActor, data);
                        break;
                    case Events.KillActor:
                        _map.actors[(int)data[0]].locked = false;
                        _map.actors[(int)data[0]].moveIndex = (int)data[3];
                        _map.actors[(int)data[0]].health = 0;
                        _map.actors[(int)data[0]].direction = (Direction)data[4];
                        _parent.HandleEvent(false, Backend.Events.KillActor, data);
                        break;
                    case Events.PlaySound:
                        _parent.HandleEvent(false, Backend.Events.PlaySound, data);
                        break;
                    case Events.Dialog:
                        //from, to, message, new Backend.DialogLine[] { new Backend.DialogLine("Goodbye", -1) }
                        _parent.HandleEvent(false, Backend.Events.Dialog, _map.actors[(int)data[0]], _map.actors[(int)data[1]], data[2]);
                        break;
                    case Events.Shop:
                        if (((int)data[0] < _map.actors.Count) && ((int)data[1] < _map.actors.Count))
                            _parent.HandleEvent(false, Backend.Events.Shop, _map.actors[(int)data[0]], _map.actors[(int)data[1]]);
                        break;
                    case Events.GameOver:
                        _parent.HandleEvent(false, Backend.Events.GameOver, data);
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
