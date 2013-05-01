using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Gruppe22 
{
    /// <summary>
    /// An abstract class representing a generic tile (i.e. blank floor)
    /// </summary>
    public class Tile : IDisposable
    {
        #region Delegates
        public delegate void OnEnter();
        #endregion

        #region Private Fields
        /// <summary>
        /// Fields displayed (and checked) on top of the current field
        /// </summary>
        private List<Tile> _overlay;
        /// <summary>
        /// Internal value whether tile can be entered
        /// </summary>
        private bool _canEnter=true;

        #endregion

        #region Public Fields

        /// <summary>
        /// An event handler for entering the field
        /// </summary>
        public OnEnter onenter;

        /// <summary>
        /// Determine whether tile can be entered
        /// </summary>
        public bool canEnter
        {
            get
            {
                bool result = _canEnter;
                int count = 0;
                while ((result) && (count < _overlay.Count))
                {
                    result = _overlay[count].canEnter;
                    ++count;
                }
                return result;
            }
            set
            {
                _canEnter = value;
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
                while ((result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile)&&(((ActorTile)_overlay[count]).actorType==ActorType.Player));
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
                while ((result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ActorTile) && (((ActorTile)_overlay[count]).actorType == ActorType.Enemy));
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
                while ((result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is TeleportTile));
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
                while ((result) && (count < _overlay.Count))
                {
                    result = ((_overlay[count] is ItemTile) && (((ItemTile)_overlay[count]).itemType == ItemType.Treasure));
                    ++count;
                }
                return result;
            }
        }

        /// <summary>
        /// Determine whether the current tile contains a "special" feature
        /// </summary>
        public bool hasSpecial
        {
            get
            {
                bool result = false;                
                return result;
            }
        }
        #endregion

        #region Public methods

        
        /// <summary>
        /// Write Tile data to an XML-file
        /// </summary>
        /// <param name="file">An XMLTextWriter containing data for the tile</param>
        /// <returns>true if write is successful</returns>
        public bool Save(XmlTextWriter target)
        {
            throw new NotImplementedException("Das muss noch jemand machen");
            return true;
        }

        /// <summary>
        /// Read Tile data from an XML-file
        /// </summary>
        /// <param name="file">An XMlTextreader containing data for the tile</param>
        /// <returns>true if read is successful</returns>
        public bool Load(XmlTextReader source)
        {
            throw new NotImplementedException("Das muss noch jemand machen");
            return true;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// An empty constructor (setting default values)
        /// </summary>
        public Tile()
            : base()
        {
            _overlay = new List<Tile>();
        }

        /// <summary>
        /// Clean up Tile
        /// </summary>
        public void Dispose()
        {
            _overlay.Clear();
            
        }
        #endregion
    }
}
