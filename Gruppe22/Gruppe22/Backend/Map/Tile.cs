using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{
    /// <summary>
    /// The types a tile can be.
    /// For example a wall or a teleporter.
    /// </summary>
    public enum TileType
    {
        Empty = 0,
        Wall,
        Trap,
        Enemy,
        Start,
        Target,
        Teleporter,
        Item,
        Checkpoint
    }

    /// <summary>
    /// An abstract class representing a generic tile (i.e. blank floor)
    /// </summary>
    public abstract class Tile : Backend.IHandleEvent
    {
        #region Private Fields

        protected object _parent = null;
        #endregion

        #region Public Fields

        /// <summary>
        /// The coordinates of the tile.
        /// Returns the coords of the tile "under" this tile isn't the lowest.
        /// The same principle for set.
        /// </summary>
        public Backend.Coords coords
        {
            get
            {
                if (_parent is FloorTile)
                    return (_parent as FloorTile).coords;
                if (_parent is Tile)
                    return (_parent as Tile).coords;
                return null;
            }
            set
            {
                if (_parent is FloorTile)
                    (_parent as FloorTile).coords = value;
                else
                    if (_parent is Tile)
                        (_parent as Tile).coords = value;
            }
        }

        /// <summary>
        /// The parent of a tile.
        /// e.g. the floortile below a actortile.
        /// </summary>
        public object parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Abstract update method.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Abstract method to save a tile in a XML file
        /// </summary>
        /// <param name="xmlw">the XmlWriter used for saving the file</param>
        public virtual void Save(XmlWriter xmlw)
        {
        }
        #endregion

        #region Constructors


        /// <summary>
        /// An empty constructor (setting default values)
        /// </summary>
        /// <param name="parent">The parent for a tile.</param>
        public Tile(object parent)
            : base()
        {
            _parent = parent;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DownStream"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (!DownStream)
                ((Backend.IHandleEvent)_parent).HandleEvent(DownStream, eventID, data);
        }
    }
}
