using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Client
{
    /// <summary>
    /// An interface to emulate Windows Forms style key and mouse events
    /// </summary>
    public interface IKeyHandler
    {
        /// <summary>
        /// Called whenever a key was up and is now down
        /// </summary>
        /// <param name="k">The key which was pressed</param>
        bool OnKeyDown(Keys k);

        /// <summary>
        /// Called whenever a key was down and is now up 
        /// </summary>
        /// <param name="k">The key which was pressed</param>
        bool OnKeyUp(Keys k);


        /// <summary>
        /// Called when a mouse button changes from up to down
        /// </summary>
        /// <param name="button">Left Button=1, Middle Button=2, Right Button=3</param>
        bool OnMouseDown(int button);

        /// <summary>
        /// Called when a mouse button changes from down to up
        /// </summary>
        /// <param name="button">Left Button=1, Middle Button=2, Right Button=3</param>
        bool OnMouseUp(int button);

        bool OnMouseHeld(int button);

        bool OnKeyHeld(Keys k);
    }

    /// <summary>
    /// A class transforming mouse and keyboard-states to Windows Forms style events
    /// </summary>
    public class StateToEvent
    {
        #region Private Fields
        /// <summary>
        /// A list of keys currently pressed
        /// </summary>
        private Keys[] _pressed = null;
        /// <summary>
        /// A reference to a parent to which events will be passed
        /// </summary>
        private IKeyHandler _parent = null;
        /// <summary>
        /// A timer used to avoid over-committing events
        /// </summary>
        private bool _working = true;
        /// <summary>
        /// Current state of left mouse button
        /// </summary>
        private bool _leftButton = false;
        /// <summary>
        /// Current state of right mouse button
        /// </summary>
        private bool _rightButton = false;
        /// <summary>
        /// Current state of middle mouse button
        /// </summary>
        private bool _middleButton = false;
        /// <summary>
        /// Counter when keyboard was last checked for input
        /// </summary>
        private int _lastCheck = 0;
        #endregion

        #region Public Fields
        public Keys[] keys
        {
            get
            {
                return _pressed;
            }
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Change states and pass events if needed
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (!_working) // Avoid conflicting access (also: save time)
            {
                _working = true;

                // 1. Mouse handling

                if (_leftButton == true)
                {
                    if (Mouse.GetState().LeftButton != ButtonState.Pressed)
                    {
                        _parent.OnMouseUp(1);
                        _leftButton = false;
                        _lastCheck = 0;
                    }
                    else
                    {
                        if (_lastCheck == 2)
                        {
                            _parent.OnMouseHeld(1);
                        }
                    }
                }
                else
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        _parent.OnMouseDown(1);
                        _leftButton = true;
                        _lastCheck = 0;
                    }
                }

                if (_middleButton == true)
                {
                    if (Mouse.GetState().MiddleButton != ButtonState.Pressed)
                    {
                        _parent.OnMouseUp(2);
                        _middleButton = false;
                        _lastCheck = 0;
                    }
                    else
                    {
                        if (_lastCheck == 2)
                        {
                            _parent.OnMouseHeld(2);
                        }
                    }
                }
                else
                {
                    if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                    {
                        _parent.OnMouseDown(2);
                        _middleButton = true;
                        _lastCheck = 0;
                    }
                }

                if (_rightButton == true)
                {
                    if (Mouse.GetState().RightButton != ButtonState.Pressed)
                    {
                        _parent.OnMouseUp(3);
                        _rightButton = false;
                        _lastCheck = 0;
                    }
                    else
                    {
                        if (_lastCheck == 2)
                        {
                            _parent.OnMouseHeld(3);
                        }
                    }
                }
                else
                {
                    if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        _parent.OnMouseDown(3);
                        _rightButton = true;
                        _lastCheck = 0;
                    }
                }


                // 2. Keyboard handling

                Keys[] current = Keyboard.GetState().GetPressedKeys();
                foreach (Keys key in _pressed)
                {
                    if (!current.Contains(key))
                    {
                        _parent.OnKeyUp(key);

                        _lastCheck = 0;
                    }                    
                }
                foreach (Keys key in current)
                {
                    if (!_pressed.Contains(key))
                    {
                        _parent.OnKeyDown(key);
                        _lastCheck = 0;
                    }
                    else
                    {
                        if (_lastCheck == 2)
                        {
                            _parent.OnKeyHeld(key);
                        }
                    }
                }
                _pressed = current; // Save current state
                _working = false;
                if ((int)(gametime.TotalGameTime.Milliseconds / 10) == 1)
                {
                    _lastCheck += 1;
                }
                if (_lastCheck > 2)
                {
                    _lastCheck = 0;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new State to event transformer (possibly only for mainwindow)
        /// </summary>
        /// <param name="parent"></param>
        public StateToEvent(IKeyHandler parent)
        {
            _parent = parent;
            _pressed = Keyboard.GetState().GetPressedKeys();
            _leftButton = (Mouse.GetState().LeftButton == ButtonState.Pressed);
            _middleButton = (Mouse.GetState().MiddleButton == ButtonState.Pressed);
            _rightButton = (Mouse.GetState().RightButton == ButtonState.Pressed);
            _working = false;
        }
        #endregion
    }
}
