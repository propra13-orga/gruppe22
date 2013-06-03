using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if (path.IndexOf(Environment.NewLine) > 0)
            {
                _deadcounter = Int32.Parse(path.Substring(path.IndexOf(Environment.NewLine) + Environment.NewLine.Length));
                path = path.Substring(0, path.IndexOf(Environment.NewLine));
            }
            if (File.Exists("saved" + (string)path))
                _map1 = new Map(Content, this, "saved" + (string)path);
            else
                _map1 = new Map(Content, this, (string)path);
            base.Initialize();
        }
        #endregion

        protected void _CombatDamage(int attacker, int defender)
        {
            if (_map1.actors[attacker].evade + r.Next(10) < _map1.actors[defender].evade + r.Next(10))
            {
                if ((_map1.actors[attacker] is Player) || (_map1.actors[defender] is Player))
                    _mainmap1.floatNumber(_map1.actors[attacker].tile.coords, "Evade", (_map1.actors[defender] is Player) ? Color.Green : Color.White);
            }
            else


                if (_map1.actors[attacker].penetrate + r.Next(10) < _map1.actors[defender].block + r.Next(10))
                {
                    if ((_map1.actors[attacker] is Player) || (_map1.actors[defender] is Player))
                        _mainmap1.floatNumber(_map1.actors[attacker].tile.coords, "Blocked", (_map1.actors[defender] is Player) ? Color.Green : Color.White);
                }
                else
                {
                    int damage = _map1.actors[attacker].damage - _map1.actors[defender].armor + (5 - r.Next(10));
                    if (damage > 0)
                    {
                        _map1.actors[defender].health -= damage;
                        if (_map1.actors[defender] is Player)
                        {
                            _mainmap1.floatNumber(_map1.actors[defender].tile.coords, damage.ToString(), Color.DarkRed);
                            RemoveHealth();
                        }
                        else
                        {
                            if (_map1.actors[attacker] is Player)
                                _mainmap1.floatNumber(_map1.actors[defender].tile.coords, damage.ToString(), Color.White);
                        }
                        if (_map1.actors[defender].isDead)
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, defender, Activity.Die);
                            _mainmap2.HandleEvent(true, Events.AnimateActor, defender, Activity.Die);
                            _map1.actors[attacker].exp += _map1.actors[defender].exp;
                            if (_map1.actors[attacker].exp > _map1.actors[attacker].expNeeded)
                            {
                                _map1.actors[attacker].LevelUp();

                                if (_map1.actors[attacker] is Player)
                                    _mainmap1.floatNumber(_map1.actors[attacker].tile.coords, "Level " + _map1.actors[attacker].level.ToString(), Color.Gold);
                                else
                                {
                                    ((Enemy)_map1.actors[attacker]).AssignSkillsAndAbilities();
                                }
                            }
                            if (_map1.actors[attacker] is Player)
                                _mainmap1.floatNumber(_map1.actors[attacker].tile.coords, "+" + _map1.actors[defender].exp + " Exp", Color.Gold);
                            AddMessage((_map1.actors[defender] is Player ? "<red>" : "") + _map1.actors[defender].name + " was killed by " + _map1.actors[attacker].name + "  doing " + damage.ToString() + " points of damage.");
                        }
                        else
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, attacker, Activity.Hit);
                            _mainmap2.HandleEvent(true, Events.AnimateActor, attacker, Activity.Hit);
                            AddMessage((_map1.actors[defender] is Player ? "<red>" : "") + _map1.actors[defender].name + " was hit by " + _map1.actors[attacker].name + " for " + damage.ToString() + " points of damage.");
                        }
                    }
                    else
                    {
                        if ((_map1.actors[attacker] is Player) || (_map1.actors[defender] is Player))
                            _mainmap1.floatNumber(_map1.actors[defender].tile.coords, "No damage", _map1.actors[defender] is Player ? Color.Green : Color.White);
                    }
                }
        }

        protected void _TrapDamage(Coords target)
        {
            Actor actor = _map1[target].firstActor;
            if (actor == null) return;
            int trapDamage = _map1[target].trap.Trigger();


            if (_map1[target.x, target.y].trap.evade + r.Next(10) < actor.evade + r.Next(10))
            {
                if (actor is Player)
                    _mainmap1.floatNumber(target, "Trap evaded", Color.Green);
            }
            else


                if (_map1[target.x, target.y].trap.penetrate + r.Next(10) < actor.block + r.Next(10))
                {
                    if (actor is Player)
                        _mainmap1.floatNumber(target, "Trap blocked", Color.Green);
                }
                else
                {
                    int damage = trapDamage - actor.armor + (5 - r.Next(10));
                    if (damage > 0)
                    {
                        actor.health -= damage;
                        if (actor is Player)
                        {
                            _mainmap1.floatNumber(target, damage.ToString(), Color.DarkRed);
                            RemoveHealth();
                        }
                        else
                        {
                            //  _mainmap1.floatNumber(target, damage.ToString(), Color.White);
                        };
                        if (actor.isDead)
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Die);
                            _mainmap2.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Die);
                            AddMessage((actor is Player ? "<red>" : "") + actor.name + " was killed by a trap  doing " + damage.ToString() + " points of damage.");
                        }
                        else
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Hit);
                            _mainmap2.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Hit);
                            AddMessage((actor is Player ? "<red>" : "") + actor.name + " was hit for " + damage.ToString() + " points of damage by a trap.");
                        }
                    }
                    else
                    {
                        if (actor is Player)
                            _mainmap1.floatNumber(target, "No damage", Color.Green);
                    }
                }
        }

        /// <summary>
        /// Handle events from UIElements and/or backend objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {

            switch (eventID)
            {
                case Events.LoadFromCheckPoint:
                    _status = GameStatus.NoRedraw;
                    _deadcounter--;
                    string lastCheck = File.ReadAllText("CheckPoint");
                    while (Directory.GetFiles(".", "savedroom*.xml").Length > 0)
                    {
                        File.Delete(Directory.GetFiles(".", "savedroom*.xml")[0]);
                    }
                    Regex re = new Regex(@"\d+");
                    foreach (string file in Directory.GetFiles(".", "checkpoint*.xml"))
                    {
                        Match m = re.Match(file);
                        File.Copy(file, "savedroom" + m.Value + ".xml");
                    }
                    _map1.actors.Clear();
                    _map1.Load("savedroom" + lastCheck + ".xml", null);
                    File.WriteAllText("GameData", "room" + _map1.currRoomNbr.ToString() + ".xml" + Environment.NewLine + _deadcounter.ToString());
                    _mainmap1.resetActors();
                    _mainmap2.resetActors();
                    _inventory.actor = _map1.actors[0];
                    _playerStats.actor = _map1.actors[0];
                    _enemyStats.actor = null;
                    _inventory.Update();
                    _status = GameStatus.Paused;
                    HandleEvent(true, Events.ContinueGame);
                    break;

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
                    File.WriteAllText("GameData", data[0].ToString() + Environment.NewLine + _deadcounter.ToString());
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

                case Events.TrapActivate:
                    {
                        Coords coords = (Coords)data[0];
                        if (((_map1[coords].hasEnemy) || (_map1[coords].hasPlayer)) && (!_map1[coords].firstActor.isDead))
                        {
                            _TrapDamage(coords);
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
                            HandleEvent(true, Events.ChangeMap, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextRoom, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextPlayerPos);

                        }

                        // Apply trap damage
                        if ((_map1[target.x, target.y].hasTrap) && _map1[target.x, target.y].trap.status == TrapState.On)
                        {
                            _TrapDamage(target);
                        }

                        //Checkpoint - save by entering
                        if ((_map1[target.x, target.y].hasCheckpoint) && (!_map1[target.x, target.y].checkpoint.visited) && (id == 0))
                        {
                            _map1[target.x, target.y].checkpoint.visited = true;
                            if (_deadcounter == -1)
                                _deadcounter = 3;
                            if (_map1[target.x, target.y].checkpoint.bonuslife > 0)
                                _deadcounter += _map1[target.x, target.y].checkpoint.bonuslife;
                            _map1.Save("savedroom" + _map1.currRoomNbr + ".xml");
                            _mainmap1.floatNumber(target, "Checkpoint", Color.DarkOliveGreen);
                            File.WriteAllText("GameData", "room" + _map1.currRoomNbr.ToString() + ".xml" + Environment.NewLine + _deadcounter.ToString());
                            File.WriteAllText("CheckPoint", _map1.currRoomNbr.ToString());
                            Regex regex = new Regex(@"\d+");
                            foreach (string file in Directory.GetFiles(".", "checkpoint*.xml"))
                            {
                                File.Delete(file);
                            }

                            foreach (string file in Directory.GetFiles(".", "savedroom*.xml"))
                            {
                                Match m = regex.Match(file);
                                File.Copy(file, "checkpoint" + m.Value + ".xml");
                            }

                            AddMessage("Checkpoint reached (" + _deadcounter.ToString() + " lives remaining)");
                        }

                        // Trigger floor switches
                        if ((_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTarget) && (id == 0))
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, id, Activity.Talk);
                            _mainmap2.HandleEvent(true, Events.AnimateActor, id, Activity.Talk);

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
                        Coords target = Map.DirectionTile(_map1.actors[id].tile.coords, dir);

                        if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                        {

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
                            _mainmap1.HandleEvent(true, Events.AnimateActor, id, Activity.Attack, false, dir, true);
                            _CombatDamage(id, _map1[target.x, target.y].firstActor.id);
                        }


                        /*
                         *                                 _mainmap1.HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Die, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                    AddMessage((_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " killed " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name) + " doing " + _map1.actors[id].damage.ToString() + " points of damage.");

                                }
                                else
                                {
                                    _mainmap1.HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Hit, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                    AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + (_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " attacked " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name));
                                    AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + "The attack caused " + (_map1[target.x, target.y].firstActor.armor - _map1.actors[id].damage).ToString() + " points of damage (" + _map1.actors[id].damage.ToString() + " attack strength - " + _map1[target.x, target.y].firstActor.armor + " defense)");
                                }
    */

                    }
                    break;
                case Events.ExplodeProjectile:
                    {
                        _map1[((ProjectileTile)data[0]).coords].Remove((ProjectileTile)data[0]);
                        _mainmap1.RemoveProjectile(((ProjectileTile)data[0]).id);
                    }
                    break;
                case Events.MoveProjectile:
                    if (data[0] == null)
                    {
                        uint id = _mainmap1.AddProjectile(((FloorTile)data[1]).coords, (Direction)data[2], new ProjectileTile((FloorTile)data[1], (Direction)data[2]));
                        _map1[((FloorTile)data[1]).coords].Add(_mainmap1.GetProjectile(id).tile, false);
                        _mainmap1.GetProjectile(id).tile.id = id;
                        _mainmap1.GetProjectile(id).tile.NextTile(false);
                    }
                    else
                    {
                        _mainmap1.GetProjectile(((ProjectileTile)data[0]).id).moveTo((Coords)data[1]);
                    }
                    break;
                case Events.FinishedProjectileMove:
                    ((ProjectileTile)data[0]).NextTile(true);
                    break;
                case Events.MoveActor:
                    {
                        int id = (int)data[0];
                        Direction dir = (Direction)data[1];

                        if (!_mainmap1.IsMoving(id) || (data.Length > 2))
                        {
                            if (((FloorTile)_map1.actors[id].tile.parent).hasTrap)
                            {
                                if (((FloorTile)_map1.actors[id].tile.parent).trap.status == TrapState.Disabled)
                                    ((FloorTile)_map1.actors[id].tile.parent).trap.status = TrapState.NoDisplay;
                            }
                            Coords target = Map.DirectionTile(_map1.actors[id].tile.coords, dir);

                            _mainmap1.ChangeDir(id, dir); // Look into different direction


                            if ((_map1[target.x, target.y].hasEnemy) || (_map1[target.x, target.y].hasPlayer))
                            {
                                if ((_map1.firstActorID(target.x, target.y) != id) && (!_map1[target.x, target.y].firstActor.isDead))
                                {
                                    HandleEvent(true, Events.Attack, id, dir);
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

                                    _mainmap1.HandleEvent(true, Events.MoveActor, id, _map1.actors[id].tile.coords);
                                    _mainmap2.HandleEvent(true, Events.MoveActor, id, _map1.actors[id].tile.coords);


                                }
                            }
                        }
                        else
                        {
                            if (data.Length < 3)
                            {
                                //  _mainmap1.actors[0].cacheDir = dir;
                            }
                        }
                    }
                    break;
            }
            base.HandleEvent(DownStream, eventID, data);
        }

    }
}
