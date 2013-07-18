using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile representing the floor of the world.
    /// Everything is build on the floor and actors also move on the floor.
    /// </summary>
    public class FloorTile : Tile, IDisposable
    {
        #region Private Fields

        protected List<Tile> _overlay;

        protected Backend.Coords _coords = null;

        /// <summary>
        /// Markiert ob dieses Bodenstück in der Minimap angezeigt wird oder nicht.
        /// </summary>
        private bool _visited = false;
        #endregion

        #region Public Fields
        /// <summary>
        /// A list with every tile above this FloorTile.
        /// </summary>
        public List<Tile> overlay
        {
            get
            {
                return _overlay;
            }
            set
            {
                _overlay = value;
            }
        }

        /// <summary>
        /// Method which returns a list of actors standing on this FloorTile.
        /// </summary>
        public List<Actor> actors
        {
            get
            {
                List<Actor> result = new List<Actor>();
                foreach (Tile tile in _overlay)
                {
                    if (tile is ActorTile) result.Add(((ActorTile)tile).actor);
                }
                return result;
            }
        }


        /// <summary>
        /// Method which returns a list of actors standing on this FloorTile.
        /// </summary>
        public TeleportTile teleportTile
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if (tile is TeleportTile) return (TeleportTile)tile;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ItemTile[] itemTiles
        {
            get
            {
                List<ItemTile> result = new List<ItemTile>();
                foreach (Tile tile in _overlay)
                {
                    if (tile is ItemTile) result.Add(((ItemTile)tile));
                }
                return result.ToArray();
            }
            set
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if (_overlay[i] is ItemTile)
                    {
                        _overlay.RemoveAt(i);
                        i -= 1;
                    }
                }
                foreach (ItemTile itemTile in value)
                {
                    itemTile.parent = this;
                }
                _overlay.AddRange(value);
            }
        }

        /// <summary>
        /// Returns an array of items laying on this FloorTile.
        /// </summary>
        public List<Item> items
        {
            get
            {
                List<Item> result = new List<Item>();
                foreach (Tile tile in _overlay)
                {
                    if (tile is ItemTile) result.Add(((ItemTile)tile).item);
                }
                return result;
            }
        }

        /// <summary>
        /// If there is a trap on this FloorTile the method returns the lowest TrapTile.
        /// Otherweise returns null.
        /// </summary>
        public TrapTile trap
        {
            get
            {

                foreach (Tile tile in _overlay)
                {
                    if (tile is TrapTile) return (TrapTile)tile;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns a CheckpointTile if there is one above this FloorTile.
        /// Otherwise null.
        /// </summary>
        public CheckpointTile checkpoint
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if (tile is CheckpointTile) return (CheckpointTile)tile;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns a TeleportTile if there is one above this FloorTile.
        /// Otherwise null.
        /// </summary>
        public TeleportTile teleport
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if (tile is TeleportTile) return tile as TeleportTile;
                }
                return null;
            }
        }

        /// <summary>
        /// If there is a enabled ActorTile above this FloorTile the method returns the associated Actor.
        /// Otherwise null.
        /// </summary>
        public Actor firstActor
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if ((tile is ActorTile) && (((ActorTile)tile).enabled) && (!(((ActorTile)tile).actor.actorType != ActorType.Player) || ((ActorTile)tile).actor.online) && (!((ActorTile)tile).actor.isDead))
                        return ((ActorTile)tile).actor;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the lowest ReservedTile above this FloorTile.
        /// If there is none the method returns null.
        /// </summary>
        public ReservedTile reserved
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if ((tile is ReservedTile) && (((ReservedTile)tile).enabled))
                        return ((ReservedTile)tile);
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the first DoorTile on this FloorTile.
        /// If there is none the method returns null.
        /// </summary>
        public DoorTile door
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if (tile is DoorTile)
                        return ((DoorTile)tile);
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the lowest ItemTile on this Floortile.
        /// </summary>
        public ItemTile firstItem
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if (tile is ItemTile)
                        return ((ItemTile)tile);
                }
                return null;
            }
        }

        /// <summary>
        /// The coordinates of the FloorTile.
        /// </summary>
        public new Backend.Coords coords
        {
            get
            {
                return _coords;
            }
            set
            {
                _coords = value;
            }
        }

        /// <summary>
        /// Determines whether there is a checkpoint on this Floortile.
        /// </summary>
        public bool hasCheckpoint
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if (_overlay[i] is CheckpointTile) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Determines whether the tile can be entered by checking the overlay for blocking tiles.
        /// </summary>
        public bool canEnter
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if ((_overlay[i] is WallTile && !(_overlay[i] is DoorTile)) || (_overlay[i] is GapTile) || ((_overlay[i] is DoorTile) && ((DoorTile)_overlay[i]).open == false)
                        || ((_overlay[i] is ReservedTile) && ((ReservedTile)_overlay[i]).canEnter == false)
                        ) return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Determines whether there is a wall on this Floortile.
        /// </summary>
        public bool hasWall
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if ((_overlay[i] is WallTile) || (_overlay[i] is DoorTile)) return true;
                }

                return false;
            }
        }


        /// <summary>
        /// Returns the type of a wall on this FloorTile.
        /// </summary>
        public WallType wallType
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if (_overlay[i] is DoorTile) return (_overlay[i] as DoorTile).type;
                    if (_overlay[i] is WallTile) return (_overlay[i] as WallTile).type;
                }

                return Backend.WallType.Normal;
            }
        }

        /// <summary>
        /// Determines whether there is a door on this tile.
        /// </summary>
        public bool hasDoor
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if (_overlay[i] is DoorTile) return true;
                }

                return false;
            }
        }


        /// <summary>
        /// Determines whether this FloorTile has the style of a GapTile.
        /// </summary>
        public int floorStyle
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if (_overlay[i] is GapTile) return ((GapTile)_overlay[i]).style;
                }

                return 1;
            }
        }
        /// <summary>
        /// Determines whether there is a player on this tile.
        /// </summary>
        public bool hasPlayer
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).actor is Player) && (((ActorTile)_overlay[count]).actor.online) && (!((ActorTile)_overlay[count]).actor.isDead));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determines whether there is a NPC on this tile.
        /// </summary>
        public bool hasNPC
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).enabled) && (((ActorTile)_overlay[count]).actor is NPC) && (!((ActorTile)_overlay[count]).actor.isDead));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determines whether there is a enemy on this tile.
        /// </summary>
        public bool hasEnemy
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).enabled) && (((ActorTile)_overlay[count]).actor is Enemy)
                        && (!((ActorTile)_overlay[count]).actor.isDead));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determines whether there is a TeleportTile on this floor.
        /// </summary>
        public bool hasTeleport
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is TeleportTile));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determines whether there is a TrapTile on this floor.
        /// </summary>
        public bool hasTrap
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = (_overlay[count] is TrapTile);
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determines whether there is a item on this floor.
        /// </summary>
        public bool hasTreasure
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = (_overlay[count] is ItemTile);
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Bool to determine whether the tile is visible for the player.
        /// </summary>
        public bool visible
        {
            get
            {
                return _visited;
            }
            set
            {
                _visited = value;
            }
        }

        /// <summary>
        /// Determines whether there is the goal on this tile.
        /// </summary>
        public bool hasTarget
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = (_overlay[count] is TargetTile);
                    ++count;
                }
                return result;
            }
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Update der interaktiven Tiles.
        /// </summary>
        /// <param name="gameTime">Hilfsobjekt.</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _overlay.Count; ++i)
            {
                _overlay[i].Update(gameTime);
            }
        }

        /// <summary>
        /// Lösche alle Tiles des angegebenen Types aus dem overlay
        /// </summary>
        /// <param name="type">Der Typ der Tiles die gelöscht werden sollen.</param>
        public void Remove(TileType type)
        {
            for (int i = 0; i < _overlay.Count; ++i)
                switch (type)
                {
                    case TileType.Checkpoint:
                        if (_overlay[i] is CheckpointTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Wall:
                        if (_overlay[i] is WallTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Trap:
                        if (_overlay[i] is TrapTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Teleporter:
                        if (_overlay[i] is TeleportTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Target:
                        if (_overlay[i] is TargetTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Start:
                        if (_overlay[i] is ActorTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Item:
                        if (_overlay[i] is ItemTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                    case TileType.Enemy:
                        if (_overlay[i] is ActorTile)
                        {
                            _overlay[i].parent = null;
                            _overlay.RemoveAt(i);
                            i -= 1;
                        }
                        break;
                }
        }


        /// <summary>
        /// Fügt den angegebenen Tile zu dem overlay hinzu. 
        /// </summary>
        /// <param name="tile">Das Tile das hinzugefügt wird.</param>
        /// <param name="update"></param>
        public void Add(Tile tile, bool update = false)
        {
            _overlay.Add(tile);
            tile.parent = this;
            if (update)
            {
                if (!((Map)_parent).updateTiles.Contains(_coords))
                    ((Map)_parent).updateTiles.Add(_coords);
            }
        }



        /// <summary>
        /// Löscht den angegebenen Tile aus dem overlay.
        /// </summary>
        /// <param name="tile"></param>
        public void Remove(Tile tile)
        {
            tile.parent = null;
            _overlay.Remove(tile);
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Einfacher Konstruktor.
        /// </summary>
        /// <param name="parent">Elternobjekt.</param>
        public FloorTile(object parent)
            : base(parent)
        {
            _overlay = new List<Tile>();

        }

        /// <summary>
        /// Dieser Konstruktor fürgt dem overlay ein WallTile hinzu, falls die Stelle als unpassierbar markiert ist 
        /// </summary>
        /// <param name="parent">Elternobjekt.</param>
        /// <param name="coords">Koordinaten.</param>
        /// <param name="canEnter">Passierbarkeit.</param>
        public FloorTile(object parent, Backend.Coords coords = null, bool canEnter = true)
            : this(parent)
        {
            if (coords != null)
            {
                _coords = coords;
            }
            if (!canEnter)
            {
                Add(new WallTile(this));
            }
        }

        #endregion

        /// <summary>
        /// Method to save the FloorTile and every tile in it's overlay in a XML-file
        /// </summary>
        /// <param name="xmlw">XMLwriter</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("Tile");
            if (_coords != null)
            {
                xmlw.WriteAttributeString("CoordX", Convert.ToString(_coords.x));
                xmlw.WriteAttributeString("CoordY", Convert.ToString(_coords.y));
            }
            xmlw.WriteAttributeString("visited", Convert.ToString(_visited));

            foreach (Tile tile in _overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
        }

        /// <summary>
        /// Aufräumen nachdem die Objekte verworfen werden können.
        /// </summary>
        public void Dispose()
        {
            _overlay.Clear();

        }
    }
}
