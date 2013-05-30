using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    public enum NotificationStyle
    {
        Default = 0
    }

    public class Notifications : UIElement
    {
        private List<Notification> _notifications = null;

        public void AddNotification(string Text = "")
        {

        }

        public void RemoveNotification(int ID = -1)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

        public override void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {
            base.HandleEvent(sender, eventID, data);
        }


        public override bool IsHit(int x, int y)
        {
            return base.IsHit(x, y);
        }



        public Notifications(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
        }
    }
    public class Notification : UIElement
    {
        #region Private Fields
        /// <summary>
        /// Counter determining how long to display the notification (default: 1000 milliseconds)
        /// </summary>
        private int _timeOut = 1000;
        /// <summary>
        /// Style of the notification (i.e. with transparent background, with text shadow, etc.)
        /// </summary>
        private NotificationStyle _style = NotificationStyle.Default;
        /// <summary>
        /// Current visibility of the notification (-1.0: hidden, but fading in; 1.0: hidden (and faded out), 0.0: visible)
        /// </summary>
        private float _visibility = -1.0f;
        /// <summary>
        /// Target opacity to fade to
        /// </summary>
        private float _vTarget = 0.0f;
        #endregion


        #region Public Methods
        /// <summary>
        /// Hide notification on mouse click
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="_lastCheck"></param>
        public override void OnMouseDown(int button)
        {
            if (_displayRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                _vTarget = 1.0f;
        }

        /// <summary>
        /// Handle fading in and out of Notification and timeout
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (_timeOut == 0)
            {
                _vTarget = 1.0f;
            }
            else
            {
                if (_timeOut > 0)
                {
                    _timeOut -= 1;
                }
            }
            if (_visibility == 1.0f)
            {
                _parent.HandleEvent(this, Events.HideNotification, 0);
            }
            else
            {
                if (Math.Abs(_vTarget - _visibility) > 0.05f)
                {
                    if (_visibility > _vTarget)
                    {
                        _visibility -= 0.05f;
                    }
                    else
                    {
                        _visibility += 0.05f;
                    }
                }
                else
                {
                    _visibility = _vTarget;
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Display the notification
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion


        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        /// <param name="text"></param>
        /// <param name="style"></param>
        /// <param name="timeout"></param>
        public Notification(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string text, NotificationStyle style = NotificationStyle.Default, int timeout = 1000)
            : base(parent, spriteBatch, content, displayRect)
        {

        }
        #endregion
    }
}
