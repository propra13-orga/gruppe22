using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Gruppe22
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
        Item
    }

    /// <summary>
    /// An abstract class representing a generic tile (i.e. blank floor)
    /// </summary>
    public abstract class Tile : IHandleEvent
    {
        #region Private Fields



        protected object _parent = null;
        #endregion

        #region Public Fields

        public Coords coords
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
        /// </summary>
        /// <returns>true if write is successful</returns>
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

        public virtual void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {
            ((IHandleEvent)_parent).HandleEvent(sender, eventID, data);
        }
    }
}
