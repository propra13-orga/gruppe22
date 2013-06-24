using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// Different types of traps
    /// </summary>
    [Flags]
    public enum TrapType
    {
        None = 0,
        Changing = 1,
        /// <summary>
        /// Trap only damages once, becomes disabled otherwiste
        /// </summary>
        OnlyOnce = 2,
        /// <summary>
        /// Invisible trap (sprung on first enter of field, then changing to Always)
        /// </summary>
        Hidden = 4
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
        Disabled = 3,
        NoDisplay = 5

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
        /// Type of trap
        /// </summary>
        private TrapType _type = TrapType.None;

        /// <summary>
        /// Time elapsed
        /// </summary>
        private uint _elapsed = 0;

        /// <summary>
        /// Cut through block
        /// </summary>
        private int _penetrate = 0;

        /// <summary>
        /// Overrule player evade
        /// </summary>
        private int _evade = 0;

        /// <summary>
        /// Repeat timer (for changing traps)
        /// </summary>
        private uint _repeatTime = 1600;
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

        public int evade
        {
            get
            {
                return _evade;
            }
            set
            {
                _evade = value;
            }
        }

        public int penetrate
        {
            get
            {
                return _penetrate;
            }
            set
            {
                _penetrate = value;
            }
        }
        public TrapType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
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

        public bool visible
        {
            get
            {
                return (((_type & TrapType.Hidden) != TrapType.Hidden)
                    && (_state == TrapState.On));
            }
        }

        #endregion

        #region Public Methods
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            if (((_type & TrapType.Changing) == TrapType.Changing) && (_state != TrapState.Destroyed) && (_state != TrapState.NoDisplay) && (_state != TrapState.Disabled) && ((_type & TrapType.Hidden) != TrapType.Hidden))
            {
                _elapsed += (uint)gameTime.ElapsedGameTime.Milliseconds;
                if (_elapsed > _repeatTime)
                {
                    _elapsed -= _repeatTime;
                    if (_state == TrapState.Off)
                    {
                        _state = TrapState.On;
                        ((FloorTile)_parent).HandleEvent(false, Backend.Events.TrapActivate, coords);
                    }
                    else _state = TrapState.Off;
                }
            }
        }



        public int Trigger()
        {
            if (_state == TrapState.On)
            {
                if ((_type & TrapType.OnlyOnce) == TrapType.OnlyOnce)
                {
                    _state = TrapState.Disabled; // Trap was sprung and will be disabled
                }
                if ((_type & TrapType.Hidden) == TrapType.Hidden)
                {
                    _type &= ~TrapType.Hidden; // Trap was hidden and now becomes visible
                }
                return _damage;
            }
            return 0;
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
            xmlw.WriteAttributeString("evade", Convert.ToString(_evade));
            xmlw.WriteAttributeString("penetrate", Convert.ToString(_penetrate));

            switch (_state)
            {
                case TrapState.Disabled:
                    xmlw.WriteAttributeString("disabled", "true");
                    break;
                case TrapState.Destroyed:
                    xmlw.WriteAttributeString("broken", "true");
                    break;
                case TrapState.NoDisplay:
                    xmlw.WriteAttributeString("invisible", "true");
                    break;
            }

            if ((_type & TrapType.Hidden) == TrapType.Hidden)
            {
                xmlw.WriteAttributeString("hidden", "true");

            }
            if ((_type & TrapType.Changing) == TrapType.Changing)
            {
                xmlw.WriteAttributeString("changing", "true");

            }
            if ((_type & TrapType.OnlyOnce) == TrapType.OnlyOnce)
            {
                xmlw.WriteAttributeString("onlyonce", "true");

            }
            xmlw.WriteEndElement();
        }
        #endregion
    }
}
