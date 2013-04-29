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
        private bool _canEnter;

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
