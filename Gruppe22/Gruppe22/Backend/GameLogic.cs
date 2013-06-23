using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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


        private void _PlaySoundEffect(int index)
        {

            SoundEffectInstance effect = soundEffects[index].CreateInstance();
            /*effect.Pan = 1.0f;*/
            effect.Play();
        }

        /// <summary>
        /// methode to evaluate the damage in a combat between two actors
        /// </summary>
        /// <param name="attacker">the attacking actor</param>
        /// <param name="defender">the attacked actor</param>
        protected void _CombatDamage(int attacker, int defender)
        {
            double _evadeChance = (0.02 * (_map1.actors[defender].evade)) / (1 + 0.065 * (_map1.actors[defender].evade)); // converges to ~25% for values from 0 to 100
            if (r.NextDouble() < _evadeChance)
            {
                if ((_map1.actors[attacker] is Player) || (_map1.actors[defender] is Player))
                    _mainmap1.floatNumber(_map1.actors[attacker].tile.coords, "Evade", (_map1.actors[defender] is Player) ? Color.Green : Color.White);
            }
            else
            {
                double dmgReduction = (0.06 * (_map1.actors[defender].armor)) / (1 + 0.06 * (_map1.actors[defender].armor)); //max ~85% at 100 armor
                int damage = _map1.actors[attacker].damage; // the damage the attacker can deal

                // an actor can block some amount between 0 and the full damage
                double blockChance = (0.02 * (_map1.actors[defender].block)) / (1 + 0.03 * (_map1.actors[defender].block)); // converges to ~50%
                if (r.NextDouble() < blockChance)
                {
                    int blockedValue = Math.Max(_map1.actors[defender].block - _map1.actors[attacker].penetrate, 0); // the amount of damage the defender can block
                    if (blockedValue >= damage) blockedValue = damage;
                    damage = Math.Max(damage - blockedValue, 0);
                    if ((_map1.actors[attacker] is Player) || (_map1.actors[defender] is Player))
                        _mainmap1.floatNumber(_map1.actors[attacker].tile.coords, "Blocked " + blockedValue + "dmg", (_map1.actors[defender] is Player) ? Color.Green : Color.White);
                }

                damage = (int)(damage * (1 - dmgReduction)); // the damage the attacker will deal

                if (damage > 0)
                {
                    _map1.actors[defender].health -= damage;
                    if (_map1.actors[defender] is Player)
                    {
                        _mainmap1.floatNumber(_map1.actors[defender].tile.coords, damage.ToString(), Color.DarkRed);
                        _RemoveHealth();
                    }
                    else
                    {
                        if (_map1.actors[attacker] is Player)
                        {
                            _mainmap1.floatNumber(_map1.actors[defender].tile.coords, damage.ToString(), Color.White);
                            _map1.actors[defender].aggro = true;
                        }
                    }
                    if (_map1.actors[defender].isDead)
                    {
                        _mainmap1.HandleEvent(true, Events.AnimateActor, defender, Activity.Die);
                        //_mainmap2.HandleEvent(true, Events.AnimateActor, defender, Activity.Die);
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
                        _AddMessage((_map1.actors[defender] is Player ? "<red>" : "") + _map1.actors[defender].name + " was killed by " + _map1.actors[attacker].name + "  doing " + damage.ToString() + " points of damage.");
                        if (_map1.actors[defender] is Player) _PlaySoundEffect(4); //SoundEffect damage
                    }
                    else
                    {
                        _mainmap1.HandleEvent(true, Events.AnimateActor, defender, Activity.Hit);
                        //_mainmap2.HandleEvent(true, Events.AnimateActor, defender, Activity.Hit);
                        _AddMessage((_map1.actors[defender] is Player ? "<red>" : "") + _map1.actors[defender].name + " was hit by " + _map1.actors[attacker].name + " for " + damage.ToString() + " points of damage.");
                        if (_map1.actors[defender] is Player) _PlaySoundEffect(4); //SoundEffect damage
                    }
                }
                else
                {
                    if ((_map1.actors[attacker] is Player) || (_map1.actors[defender] is Player))
                        _mainmap1.floatNumber(_map1.actors[defender].tile.coords, "No damage", _map1.actors[defender] is Player ? Color.Green : Color.White);
                }
            }
        }

        /// <summary>
        /// methode to evaluate the damage a trap deals to an actor walking over it or stands on raising trap
        /// </summary>
        /// <param name="target">Coords of the actor which walked over the trap</param>
        protected void _TrapDamage(Coords target)
        {
            Actor actor = _map1[target].firstActor;
            if (actor == null) return;
            int trapDamage = _map1[target].trap.Trigger();
            double _evadeChance = (0.02 * (actor.evade)) / (1 + 0.065 * (actor.evade)); // same formula as in _CombatDamage
            Random r = new Random();

            if (r.NextDouble() < _evadeChance)
            {
                if (actor is Player)
                    _mainmap1.floatNumber(target, "Trap evaded", Color.Green);
            }
            else
            {
                //a trap can either be fully blocked or not blocked
                double blockChance = Math.Max((0.02 * (actor.block - _map1[target.x, target.y].trap.penetrate)) / (1 + 0.03 * (actor.block - _map1[target.x, target.y].trap.penetrate)), 0);
                if (r.NextDouble() < blockChance)
                {
                    if (actor is Player)
                        _mainmap1.floatNumber(target, "Trap blocked", Color.Green);
                }
                else
                {
                    double dmgReduction = (0.06 * (actor.armor)) / (1 + 0.06 * (actor.armor));
                    int damage = (int)(trapDamage * (1 - dmgReduction));
                    if (damage > 0)
                    {
                        actor.health -= damage;
                        if (actor is Player)
                        {
                            _mainmap1.floatNumber(target, damage.ToString(), Color.DarkRed);
                            _RemoveHealth();
                        }
                        else
                        {
                            //  _mainmap1.floatNumber(target, damage.ToString(), Color.White);
                        };
                        if (actor.isDead)
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Die);
                            //_mainmap2.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Die);
                            _AddMessage((actor is Player ? "<red>" : "") + actor.name + " was killed by a trap  doing " + damage.ToString() + " points of damage.");
                            if (actor is Player) _PlaySoundEffect(4); //SoundEffect damage
                        }
                        else
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Hit);
                            //_mainmap2.HandleEvent(true, Events.AnimateActor, actor.id, Activity.Hit);
                            _AddMessage((actor is Player ? "<red>" : "") + actor.name + " was hit for " + damage.ToString() + " points of damage by a trap.");
                            if (actor is Player) _PlaySoundEffect(4); //SoundEffect damage
                        }
                    }
                    else
                    {
                        if (actor is Player)
                            _mainmap1.floatNumber(target, "No damage", Color.Green);
                    }
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
                case Events.ActivateAbility:
                    {
                        Actor actor = (Actor)data[0];
                        int id = (int)data[1];
                        if (id < 0)
                        {
                            actor.Items(-id).UseItem();
                            HandleEvent(false, Events.ShowMessage, "You used " + actor.Items(-id).name);
                            if (actor.Items(-id).destroyed)
                            {
                                _toolbar.HandleEvent(true, Events.AddDragItem, id);
                            }
                        }
                        else
                        {
                            actor.mana -= actor.abilities[id - 1].cost;
                            actor.abilities[id - 1].currentCool = actor.abilities[id - 1].cooldown * 7;
                            switch (actor.abilities[id - 1].element)
                            {
                                case AbilityElement.Charm:
                                    _mainmap1.FireProjectile();

                                    break;
                                case AbilityElement.Fire:
                                    _mainmap1.FireProjectile();

                                    break;
                                case AbilityElement.Health:
                                    actor.health += actor.abilities[id - 1].intensity;
                                    break;
                                case AbilityElement.HealthReg:
                                    actor.health += actor.abilities[id - 1].intensity;
                                    break;
                                case AbilityElement.Ice:
                                    _mainmap1.FireProjectile();

                                    break;
                                case AbilityElement.ManaReg:
                                    break;
                                case AbilityElement.Morph:
                                    _mainmap1.FireProjectile();

                                    break;
                                case AbilityElement.Scare:
                                    _mainmap1.FireProjectile();

                                    break;
                                case AbilityElement.Stun:
                                    _mainmap1.FireProjectile();

                                    break;
                                case AbilityElement.Teleport:
                                    _mainmap1.FireProjectile();

                                    break;
                            }
                        }
                    }
                    break;

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
                    File.WriteAllText("GameData", "room" + _map1.id.ToString() + ".xml" + Environment.NewLine + _deadcounter.ToString());
                    _mainmap1.resetActors();
                    //_mainmap2.resetActors();
                    _mana.actor = _map1.actors[0];
                    _health.actor = _map1.actors[0];
                    _toolbar.actor = _map1.actors[0];
                    _status = GameStatus.Paused;
                    HandleEvent(true, Events.ContinueGame);
                    break;

                case Events.ChangeMap: // Load another map
                    _PlaySoundEffect(0); //SoundEffect change map
                    _status = GameStatus.NoRedraw; // prevent redraw (which would crash the game!)
                    _map1.Save("savedroom" + _map1.id + ".xml");
                    if (File.Exists("saved" + (string)data[0]))
                        _map1.Load("saved" + (string)data[0], (Coords)data[1]);
                    else
                        _map1.Load((string)data[0], (Coords)data[1]);
                    _mainmap1.resetActors();
                    //_mainmap2.resetActors();
                    _minimap1.MoveCamera(_map1.actors[0].tile.coords);

                    _AddMessage("You entered room number " + data[0].ToString().Substring(4, 1) + ".");
                    File.WriteAllText("GameData", data[0].ToString() + Environment.NewLine + _deadcounter.ToString());

                    _backMusic = Content.Load<Song>(_map1.music); // Todo: *.mp3
                    MediaPlayer.Play(_backMusic);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)0.3;
                    _status = GameStatus.Running;
                    break;

                case Events.FinishedAnimation:
                    int FinishedID = (int)data[0];
                    Activity FinishedActivity = (Activity)data[1];
                    if (FinishedActivity == Activity.Die)
                    {
                        if (_map1.actors[FinishedID] is Enemy)
                        {
                            if (_map1.actors[FinishedID].tile.enabled)
                            {
                                ((ActorTile)_map1.actors[FinishedID].tile).enabled = false;
                                _AddMessage(_map1.actors[FinishedID].name + " is dead.");
                                ((ActorTile)_map1.actors[FinishedID].tile).DropItems();
                                if (_map1.actors[FinishedID].gold > 0)
                                {

                                    ItemTile tile = new ItemTile(((FloorTile)(_map1.actors[FinishedID].tile.parent)));

                                    Item item = new Item(Content, tile, ItemType.Gold, "", null, _map1.actors[FinishedID].gold);
                                    item.value = _map1.actors[FinishedID].gold;
                                    tile.item = item;
                                    ((FloorTile)(_map1.actors[FinishedID].tile.parent)).Add(tile);
                                }
                            }
                        }
                        else
                        {
                            _AddMessage("<red>You are dead.");
                            _RemoveHealth();
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
                            _PlaySoundEffect(2); //SoundEffect pick items
                            _AddMessage(((id == 0) ? "You found " : _map1.actors[id].name + " found ") + _map1[target.x, target.y].firstItem.item.name + " .");
                            if (id == 0)
                                _mainmap1.floatNumber(target, "Found " + _map1[target.x, target.y].firstItem.item.name, Color.DarkGreen);
                            _map1[target.x, target.y].firstItem.item.Pickup(_map1.actors[id]);
                            _map1[target.x, target.y].Remove(_map1[target.x, target.y].firstItem);
                        }
                        // Apply teleporter (move to next room)
                        if ((id == 0) && (_map1[target.x, target.y].hasTeleport))
                        {
                            HandleEvent(true, Events.ChangeMap, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextRoom, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextPlayerPos);

                        }

                        // Apply trap damage
                        if (((_map1[target.x, target.y].hasTrap) && _map1[target.x, target.y].trap.status == TrapState.On) && !(_map1.actors[id] is NPC))
                        {
                            _TrapDamage(target);
                        }

                        //Checkpoint - save by entering
                        if ((_map1[target.x, target.y].hasCheckpoint) && (!_map1[target.x, target.y].checkpoint.visited) && (id == 0))
                        {
                            _PlaySoundEffect(1);//SoundEffect checkpoint
                            _map1[target.x, target.y].checkpoint.visited = true;
                            _map1.actors[id].health = _map1.actors[id].maxhealth;
                            _map1.actors[id].mana = _map1.actors[id].maxMana;
                            if (_deadcounter == -1)
                                _deadcounter = 3;
                            if (_map1[target.x, target.y].checkpoint.bonuslife > 0)
                                _deadcounter += _map1[target.x, target.y].checkpoint.bonuslife;
                            _map1.Save("savedroom" + _map1.id + ".xml");
                            _mainmap1.floatNumber(target, "Checkpoint", Color.DarkOliveGreen);
                            File.WriteAllText("GameData", "room" + _map1.id.ToString() + ".xml" + Environment.NewLine + _deadcounter.ToString());
                            File.WriteAllText("CheckPoint", _map1.id.ToString());
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

                            _AddMessage("Checkpoint reached (" + _deadcounter.ToString() + " lives remaining)");
                            _mainmap1.HandleEvent(true, Events.Player1, 1);
                        }

                        // Trigger floor switches
                        if ((_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTarget) && (id == 0))
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, id, Activity.Talk);
                            //_mainmap2.HandleEvent(true, Events.AnimateActor, id, Activity.Talk);

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
                        if (data[2] != null)
                        {
                            Actor actor = data[2] as Actor;
                            int damage = 20 - actor.armor + (5 - r.Next(10));
                            if (damage > 0)
                            {
                                actor.health -= damage;
                                if (actor is Player)
                                {
                                    _mainmap1.floatNumber(actor.tile.coords, damage.ToString(), Color.DarkRed);
                                    _RemoveHealth();
                                }
                            }
                        }
                        _mainmap1.HandleEvent(true, eventID, data);
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

                case Events.Shop:
                    ShowShopWindow(_map1.actors[0], (Actor)data[0]);
                    break;

                case Events.Dialogue:
                    ShowMessage();
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

                            Actor a = _map1[target.x, target.y].firstActor;
                            if ((a is NPC) && (_map1.actors[id] is Player))
                            {
                                (a as NPC).Interact();
                            }
                            if (((a is Enemy || a is Player) && !(_map1.actors[id] is NPC))
                                && ((a.id != id) && (!a.isDead)))
                            {
                                HandleEvent(true, Events.Attack, id, dir);
                                _map1.actors[id].locked = true;

                            }
                            else
                            {
                                if ((_map1[target].hasDoor) && (_map1.actors[id] is Player) && (!_map1[target].door.open))
                                {
                                    if (_map1.actors[id].HasKey(_map1.level))
                                    {
                                        _map1[target].door.open = true;
                                        ShowMessage("You open the door using the key you fought for.");
                                    }
                                    else
                                    {
                                        ShowMessage("The door is locked.\n It is likely a strong creature guards the key.");
                                    }
                                }
                                if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                                {
                                    _map1.MoveActor(_map1.actors[id], dir);
                                    if (_map1.actors[id] is Player)
                                        _minimap1.MoveCamera(_map1.actors[id].tile.coords);
                                    _map1.actors[id].locked = true;

                                    _mainmap1.HandleEvent(true, Events.MoveActor, id, _map1.actors[id].tile.coords);
                                    //_mainmap2.HandleEvent(true, Events.MoveActor, id, _map1.actors[id].tile.coords);


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


        private Direction FindAvailableExits(List<Generator> rooms = null, int id = 0, int col = 0, int totalCols = 0, int row = 0, int totalRows = 0, int totalRooms = 0, int roomsPerRow = 0)
        {
            Direction exitsPossible = rooms[id].blocked;
            if (row == 0) // Highest Row: Remove all exits from top row
            {
                exitsPossible &= ~Direction.Up;
                exitsPossible &= ~Direction.UpRight;
                exitsPossible &= ~Direction.UpLeft;
            }
            if (row == totalRows - 1) // Lowest Row: Remove all exits from bottom row
            {
                exitsPossible &= ~Direction.Down;
                exitsPossible &= ~Direction.DownLeft;
                exitsPossible &= ~Direction.DownRight;
            }

            if (col == 0) // Lowest Column: Remove exits to left
            {
                exitsPossible &= ~Direction.Left;
                exitsPossible &= ~Direction.DownLeft;
                exitsPossible &= ~Direction.UpLeft;
            }
            if (col == totalCols - 1) // Highest Column: Remove exits to right
            {
                exitsPossible &= ~Direction.Right;
                exitsPossible &= ~Direction.DownRight;
                exitsPossible &= ~Direction.UpRight;
            }

            if (row == totalRows - 2)
            { // Next to final (possibly incomplete) row
                if (col >= totalRooms - (roomsPerRow * (totalRows - 1))) // Remove Exit downRight
                {
                    exitsPossible &= ~Direction.DownRight;
                }

                if (col >= totalRooms - (roomsPerRow * (totalRows - 1))) // Remove Exit Down
                {
                    exitsPossible &= ~Direction.Down;
                }
                if (col > totalRooms - (roomsPerRow * (totalRows - 1)) + 1) // Remove Exit DownLeft
                {
                    exitsPossible &= ~Direction.DownLeft;
                }
            }
            //    System.Diagnostics.Debug.WriteLine(id + ": Exits " + exitsPossible);

            return exitsPossible;

        }

        public void ConnectRooms(List<Generator> rooms = null, int From = 0, Direction exit = Direction.None, int roomsPerRow = 0)
        {
            int To = From;

            if ((exit == Direction.Up) || (exit == Direction.UpLeft) || (exit == Direction.UpRight))
            {
                To -= roomsPerRow;
            }
            if ((exit == Direction.Down) || (exit == Direction.DownLeft) || (exit == Direction.DownRight))
            {
                To += roomsPerRow;
            }
            if ((exit == Direction.Left) || (exit == Direction.DownLeft) || (exit == Direction.UpLeft))
            {
                To -= 1;
            }
            if ((exit == Direction.Right) || (exit == Direction.DownRight) || (exit == Direction.UpRight))
            {
                To += 1;
            }
            Coords CoordsFrom = rooms[From].SuggestExit(exit);
            Coords CoordsTo = rooms[To].SuggestExit(Map.OppositeDirection(exit));
            //  System.Diagnostics.Debug.WriteLine(exit.ToString() + ": Level " + rooms[From].level.ToString() + ": " + From + " to " + To + " (" + roomsPerRow.ToString() + ")");

            if ((CoordsFrom.x != -1) && (CoordsFrom.y != -1) && (CoordsTo.x != -1) && (CoordsTo.y != -1))
            {
                rooms[From].ConnectTo(CoordsFrom, To, CoordsTo, false);
                rooms[To].ConnectTo(CoordsTo, From, CoordsFrom, false);
                if (rooms[From].connected)
                {
                    ConnectRecursive(rooms, To);
                }
                if (rooms[To].connected)
                {
                    ConnectRecursive(rooms, From);
                }
            }

        }

        public void ConnectRecursive(List<Generator> rooms, int id, List<int> visited = null)
        {
            //   System.Diagnostics.Debug.WriteLine("Connect " + id);

            if (visited == null)
            {
                visited = new List<int>();
            }
            visited.Add(id);
            rooms[id].connected = true;
            foreach (int connection in rooms[id].connectedRooms)
            {
                if (!visited.Contains(connection - 1))
                {
                    ConnectRecursive(rooms, connection - 1, visited);
                }
            }
        }



        /// <summary>
        /// A text displayed if the player died
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        public void ShowMessage(string message = "")
        {
            if (message == "")
                switch (r.Next(10))
                {
                    case 0:
                        message = "This is a cursed place. Evil creatures are attacking everyone in sight.";
                        break;
                    case 1:
                        message = "You should really get better equipment at a shop. My brother might help you out.\n He is over at the entrance.";
                        break;
                    case 2:
                        message = "It is hopeless. We are all going to die...";
                        break;

                    case 3:
                        message = "There are pests on the first level, undead on the second and unknown evil on the third...";
                        break;

                    case 4:
                        message = "Nobody has ever found a way out of here...";
                        break;

                    case 5:
                        message = "Are you sure you can take on the enemies around here?";
                        break;

                    case 6:
                        message = "I haven't gotten any sleep for a week!";
                        break;

                    case 7:
                        message = "It is rumored that there is a deadly beast on the lowest level...";
                        break;

                    case 8:
                        message = "The lower levels are far darker than this level...";
                        break;

                    case 9:
                        message = "If this were a real dungeon, someone might have a quest for you...";
                        break;
                }
            _status = GameStatus.Paused;
            Window _messagebox = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 600, 200));
            Statusbox stat = new Statusbox(_messagebox, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 70, 590, 110), false, true);
            stat.AddLine(message);
            _messagebox.AddChild(stat);
            _messagebox.AddChild(new Button(_messagebox, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 65, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 130, 40), "Goodbye!", (int)Buttons.Close));
            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));

            _interfaceElements.Add(_messagebox);
            _messagebox.ChangeFocus();
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }


        /// <summary>
        /// Generate three levels consisting of multiple rooms each and save them to xml files
        /// </summary>
        public void GenerateMaps()
        {

            List<Generator> rooms = new List<Generator>();
            int maxLevel = 3;
            int LevelStart = 0;
            int prevTotal = 0;
            int prevLevelStart = 0;
            for (int level = 1; level < maxLevel + 1; ++level)  // Generate 3 levels (for now; possibly random number of levels later)
            {
                LevelStart = rooms.Count();

                // Phase 1: Generate maze like rooms
                int totalRooms = r.Next(10) + 3;
                string _name = null;
                for (int i = 0; i < totalRooms; ++i)
                {

                    rooms.Add(new Generator(Content, this, 10 + r.Next(8) + ((i == totalRooms) ? 5 : 0), 10 + r.Next(8) + ((i == totalRooms) ? 5 : 0), true, null, LevelStart + i + 1, totalRooms, r, _name, level));
                    if (_name == null) _name = rooms[i].dungeonname;
                    if (i == 0)
                    {
                        rooms[i].connected = true;
                    }

                    if (i + LevelStart == 0)
                    {
                        rooms[i].AddShop();
                        rooms[i].AddNPC();

                    }
                }
                // Phase 2 Generate roads between rooms
                int roomsPerRow = (int)Math.Floor(Math.Sqrt(totalRooms));
                int totalRows = (int)Math.Ceiling(((float)totalRooms / (float)roomsPerRow));
                for (int row = 0; row < totalRows; ++row)
                {

                    int totalCols = roomsPerRow;
                    if (row == totalRows - 1)
                    {
                        totalCols = totalRooms - (roomsPerRow * (totalRows - 1));
                    };
                    for (int col = 0; col < totalCols; ++col)
                    {
                        int From = LevelStart + roomsPerRow * row + col;
                        Direction exitsPossible = FindAvailableExits(rooms, From, col, totalCols, row, totalRows, totalRooms, roomsPerRow);

                        do
                        {
                            Direction exit = (Direction)Math.Pow(2, r.Next(4));
                            do
                            {
                                exit = (Direction)Math.Pow(2, r.Next(4));
                            } while ((exitsPossible != Direction.None) && ((exit == Direction.None) || (!exitsPossible.HasFlag(exit))));
                            if ((exit != Direction.None) && (exitsPossible != Direction.None))
                                ConnectRooms(rooms, From, exit, roomsPerRow);
                            exitsPossible &= ~exit;
                        }
                        while ((exitsPossible != Direction.None) && r.Next(100) > 80);
                    }
                }
                for (int i = LevelStart; i < rooms.Count; ++i)
                {
                    int col = (i - LevelStart) % roomsPerRow;
                    int maxcol = ((int)Math.Floor((float)(i - LevelStart) / (float)roomsPerRow) == totalRows - 1) ? (totalRooms - (roomsPerRow * (totalRows - 1))) : roomsPerRow;
                    int row = (int)Math.Floor((float)(i - LevelStart) / (float)roomsPerRow);

                    Direction exits = FindAvailableExits(rooms, i, col,
                        maxcol,
                        row,
                        totalRows,
                        totalRooms,
                        roomsPerRow);
                    while ((exits != Direction.None) && (!rooms[i].connected))
                    {
                        Direction exit = (Direction)Math.Pow(2, r.Next(4));
                        if (exits.HasFlag(exit))
                            ConnectRooms(rooms, i, exit, roomsPerRow);
                        exits = FindAvailableExits(rooms, i, col,
                        maxcol,
                        row,
                        totalRows,
                        totalRooms,
                        roomsPerRow);
                    }
                }


                // Phase 4 Add stairs (up and corresponding down)
                if (level != 1)
                {

                    int exit = r.Next(totalRooms) + LevelStart;
                    while (rooms[exit].hasStairs)
                    {
                        exit = r.Next(totalRooms) + LevelStart;
                    };

                    int entrance = r.Next(prevTotal) + prevLevelStart;
                    while (rooms[entrance].hasStairs)
                    {
                        entrance = r.Next(prevTotal) + prevLevelStart;
                    };
                    Coords entranceCoords = rooms[entrance].FindRoomForStairs;
                    Coords exitCoords = rooms[exit].FindRoomForStairs;
                    rooms[exit].AddStairs(exitCoords, entrance + 1, entranceCoords, true);
                    rooms[entrance].AddStairs(entranceCoords, exit + 1, exitCoords, false);
                    rooms[exit].AddShop();
                }

                int checkpoint = LevelStart + r.Next(totalRooms);
                rooms[checkpoint].AddCheckpoint();
                for (int i = 0; i < r.Next(3) + 1; ++i)
                {
                    int npc = LevelStart + r.Next(totalRooms);
                    rooms[npc].AddNPC();
                }
                if (level == maxLevel)
                {
                    int exit = r.Next(totalRooms) + LevelStart;
                    while (rooms[exit].hasStairs)
                    {
                        exit = r.Next(totalRooms) + LevelStart;
                    };
                    Coords targetCoords = rooms[exit].FindRoomForStairs;
                    rooms[exit].AddTarget(targetCoords);
                }

                // Phase 5 Add Checkpoints, Shops and Boss Fights; cut out areas in maps and add appropriate "regions"

                // Phase 6 Add a few challenges: Switches, Doors, Illusionary and destructible walls, keys and locks

                // Phase 7 Add Other NPCs, environmental elements and Quest Items / Enemies / NPCs
                int boss = r.Next(totalRooms) + LevelStart;
                while ((rooms[boss].hasStairs) || (boss == 0))
                {
                    boss = r.Next(totalRooms) + LevelStart;
                };
                rooms[boss].AddBoss();

                prevLevelStart = LevelStart;
                prevTotal = totalRooms;
            }

            foreach (Generator map in rooms)
            {
                map.Save("room" + map.id + ".xml");
                map.Dispose();
            }
            rooms.Clear();
        }
    }
}
