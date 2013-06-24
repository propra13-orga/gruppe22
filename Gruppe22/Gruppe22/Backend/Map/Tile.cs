using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{

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
        public Tile(object parent)
            : base()
        {
            _parent = parent;
        }
        #endregion

        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (!DownStream)
                ((Backend.IHandleEvent)_parent).HandleEvent(DownStream, eventID, data);
        }
    }
}
