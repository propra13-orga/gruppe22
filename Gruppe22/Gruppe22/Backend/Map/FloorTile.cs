using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Xna.Framework;

namespace Gruppe22
{
    public class FloorTile : Tile, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Fields displayed (and checked) on top of the current field
        /// </summary>
        protected List<Tile> _overlay;

        /// <summary>
        /// The Position of the tile
        /// </summary>
        protected Coords _coords = null;

        /// <summary>
        /// Whether field is visible on the minimap
        /// </summary>
        private bool _visited = false;
        #endregion

        #region Public Fields
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

        public Actor firstActor
        {
            get
            {
                foreach (Tile tile in _overlay)
                {
                    if ((tile is ActorTile) && (((ActorTile)tile).enabled))
                        return ((ActorTile)tile).actor;
                }
                return null;
            }
        }

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

        public new Coords coords
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
        /// Determine whether tile has a checkpoint on it
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
        /// Determine whether tile can be entered
        /// </summary>
        public bool canEnter
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if ((_overlay[i] is WallTile) || (_overlay[i] is GapTile) || ((_overlay[i] is DoorTile) && ((DoorTile)_overlay[i]).open == false)) return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Determine whether tile has a wall on it
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

        public WallType wallType
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if ((_overlay[i] is WallTile) || (_overlay[i] is DoorTile)) return (_overlay[i] as WallTile).type;
                }

                return WallType.Normal;
            }
        }

        /// <summary>
        /// Determine whether tile has a door on it
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
        /// Determine whether tile has a special floor style (empty space, fire, water, road, grass, rocks, etc.)
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
        /// Determine whether a player is standing on the current tile
        /// </summary>
        public bool hasPlayer
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).actor is Player));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determine whether an enemy is standing on the current tile
        /// </summary>
        public bool hasNPC
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).enabled) && (((ActorTile)_overlay[count]).actor is NPC));
                    ++count;
                }
                return result;
            }
        }
        /// <summary>
        /// Determine whether an enemy is standing on the current tile
        /// </summary>
        public bool hasEnemy
        {
            get
            {
                bool result = false;
                int count = 0;
                while ((!result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).enabled) && (((ActorTile)_overlay[count]).actor is Enemy));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determine whether the current tile contains a teleporter
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
        /// Determine whether the current tile has a trap on it
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
        /// Determine whether there is a treasure on the current tile
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
        /// Determine whether the current tile is the 'end' of the game
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
        /// Refresh tiles which do something (traps, enemies, NPCs)
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _overlay.Count; ++i)
            {
                _overlay[i].Update(gameTime);
            }
        }

        /// <summary>
        /// Remove all tiles of a specified type from overlay
        /// </summary>
        /// <param name="type"></param>
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
        /// Add specified tile to overlay
        /// </summary>
        /// <param name="tile"></param>
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
        /// Remove specified tile from overlay
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
        /// A empty constructor
        /// </summary>
        public FloorTile(object parent)
            : base(parent)
        {
            _overlay = new List<Tile>();

        }

        /// <summary>
        /// A constructor adding a walltile to the overlay if the contructed tile is impassable
        /// </summary>
        public FloorTile(object parent, Coords coords = null, bool canEnter = true)
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
        /// Save the Floortile and every tile in it's overlay
        /// </summary>
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
        /// Clean up Tile
        /// </summary>
        public void Dispose()
        {
            _overlay.Clear();

        }
    }
}
