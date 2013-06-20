using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    /// <summary>
    /// Stellt einen Service zur Verfügung, der zeitgesteuerte Nachrichten anzeigt
    /// </summary>
    public class SimpleTextOutput : DrawableGameComponent
    {
        private SpriteFont _font;
        private SpriteBatch _sb;

        public List<SimpleTextMessage> Messages { get; private set; }

        public SimpleTextOutput(Game game)
            : base(game)
        {
            game.Components.Add(this);

            Messages = new List<SimpleTextMessage>();
            _sb = new SpriteBatch(Game.GraphicsDevice);

            _font = Game.Content.Load<SpriteFont>("font");
        }

        public void ShowMessage(SimpleTextMessage message)
        {
            if (!Messages.Contains(message))
                Messages.Add(message);
        }

        public void ShowMessage(string text, float duration = 6f, Color? color = null)
        {
            Messages.Add(new SimpleTextMessage(text, duration, color));
        }

        public void RemoveMessage(SimpleTextMessage message)
        {
            Messages.Remove(message);
        }


        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            List<SimpleTextMessage> _removeList = new List<SimpleTextMessage>();

            foreach (SimpleTextMessage message in Messages)
            {
                if (message.Elapsed >= message.Duration)
                    _removeList.Add(message);
                else
                    message.Elapsed += delta;
            }

            foreach (SimpleTextMessage message in _removeList)
                Messages.Remove(message);


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Messages.Count == 0) return;

            Vector2 startPos = new Vector2(10, 10);
            _sb.Begin();

            foreach (SimpleTextMessage message in Messages)
            {
                _sb.DrawString(_font, message.Text, startPos, message.Color);
                startPos.Y += _font.MeasureString(message.Text).Y;
            }

            _sb.End();

            base.Draw(gameTime);
        }
    }

    /// <summary>
    /// Eine Nachricht die für bestimmte Zeit angezeigt werden soll.
    /// </summary>
    public class SimpleTextMessage
    {
        public SimpleTextMessage(string text, float duration = 3f, Color? color = null)
        {
            Text = text;
            Duration = duration;
            Color = color.HasValue ? color.Value : Color.Black;
        }

        public string Text { get; set; }
        public float Duration { get; set; }
        public Color Color { get; set; }
        internal float Elapsed { get; set; }
    }
}
