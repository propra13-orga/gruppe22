using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    /// <summary>
    /// Different types of traps
    /// </summary>
    public enum TrapType
    {
        Always = 0,
        /// <summary>
        /// 
        /// </summary>
        Changing = 1,
        /// <summary>
        /// Trap only damages once, becomes disabled otherwiste
        /// </summary>
        OnlyOnce = 2,
        /// <summary>
        /// Invisible trap (sprung on first enter of field, then changing to Always)
        /// </summary>
        Hidden = 3,
        /// <summary>
        /// One hit kill
        /// </summary>
        Deadly = 4
    }
    /// <summary>
    /// Determines the State of a trap
    /// </summary>
    public enum TrapState
    {
        /// <summary>
        /// Trap is destroyed and will not be re-enabled 
        /// </summary>
        Destroyed = 0,
        /// <summary>
        /// Trap is enabled and doing damage
        /// </summary>
        On = 1,
        /// <summary>
        /// Trap is enabled, but not doing damage
        /// </summary>
        Off = 2,
        /// <summary>
        /// Trap is disabled and can only be re-enabled manually
        /// </summary>
        Disabled = 3
    }
    /// <summary>
    /// A tile containing a trap
    /// </summary>
    public class TrapTile : Tile
    {
        #region Private Fields
        /// <summary>
        /// the damage a trap deals to the player
        /// </summary>
        private int _damage = 0;

        /// <summary>
        /// Current status of trap
        /// </summary>
        private TrapState _state = TrapState.On;

        /// <summary>
        /// Interval when trap state is changed
        /// </summary>
        private int _interval = -1;

        /// <summary>
        /// Type of trap
        /// </summary>
        private TrapType _type=TrapType.Always;
        #endregion

        #region Public Fields
        /// <summary>
        /// Determine damage trap does right now (0 if destroyed, off or disabled)
        /// </summary>
        public int damage
        {
            get
            {
                if (_state == TrapState.On)
                    return _damage;
                else
                    return 0;
            }
            set
            {
                _damage = value;
            }
        }

        /// <summary>
        /// Current status of the trap
        /// </summary>
        public TrapState status
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// A Constructor setting the damage of the trap
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dmg">The damage the trap will deal</param>
        public TrapTile(object parent, int dmg)
            : base(parent)
        {
            _damage = dmg;
        }
        #endregion

        #region Public Methods
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TrapTile");
            xmlw.WriteAttributeString("damage", Convert.ToString(_damage));

            xmlw.WriteEndElement();
        }
        #endregion
    }
}
