using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gruppe22
{
    /// <summary>
    /// Handle game logic
    /// </summary>
    public class GameLogic : GameWin
    {
        #region Overriden MonoGame default methods
        /// <summary>
        /// Set up the (non visible) objects of the game
        /// </summary>
        protected override void Initialize()
        {
            if (!System.IO.File.Exists("room1.xml"))
            {
                GenerateMaps();
            }
            string path = "room1.xml";
            if (File.Exists("GameData"))
                path = File.ReadAllText("GameData");
            if (File.Exists("saved" + (string)path))
                _map1 = new Map(Content, this, "saved" + (string)path);
            else
                _map1 = new Map(Content, this, (string)path);
            base.Initialize();
        }
        #endregion


        /// <summary>
        /// Handle events from UIElements and/or backend objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public override void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {

            switch (eventID)
            {
                case Events.ChangeMap: // Load another map
                    _status = GameStatus.NoRedraw; // prevent redraw (which would crash the game!)
                    _map1.Save("savedroom" + _map1.currRoomNbr + ".xml");
                    if (File.Exists("saved" + (string)data[0]))
                        _map1.Load("saved" + (string)data[0], (Coords)data[1]);
                    else
                        _map1.Load((string)data[0], (Coords)data[1]);
                    _mainmap1.resetActors();
                    _mainmap2.resetActors();

                    AddMessage("You entered room number " + data[0].ToString().Substring(4, 1) + ".");
                    File.WriteAllText("GameData", data[0].ToString());
                    _status = GameStatus.Running;
                    break;

                case Events.FinishedAnimation:
                    int FinishedID = (int)data[0];
                    Activity FinishedActivity = (Activity)data[1];
                    if (FinishedActivity == Activity.Die)
                    {
                        if (_map1.actors[FinishedID] is Enemy)
                        {
                            ((ActorTile)_map1.actors[FinishedID].tile).enabled = false;
                            AddMessage(_map1.actors[FinishedID].name + " is dead.");
                            ((ActorTile)_map1.actors[FinishedID].tile).DropItems();
                        }
                        else
                        {
                            AddMessage("<red>You are dead.");
                            RemoveHealth();
                            ShowEndGame();
                        }
                    }
                    break;

                case Events.TileEntered:
                    {
                        int id = (int)data[0];
                        Direction dir = (Direction)data[1];
                        Coords target = _map1.actors[id].tile.coords;

                        // Pickup any items
                        while (_map1[target.x, target.y].hasTreasure)
                        {
                            AddMessage(((id == 0) ? "You found " : _map1.actors[id].name + " found ") + _map1[target.x, target.y].firstItem.item.name + " .");
                            if (id == 0)
                                _mainmap1.floatNumber(target, "Found " + _map1[target.x, target.y].firstItem.item.name, Color.DarkGreen);
                            _map1[target.x, target.y].firstItem.item.Pickup(_map1.actors[id]);
                            _map1[target.x, target.y].Remove(_map1[target.x, target.y].firstItem);
                            _inventory.Update();
                        }
                        // Apply teleporter (move to next room)
                        if ((id == 0) && (_map1[target.x, target.y].hasTeleport))
                        {
                            HandleEvent(null, Events.ChangeMap, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextRoom, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextPlayerPos);

                        }

                        // Apply trap damage
                        if (_map1[target.x, target.y].hasTrap)
                        {
                            _map1.actors[id].SetDamage(_map1[target.x, target.y].trapDamage);
                            if (id == 0)
                                _mainmap1.floatNumber(target, _map1[target.x, target.y].trapDamage.ToString(), Color.DarkRed);
                            if (_map1.actors[id].isDead)
                            {
                                _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Die);
                                _mainmap2.HandleEvent(null, Events.AnimateActor, id, Activity.Die);

                                AddMessage((_map1.actors[id] is Player ? "You were" : _map1.actors[id].name + " was") + " killed by a trap  doing " + (_map1[target.x, target.y].trapDamage - _map1.actors[id].armor).ToString() + " points of damage (" + _map1[target.x, target.y].trapDamage.ToString() + " - " + _map1.actors[id].armor + " protection)");

                            }
                            else
                            {
                                _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Hit);
                                _mainmap2.HandleEvent(null, Events.AnimateActor, id, Activity.Hit);

                                AddMessage((_map1.actors[id] is Player ? "You were" : _map1.actors[id].name + "  was") + " hit for " + (_map1[target.x, target.y].trapDamage - _map1.actors[id].armor).ToString() + " points of damage (" + _map1[target.x, target.y].trapDamage.ToString() + " - " + _map1.actors[id].armor + " protection)");
                            }
                            if (_map1.actors[id] is Player) RemoveHealth();

                        }

                        // Trigger floor switches
                        if ((_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTarget) && (id == 0))
                        {
                            _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Talk);
                            _mainmap2.HandleEvent(null, Events.AnimateActor, id, Activity.Talk);

                            ShowEndGame("You have successfully found the hidden treasure. Can you do it again?", "Congratulations!");
                        }


                        _map1.actors[id].locked = false;

                    }
                    // Allow to choose next turn
                    break;


                case Events.Attack:
                    {

                        int id = (int)data[0];
                        Direction dir = (Direction)data[1];
                        if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                        {
                            Coords target = Map.DirectionTile(_map1.actors[id].tile.coords, dir);
                            // Display enemy statistics
                            if (_map1[target.x, target.y].firstActor is Player)
                            {
                                _enemyStats.actor = _map1.actors[id]; // Enemy attacked
                            }
                            else
                            {
                                if (id == 0)
                                {
                                    _enemyStats.actor = _map1[target.x, target.y].firstActor; // Player attacked enemy
                                }
                            }
                            // Aktuelle Figur attackiert
                            // Spieler verletzt
                            // oder tot
                            _map1[target.x, target.y].firstActor.SetDamage(_map1.actors[id]);

                            if (id == 0)
                                _mainmap1.floatNumber(target, _map1.actors[id].damage.ToString(), Color.White);

                            if (_map1[target.x, target.y].firstActor is Player)
                                _mainmap1.floatNumber(target, _map1.actors[id].damage.ToString(), Color.DarkRed);

                            if (_map1[target.x, target.y].firstActor is Player) RemoveHealth();
                            if (_map1[target.x, target.y].firstActor.isDead)
                            {
                                _mainmap1.HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Die, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                AddMessage((_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " killed " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name) + " doing " + _map1.actors[id].damage.ToString() + " points of damage.");

                            }
                            else
                            {
                                _mainmap1.HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Hit, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + (_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " attacked " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name));
                                AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + "The attack caused " + (_map1[target.x, target.y].firstActor.armor - _map1.actors[id].damage).ToString() + " points of damage (" + _map1.actors[id].damage.ToString() + " attack strength - " + _map1[target.x, target.y].firstActor.armor + " defense)");
                            }
                            _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Attack, false, dir, true);
                        }
                    }
                    break;


                case Events.MoveActor:
                    {
                        int id = (int)data[0];
                        if (!_mainmap1.IsMoving(id))
                        {
                            Direction dir = (Direction)data[1];
                            Coords target = Map.DirectionTile(_map1.actors[id].tile.coords, dir);

                            _mainmap1.ChangeDir(id, dir); // Look into different direction


                            if ((_map1[target.x, target.y].hasEnemy) || (_map1[target.x, target.y].hasPlayer))
                            {
                                if ((_map1.firstActorID(target.x, target.y) != id) && (!_map1[target.x, target.y].firstActor.isDead))
                                {
                                    HandleEvent(null, Events.Attack, id, dir);
                                    _map1.actors[id].locked = true;
                                }
                            }
                            else
                            {
                                if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                                {
                                    _map1.MoveActor(_map1.actors[id], dir);
                                    if (_map1.actors[id] is Player)
                                        _minimap1.MoveCamera(_map1.actors[id].tile.coords);
                                    _map1.actors[id].locked = true;

                                    _mainmap1.HandleEvent(null, Events.MoveActor, id, _map1.actors[id].tile.coords);
                                    _mainmap2.HandleEvent(null, Events.MoveActor, id, _map1.actors[id].tile.coords);


                                }
                            }
                        }
                    }
                    break;
            }
            base.HandleEvent(sender, eventID, data);
        }

      
    }
}
