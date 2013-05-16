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
        private Coords _coords = null;
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
        /// Determine whether tile can be entered
        /// </summary>
        public bool canEnter
        {
            get
            {
                for (int i = 0; i < _overlay.Count; ++i)
                {
                    if (_overlay[i] is WallTile) return false;
                }

                return true;
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
        /// Determine whether the current tile contains a "special" feature
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

        /// <summary>
        /// Determine whether there is a treasure on the current tile
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
        /// Add a (generic) tile of a specified type to overlay and return a pointer to that tile
        /// </summary>
        /// <param name="type"></param>
        public Tile Add(TileType type)
        {
            switch (type)
            {
                case TileType.Wall:
                    _overlay.Add(new WallTile(this));
                    break;
                case TileType.Trap:
                    _overlay.Add(new TrapTile(this, 1));
                    break;
                case TileType.Teleporter:
                    _overlay.Add(new TeleportTile(this, "ddd", new Coords(0, 0)));
                    break;
                case TileType.Target:
                    _overlay.Add(new TargetTile(this));
                    break;
                case TileType.Start:
                    _overlay.Add(new ActorTile(this));
                    break;
                case TileType.Item:
                    _overlay.Add(new ItemTile(this));
                    break;
                case TileType.Enemy:
                    _overlay.Add(new ActorTile(this));
                    break;
            }
            return _overlay[_overlay.Count - 1];
        }

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
        public void Add(Tile tile)
        {
            _overlay.Add(tile);
            tile.parent = this;
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
        /// You can always walk over a Floor
        /// </summary>
        public FloorTile(object parent)
            : base(parent)
        {
            _overlay = new List<Tile>();

        }


        /// <summary>
        /// An empty constructor (setting default values)
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
                Add(TileType.Wall);
            }
        }
        #endregion

        /// <summary>
        /// </summary>
        /// <returns>true if write is successful</returns>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("Tile");
            if (_coords != null)
            {
                xmlw.WriteAttributeString("CoordX", Convert.ToString(_coords.x));
                xmlw.WriteAttributeString("CoordY", Convert.ToString(_coords.y));
            }
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
