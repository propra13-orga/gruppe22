using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{

    /// <summary>
    /// Program Logic completely abstracted from Frontend
    /// </summary>
    public class PureLogic : Logic, IHandleEvent
    {
        private bool _paused = true;

        /// <summary>
        /// Method to change the "room"
        /// Loads the saved (previously visited) version of the next room if possible
        /// and the generated version if there is no saved one
        /// </summary>
        /// <param name="filename">The path to the .xml of the next room</param>
        /// <param name="pos">The spawning position in the next room</param>
        public override void ChangeMap(string filename, Coords pos)
        {
            map.Save("savedroom" + map.id + ".xml");
            if (File.Exists("saved" + filename))
                map.Load("saved" + filename, pos, false);
            else
                map.Load(filename, pos, false);
            map.Save("saved" + filename);
            File.WriteAllText("GameData", filename);
        }

        public void ReassignPlayer()
        {
            int _playerID = 0;
            for (int i = 0; i < map.actors.Count; ++i)
            {
                if (map.actors[i] is Player)
                {
                    _playerID = i;
                    break;
                }
            }
            _parent.HandleEvent(true, Backend.Events.ChangeMap, _playerID);
        }

        /// <summary>
        /// Method to start the update-routine of the map
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            if (!_paused)
            {
                _map.Update(gametime);
            }
        }
        /// <summary>
        /// Pause / Unpause Game Logic
        /// </summary>
        public bool paused
        {
            get
            {
                return _paused;
            }
            set
            {
                _paused = value;
            }
        }

        /// <summary>
        /// Method to for reseting the game to the generated version
        /// </summary>
        public void Restart()
        {
            _DeleteSavedRooms();
            map.Load("room1.xml");
        }

        /// <summary>
        /// Method to delete old saved rooms
        /// </summary>
        private void _DeleteSavedRooms()
        {
            if (File.Exists("GameData")) File.Delete("GameData");
            while (Directory.GetFiles(".", "savedroom*.xml").Length > 0)
            {
                File.Delete(Directory.GetFiles(".", "savedroom*.xml")[0]);
            }
        }

        /// <summary>
        /// Methode to evaluate the damage in a combat between two actors.
        /// Calculates the percentage damage reduction based on armor and penetration.
        /// Shows the floating combat text for the attack.
        /// </summary>
        /// <param name="attacker">the attacking actor</param>
        /// <param name="defender">the attacked actor</param>
        protected void _CombatDamage(int attacker, int defender)
        {
            if (!_paused)
            {
                if (_map.actors[attacker].damage > 0)
                {
                    double _evadeChance = (0.02 * (_map.actors[defender].evade)) / (1 + 0.065 * (_map.actors[defender].evade)); // converges to ~25% for values from 0 to 100
                    if (_random.NextDouble() < _evadeChance) // show evade-message if the attack was evaded
                    {
                        _parent.HandleEvent(false, Events.ActorText, defender, _map.actors[defender].tile.coords, "Evade");
                    }
                    else
                    {
                        double dmgReduction = (0.06 * (_map.actors[defender].armor)) / (1 + 0.06 * (_map.actors[defender].armor)); // calculate damage reduction from armor; max 85% @100armor
                        int damage = _map.actors[attacker].damage;

                        // an actor can block some amount between 0 and the full damage
                        double blockChance = (0.02 * (_map.actors[defender].block)) / (1 + 0.03 * (_map.actors[defender].block)); // converges to ~50% @100block
                        if (_random.NextDouble() < blockChance)
                        {
                            int blockedValue = Math.Max(_map.actors[defender].block - _map.actors[attacker].penetrate, 0); // calculate blocked damage and show block-message
                            if (blockedValue >= damage) blockedValue = damage;
                            damage = Math.Max(damage - blockedValue, 0);
                            _parent.HandleEvent(false, Events.ActorText, defender, _map.actors[defender].tile.coords, "Block" + blockedValue + "dmg");
                        }

                        damage = (int)(damage * (1 - dmgReduction)); // the damage the attacker will deal after armor, penetration etc
                        _map.actors[defender].health -= damage;

                        //check and calculate elemental (magic?) damage
                        if (_map.actors[attacker].fireDamage > 0)
                        {
                            double firedmgreduction = (0.06 * (_map.actors[defender].fireDefense)) / (1 + 0.06 * (_map.actors[defender].fireDefense));
                            int firedmg = (int)(_map.actors[attacker].fireDamage * (1 - firedmgreduction));
                            _map.actors[defender].health -= firedmg;
                        }
                        if (_map.actors[attacker].iceDamage > 0)
                        {
                            double frostdmgreduction = (0.06 * (_map.actors[defender].iceDefense)) / (1 + 0.06 * (_map.actors[defender].iceDefense));
                            int frostdmg = (int)(_map.actors[attacker].iceDamage * (1 - frostdmgreduction));
                            _map.actors[defender].health -= frostdmg;
                        }


                        if (_map.actors[defender].isDead)
                        {
                            _parent.HandleEvent(false, Events.KillActor, defender, _map.actors[defender].tile.coords, _map.actors[defender].health, damage);
                            //_mainmap2.HandleEvent(true, Backend.Events.AnimateActor, defender, Activity.Die);
                            _map.actors[attacker].exp += _map.actors[defender].exp;
                            // If the attacker killed the enemy and got enough exp he levels up
                            if (_map.actors[attacker].exp > _map.actors[attacker].expNeeded)
                            {
                                _map.actors[attacker].LevelUp();

                                if (!(_map.actors[attacker] is Player))
                                    ((Enemy)_map.actors[attacker]).AssignSkillsAndAbilities(); // Auto-improve statistics of enemies
                                _parent.HandleEvent(false, Events.ChangeStats, attacker, _map.actors[attacker]); // Update data on client
                            }
                            else
                            {
                                _parent.HandleEvent(false, Events.ActorText, attacker, _map.actors[defender].tile.coords, "+" + _map.actors[defender].exp + " Exp", Color.Gold); // Update data on client
                            }
                        }
                        else
                        {
                            _parent.HandleEvent(false, Events.DamageActor, defender, _map.actors[defender].tile.coords, _map.actors[defender].health, damage);
                            _map.actors[defender].aggro = true;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// methode to evaluate the damage a trap deals to an actor walking over it or stands on raising trap
        /// </summary>
        /// <param name="target">Coords of the actor which walked over the trap</param>
        protected void _TrapDamage(Coords target)
        {
            if (!_paused)
            {
                foreach (Actor actor in _map[target].actors)
                {
                    if (actor == null) return;
                    int trapDamage = _map[target].trap.Trigger();
                    double _evadeChance = (0.02 * (actor.evade)) / (1 + 0.065 * (actor.evade)); // same formula as in _CombatDamage

                    if (_random.NextDouble() < _evadeChance)
                    {
                        if (actor is Player)
                            _parent.HandleEvent(false, Events.ActorText, actor.id, target, "Trap evaded");
                    }
                    else
                    {
                        //a trap can either be fully blocked or not blocked
                        double blockChance = Math.Max((0.02 * (actor.block - _map[target.x, target.y].trap.penetrate)) / (1 + 0.03 * (actor.block - _map[target.x, target.y].trap.penetrate)), 0);
                        if (_random.NextDouble() < blockChance)
                        {
                            if (actor is Player)
                                _parent.HandleEvent(false, Events.ActorText, actor.id, target, "Trap blocked");
                        }
                        else
                        {
                            double dmgReduction = (0.06 * (actor.armor)) / (1 + 0.06 * (actor.armor));
                            int damage = (int)(trapDamage * (1 - dmgReduction));

                            actor.health -= damage;
                            if (actor.isDead)
                            {
                                _parent.HandleEvent(false, Events.KillActor, actor.id, target, actor.health, damage);
                            }
                            else
                            {
                                _parent.HandleEvent(false, Events.DamageActor, actor.id, target, actor.health, damage);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// A text displayed if the player died or talk to NPC
        /// </summary>
        /// <param name="message">A custom message that could be displayed</param>
        public void GenericDialog(int from, int to, string message = "")
        {
            if (!_paused)
            {
                if (message == "")
                    switch (_random.Next(10))
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
                _parent.HandleEvent(false, Events.Dialog, from, to, message, new Backend.DialogLine[] { new Backend.DialogLine("Goodbye", -1) });
            }
        }

        /// /// <summary>
        /// Handle events from UIElements and/or backend objects
        /// </summary>
        ///  <param name="DownStream"></param>
        /// <param name="eventID">The ID of the event</param>
        /// <param name="data">The parameters needed for that event</param>
        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {

            switch (eventID)
            {
                // Client: Player used item/ability
                // Map: NPC / Monster used item/ability
                case Backend.Events.ActivateAbility:
                    {
                        Actor actor = (Actor)data[0];
                        int id = (int)data[1];

                        // Use Item from inventory
                        if (id < 0)
                        {
                            actor.Items(-id).UseItem();
                            // 2 Events: 
                            // 1. Item entfernen bzw. ausrüsten
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, actor.id, id);
                            // 2. Statistiken anpassen
                            _parent.HandleEvent(false, Events.ChangeStats, actor.id, actor); // Update data on client

                        }
                        else
                        {
                            actor.mana -= actor.abilities[id - 1].cost;
                            actor.abilities[id - 1].currentCool = actor.abilities[id - 1].cooldown * 7;
                            switch (actor.abilities[id - 1].element)
                            {
                                case AbilityElement.Charm:
                                    _parent.HandleEvent(false, Events.FireProjectile, actor.id, AbilityElement.Charm);
                                    break;
                                case AbilityElement.Fire:
                                    _parent.HandleEvent(false, Events.FireProjectile, actor.id, AbilityElement.Fire);
                                    break;
                                case AbilityElement.Health:
                                    actor.health += actor.abilities[id - 1].intensity;
                                    break;
                                case AbilityElement.HealthReg:
                                    actor.health += actor.abilities[id - 1].intensity;
                                    break;
                                case AbilityElement.Ice:
                                    _parent.HandleEvent(false, Events.FireProjectile, actor.id, AbilityElement.Ice);
                                    break;
                                case AbilityElement.ManaReg:
                                    break;
                                case AbilityElement.Scare:
                                    _parent.HandleEvent(false, Events.FireProjectile, actor.id, AbilityElement.Scare);
                                    break;
                                case AbilityElement.Stun:
                                    _parent.HandleEvent(false, Events.FireProjectile, actor.id, AbilityElement.Stun);
                                    break;
                            }
                            _parent.HandleEvent(false, Events.ChangeStats, actor.id, actor); // Update data on client
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, actor.id, id);

                        }
                    }
                    break;

                // Client: Animation finished playing => Re-Enable Actor, continue game
                case Backend.Events.FinishedAnimation:
                    int FinishedID = (int)data[0];
                    if (_map.actors.Count >= FinishedID)
                    {
                        Activity FinishedActivity = (Activity)data[1];
                        switch (FinishedActivity)
                        {
                            case Activity.Die:
                                if (_map.actors[FinishedID] is Enemy)
                                {
                                    if (_map.actors[FinishedID].tile.enabled)
                                    {
                                        ((ActorTile)_map.actors[FinishedID].tile).enabled = false;
                                        ((ActorTile)_map.actors[FinishedID].tile).DropItems();
                                        if (_map.actors[FinishedID].gold > 0)
                                        {
                                            ItemTile tile = new ItemTile(((FloorTile)(_map.actors[FinishedID].tile.parent)));
                                            Backend.Item item = new Backend.Item(tile, Backend.ItemType.Gold, "", null, _map.actors[FinishedID].gold);
                                            item.value = _map.actors[FinishedID].gold;
                                            tile.item = item;
                                            ((FloorTile)(_map.actors[FinishedID].tile.parent)).Add(tile);
                                        }
                                        _parent.HandleEvent(false, Events.SetItemTiles, _map.actors[FinishedID].tile, ((FloorTile)(_map.actors[FinishedID].tile.parent)).itemTiles);
                                    }
                                }
                                else
                                {
                                    _parent.HandleEvent(false, Events.KillActor, FinishedID);
                                }

                                break;
                            default:
                                _map.actors[FinishedID].locked = false;
                                break;
                        }
                    }
                    break;

                case Backend.Events.TrapActivate:
                    {
                        Backend.Coords coords = (Coords)data[0];
                        if (((_map[coords].hasEnemy) || (_map[coords].hasPlayer)) && (!_map[coords].firstActor.isDead))
                        {
                            _TrapDamage(coords);
                        }
                    }
                    break;

                case Backend.Events.EndGame:
                    map.Save("savedroom" + map.id + ".xml");
                    File.WriteAllText("GameData", "room" + map.id.ToString() + ".xml");
                    break;

                case Backend.Events.TileEntered:
                    {
                        int id = (int)data[0];
                        if (id < _map.actors.Count)
                        {
                            _map.actors[id].locked = false;

                            Direction dir = (Direction)data[1];
                            Backend.Coords target = _map.actors[id].tile.coords;

                            // Pickup any items
                            while (_map[target.x, target.y].hasTreasure)
                            {
                                _parent.HandleEvent(false, Events.PlaySound, SoundFX.Pickup); //SoundEffect pick items
                                _parent.HandleEvent(false, Events.ShowMessage, ((_map.actors[id] is Player) ? "You found " : _map.actors[id].name + " found ") + _map[target.x, target.y].firstItem.item.name + " .");
                                if (_map.actors[id] is Player)
                                    _parent.HandleEvent(false, Events.ActorText, _map[target].firstActor, target, "Found " + _map[target.x, target.y].firstItem.item.name, Color.DarkGreen);
                                _map[target.x, target.y].firstItem.item.Pickup(_map.actors[id]);
                                _map[target.x, target.y].Remove(_map[target.x, target.y].firstItem);
                            }


                            // Apply trap damage
                            if (((_map[target.x, target.y].hasTrap) && _map[target.x, target.y].trap.status == TrapState.On) && !(_map.actors[id] is NPC))
                            {
                                _TrapDamage(target);
                            }

                            if (_map.actors[id] is Player)
                            {

                                //Checkpoint - save by entering
                                if ((_map[target.x, target.y].hasCheckpoint) && (!_map[target.x, target.y].checkpoint.visited))
                                {
                                    _parent.HandleEvent(false, Events.PlaySound, SoundFX.Checkpoint);//SoundEffect checkpoint
                                    _map[target.x, target.y].checkpoint.visited = true;
                                    _map.actors[id].health = _map.actors[id].maxhealth;
                                    _map.actors[id].mana = _map.actors[id].maxMana;
                                    if (_map.actors[id].lives == -1)
                                        _map.actors[id].lives = 3;
                                    if (_map[target.x, target.y].checkpoint.bonuslife > 0)
                                        _map.actors[id].lives += (int)_map[target.x, target.y].checkpoint.bonuslife;
                                    _map.Save("savedroom" + _map.id + ".xml");
                                    _parent.HandleEvent(false, Events.ActorText, _map[target].firstActor, target, "Checkpoint", Color.DarkOliveGreen);
                                    File.WriteAllText("GameData", "room" + _map.id.ToString() + ".xml" + Environment.NewLine + _map.actors[id].lives.ToString());
                                    File.WriteAllText("CheckPoint", _map.id.ToString());
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

                                    _parent.HandleEvent(false, Events.ShowMessage, "Checkpoint reached (" + _map.actors[id].lives.ToString() + " lives remaining)");
                                    _parent.HandleEvent(false, Events.Checkpoint);
                                }

                                // Trigger floor switches
                                if (_map[_map.actors[id].tile.coords.x, _map.actors[id].tile.coords.y].hasTarget)
                                {
                                    _parent.HandleEvent(false, Backend.Events.AnimateActor, id, Activity.Talk);
                                    //_mainmap2.HandleEvent(true, Backend.Events.AnimateActor, id, Activity.Talk);

                                    _parent.HandleEvent(false, Events.GameOver, true);
                                }

                                // Apply teleporter (move to next room)
                                if (_map[target.x, target.y].hasTeleport)
                                {
                                    HandleEvent(true, Backend.Events.ChangeMap, ((TeleportTile)_map[target.x, target.y].overlay[0]).nextRoom, ((TeleportTile)_map[target.x, target.y].overlay[0]).nextPlayerPos);

                                }
                            }

                        }
                    }
                    // Allow to choose next turn
                    break;


                case Backend.Events.Attack:
                    {
                        int id = (int)data[0];
                        Direction dir = (Direction)data[1];
                        Backend.Coords target = Map.DirectionTile(_map.actors[id].tile.coords, dir);
                        if (_map.CanMove(_map.actors[id].tile.coords, dir))
                        {
                            _parent.HandleEvent(false, Backend.Events.Attack, id, dir);
                            _CombatDamage(id, _map[target.x, target.y].firstActor.id);
                        }
                    }
                    break;

                case Backend.Events.ExplodeProjectile:
                    {
                        _map[((ProjectileTile)data[0]).coords].Remove((ProjectileTile)data[0]);
                        if (data[2] != null)
                        {
                            Actor actor = data[2] as Actor;
                            int damage = 20 - actor.armor + (5 - _random.Next(10));
                            if (damage > 0)
                            {
                                actor.health -= damage;
                                if (actor is Player)
                                {
                                    _parent.HandleEvent(false, Events.FloatText, actor.tile.coords, damage.ToString(), Color.DarkRed);
                                    _parent.HandleEvent(false, Events.DamageActor);
                                }
                            }
                        }
                        _parent.HandleEvent(false, eventID, data);
                    }
                    break;

                case Backend.Events.MoveProjectile:
                    if (data[0] == null)
                    {
                        _parent.HandleEvent(false, Backend.Events.AddProjectile, ((FloorTile)data[1]).coords, (Direction)data[2], new ProjectileTile((FloorTile)data[1], (Direction)data[2]));
                    }
                    else
                    {
                        _parent.HandleEvent(false, Backend.Events.MoveProjectile, ((ProjectileTile)data[0]).id, (Coords)data[1]);
                    }
                    break;

                case Backend.Events.FinishedProjectileMove:
                    ((ProjectileTile)data[0]).NextTile(true);
                    break;

                case Backend.Events.Shop:
                    _parent.HandleEvent(false, Events.Shop, (Actor)data[1], (Actor)data[0]);
                    break;

                case Backend.Events.Dialog:
                    //TODO: Quest anzeigen an der stelle, jetzt können die "sinnlosen Dialogtexte durch Questziele ersetzt werden"
                    Player cur_palyer = ((Player)data[1]); //player
                    string texttodisplay = "";
                    int queststodo = 0;
                    //show all quests and show wether finished
                    if (cur_palyer.QuestsCount <= 0)
                        texttodisplay += "Willkommen, ich bin ein freundlicher NPC :) Da du noch keine Quests hast,\n folgendes ist zu beachten:  Also um neue Nebenmissionen (Quests) zu erhalten\n schaue bei mir vorbei.\n Wenn du eine Mission abgeschlossen hast werde ich dir die Belohnung geben.\n Viel Erfolg und hier dein erster Quest:\n\n";
                    else
                    {
                        texttodisplay += "Deine Quests:\n\n";
                        cur_palyer.UpdateQuests(); //update quests and get reward
                        foreach(Quest q in cur_palyer.GetQuests())
                        {
                            texttodisplay += q.GetDescription();
                            if(q.IsDone)
                                texttodisplay += " [ABGESCHLOSSEN]\n\n";
                            else 
                            {
                                texttodisplay += " [NICHT ABGESCHLOSSEN]\n\n";
                                queststodo++;
                            }
                        }
                    }
                    //hand out a new quest under special perconditions
                    if (queststodo < 3)
                    {
                        Quest nq = new Quest(Quest.QuestType.CollectItems, "TEST", 1000, 1);
                        cur_palyer.AddQuest(nq);
                        texttodisplay += "Neues Quest:\n\n" + nq.GetDescription() + " [NEU]\n";
                    }
                    GenericDialog(((Actor)data[1]).id, ((Actor)data[0]).id, texttodisplay);
                    break;

                case Backend.Events.MoveActor:
                    {
                        if ((data.Length > 1) && (_map.actors.Count >= (int)data[0]) && (!_map.actors[(int)data[0]].locked))
                        {
                            int id = (int)data[0];
                            Direction dir = (Direction)data[1];
                            Backend.Coords target = Map.DirectionTile(_map.actors[id].tile.coords, dir);
                            _map.actors[id].direction = dir;
                            _parent.HandleEvent(false, Backend.Events.MoveActor, id, _map.actors[id].tile.coords, dir);

                            if (((FloorTile)_map.actors[id].tile.parent).hasTrap)
                            {
                                if (((FloorTile)_map.actors[id].tile.parent).trap.status == TrapState.Disabled)
                                    ((FloorTile)_map.actors[id].tile.parent).trap.status = TrapState.NoDisplay;
                                // TODO: Update Tile on Client by event
                            }

                            Actor a = _map[target.x, target.y].firstActor;
                            if ((a is NPC) && (_map.actors[id] is Player))
                            {
                                (a as NPC).Interact(_map.actors[id]);
                            }
                            if (((a is Enemy || a is Player) && !(_map.actors[id] is NPC))
                                && ((a.id != id) && (!a.isDead)))
                            {
                                HandleEvent(true, Backend.Events.Attack, id, dir);
                                _map.actors[id].locked = true;

                            }
                            else
                            {
                                if ((_map[target].hasDoor) && (_map.actors[id] is Player) && (!_map[target].door.open))
                                {
                                    if (_map.actors[id].HasKey(_map.level))
                                    {
                                        _map[target].door.open = true;
                                        _parent.HandleEvent(false, Events.ShowMessage, "You open the door using the key you fought for.");
                                    }
                                    else
                                    {
                                        _parent.HandleEvent(false, Events.ShowMessage, "The door is locked.\n It is likely a strong creature guards the key.");
                                    }
                                }
                                if (_map.CanMove(_map.actors[id].tile.coords, dir))
                                {
                                    _map.MoveActor(_map.actors[id], dir);
                                    _parent.HandleEvent(false, Backend.Events.MoveActor, id, _map.actors[id].tile.coords, dir);
                                    _map.actors[id].locked = true;
                                }
                            }
                        }
                    }
                    break;

                case Backend.Events.Initialize:
                    ReassignPlayer();
                    break;

                case Backend.Events.Pause:
                    _paused = true;
                    _parent.HandleEvent(true, Backend.Events.Pause);
                    break;

                case Backend.Events.ContinueGame:
                    if (!DownStream)
                        _parent.HandleEvent(true, Backend.Events.ContinueGame, true);
                    _paused = false;
                    break;

                case Backend.Events.LoadFromCheckPoint:
                    HandleEvent(false, Backend.Events.Pause);
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
                    map.Load("savedroom" + lastCheck + ".xml", null, true);
                    map.actors[(int)data[0]].lives--;
                    map.Save("savedroom" + lastCheck + ".xml");
                    map.Save("checkpoint" + lastCheck + ".xml");
                    HandleEvent(false, Backend.Events.ContinueGame, true);
                    break;

                case Backend.Events.ChangeMap: // Load another map
                    HandleEvent(false, Backend.Events.Pause);
                    ChangeMap((string)data[0], (Coords)data[1]);
                    ReassignPlayer();
                    break;

                case Backend.Events.NewMap:
                    HandleEvent(false, Backend.Events.Pause);
                    GenerateMaps();
                    map.Load("room1.xml", null, true);
                    HandleEvent(false, Backend.Events.ContinueGame, true);
                    break;
                case Backend.Events.ResetGame:
                    _DeleteSavedRooms();
                    _map.Load("room1.xml", null, true);
                    HandleEvent(false, Events.ContinueGame, true);
                    break;
            }
        }


        #region Generator

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
            Backend.Coords CoordsFrom = rooms[From].SuggestExit(exit);
            Backend.Coords CoordsTo = rooms[To].SuggestExit(Map.OppositeDirection(exit));
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
        /// Generate three levels consisting of multiple rooms each and save them to xml files
        /// </summary>
        public override void GenerateMaps()
        {
            _DeleteSavedRooms();
            List<Generator> rooms = new List<Generator>();
            int maxLevel = 3;
            int LevelStart = 0;
            int prevTotal = 0;
            int prevLevelStart = 0;
            for (int level = 1; level < maxLevel + 1; ++level)  // Generate 3 levels (for now; possibly _random number of levels later)
            {
                LevelStart = rooms.Count();

                // Phase 1: Generate maze like rooms
                int totalRooms = _random.Next(10) + 3;
                string _name = null;
                for (int i = 0; i < totalRooms; ++i)
                {

                    rooms.Add(new Generator(this, 10 + _random.Next(8) + ((i == totalRooms) ? 5 : 0), 10 + _random.Next(8) + ((i == totalRooms) ? 5 : 0), true, null, LevelStart + i + 1, totalRooms, _random, _name, level));
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
                            Direction exit = (Direction)Math.Pow(2, _random.Next(4));
                            do
                            {
                                exit = (Direction)Math.Pow(2, _random.Next(4));
                            } while ((exitsPossible != Direction.None) && ((exit == Direction.None) || (!exitsPossible.HasFlag(exit))));
                            if ((exit != Direction.None) && (exitsPossible != Direction.None))
                                ConnectRooms(rooms, From, exit, roomsPerRow);
                            exitsPossible &= ~exit;
                        }
                        while ((exitsPossible != Direction.None) && _random.Next(100) > 80);
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
                        Direction exit = (Direction)Math.Pow(2, _random.Next(4));
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

                    int exit = _random.Next(totalRooms) + LevelStart;
                    while (rooms[exit].hasStairs)
                    {
                        exit = _random.Next(totalRooms) + LevelStart;
                    };

                    int entrance = _random.Next(prevTotal) + prevLevelStart;
                    while (rooms[entrance].hasStairs)
                    {
                        entrance = _random.Next(prevTotal) + prevLevelStart;
                    };
                    Backend.Coords entranceCoords = rooms[entrance].FindRoomForStairs;
                    Backend.Coords exitCoords = rooms[exit].FindRoomForStairs;
                    rooms[exit].AddStairs(exitCoords, entrance + 1, entranceCoords, true);
                    rooms[entrance].AddStairs(entranceCoords, exit + 1, exitCoords, false);
                    rooms[exit].AddShop();
                }

                int checkpoint = LevelStart + _random.Next(totalRooms);
                rooms[checkpoint].AddCheckpoint();
                for (int i = 0; i < _random.Next(3) + 1; ++i)
                {
                    int npc = LevelStart + _random.Next(totalRooms);
                    rooms[npc].AddNPC();
                }
                if (level == maxLevel)
                {
                    int exit = _random.Next(totalRooms) + LevelStart;
                    while (rooms[exit].hasStairs)
                    {
                        exit = _random.Next(totalRooms) + LevelStart;
                    };
                    Backend.Coords targetCoords = rooms[exit].FindRoomForStairs;
                    rooms[exit].AddTarget(targetCoords);
                }

                // Phase 5 Add Checkpoints, Shops and Boss Fights; cut out areas in maps and add appropriate "regions"

                // Phase 6 Add a few challenges: Switches, Doors, Illusionary and destructible walls, keys and locks

                // Phase 7 Add Other NPCs, environmental elements and Quest Items / Enemies / NPCs
                int boss = _random.Next(totalRooms) + LevelStart;
                while ((rooms[boss].hasStairs) || (boss == 0))
                {
                    boss = _random.Next(totalRooms) + LevelStart;
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
        #endregion

        /// <summary>
        /// The constructor for the logic behind the game
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="map">The map object which will contain the rooms</param>
        /// <param name="_random">A random</param>
        public PureLogic(IHandleEvent parent, Map map = null, Random _random = null)
            : base(parent, map, _random)
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
                path = path.Substring(0, path.IndexOf(Environment.NewLine));
            }
            if (File.Exists("saved" + (string)path))
                _map = new Map(this, "saved" + (string)path);
            else
                _map = new Map(this, (string)path);
        }
    }
}
