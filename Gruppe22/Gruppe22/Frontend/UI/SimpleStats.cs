using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
{
    public class SimpleStats : UIElement
    {
        #region Private Fields
        private SpriteFont _font;
        private ProgressBar _manaBar;
        private ProgressBar _healthBar;
        private Actor _actor;
        private int _lineheight;
        private Texture2D _background;

        #endregion

        #region Public Fields
        public Actor actor
        {
            get
            {
                return _actor;
            }
            set
            {
                _actor = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// React to changes in character stats
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Update Health bar
            if (_actor != null)
            {
                if (_actor.health != _healthBar.value)
                {
                    _healthBar.value = _actor.health;
                }
                if (_actor.maxHealth != _healthBar.total)
                {
                    _healthBar.total = _actor.maxHealth;
                }

                if (_actor.currMana != _manaBar.value)
                {
                    _manaBar.value = _actor.currMana;
                }
                if (_actor.maxMana != _manaBar.total)
                {
                    _manaBar.total = _actor.maxMana;
                } 
            }
        }
        /// <summary>
        /// Refresh display of actor data
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_actor != null)
            {
                _spriteBatch.Begin();
                Color color = Color.White;
                if (_actor is Player) color = Color.Green; // Player in Green
                if (_actor is Enemy) color = Color.Red; // Opponent in red

                // Character Name
                _spriteBatch.DrawString(_font, _actor.name.ToString(), new Vector2(_displayRect.Left + 9, _displayRect.Top), Color.White);
                _spriteBatch.DrawString(_font, _actor.name.ToString(), new Vector2(_displayRect.Left + 10, _displayRect.Top + 1), color);

                // Separator Line
                // _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 5, _displayRect.Top + _lineheight -3, _displayRect.Width - 10, 2), new Rectangle(39, 6, 1, 1), color);



                // Statistics
                _spriteBatch.DrawString(_font, "ATK: " + _actor.damage.ToString() + " - DEF:" + _actor.armour.ToString(), new Vector2(_displayRect.Left + 10, _displayRect.Top + _lineheight * 3 + 8), color);

                // Additional Data for player: Experience
                if (_actor is Player)
                {
                    _spriteBatch.DrawString(_font, "LVL: " + _actor.level.ToString() + " - EXP to next LVL:" + _actor.exp.ToString(), new Vector2(_displayRect.Left + 10, _displayRect.Top + _lineheight * 4 + 8), color);
                }
                _spriteBatch.End();
                // Health bar and Mana bar
                _healthBar.Draw(gameTime);
                _manaBar.Draw(gameTime);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SimpleStats(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor)
            : base(parent, spriteBatch, content, displayRect)
        {
            _font = _content.Load<SpriteFont>("SmallFont");
            _actor = actor;
            _lineheight = (int)(_font.MeasureString("WgjITt").Y);
            _healthBar = new ProgressBar(this, _spriteBatch, _content, new Rectangle(
_displayRect.Left + 10, _displayRect.Top + _lineheight, _displayRect.Width - 20, _lineheight + 4), ProgressStyle.Precise, (actor != null) ? actor.maxHealth : 0, (actor != null) ? actor.health : 0);
            _healthBar.color = Color.Red;
            _manaBar = new ProgressBar(this, _spriteBatch, _content, new Rectangle(
_displayRect.Left + 10, _displayRect.Top + 2 * _lineheight, _displayRect.Width - 20, _lineheight + 4), ProgressStyle.Precise, (actor != null) ? actor.maxMana : 0, (actor != null) ? actor.currMana : 0); //TODO: Mana public fields
            _manaBar.color = Color.Blue;
            _background = _content.Load<Texture2D>("Minimap");

        }
        #endregion
    }
}
