using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    public class AbilityChoice : Grid
    {
        private int _width = 163;
        private Random _random;
        private Actor _actor;
        private Ability[] _abilities;
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

        public override bool OnMouseDown(int button)
        {
            int clicked = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
            if (clicked != -1)
            {
                if (_abilities[clicked].improveOver != -1)
                {
                    _actor.abilities[_abilities[clicked].improveOver] = _abilities[clicked];
                    GenerateAbility(0);

                    GenerateAbility(1);
                    GenerateAbility(2);

                } else {
                    _actor.abilities.Add(_abilities[clicked]);
                }
                GenerateAbility(clicked);
                _parent.HandleEvent(false, Events.ButtonPressed, Buttons.Reset);
                return true;
            }
            return false;
        }

        public override int Pos2Tile(int x, int y)
        {
            int result = -1;
            if (_displayRect.Contains(x, y))
            {
                x -= _displayRect.Left;
                x = x / (_width + 3);
                result = x;
            }
            return result;
        }

        public void GenerateAbility(int id)
        {
            AbilityElement element = (AbilityElement)Math.Pow(2, _random.Next(10));
            AbilityTarget target = AbilityTarget.None;
            switch (element)
            {
                case AbilityElement.ManaReg:
                case AbilityElement.Health:
                case AbilityElement.HealthReg:
                    target = AbilityTarget.Self;
                    break;

                case AbilityElement.Fire:
                case AbilityElement.Charm:
                case AbilityElement.Scare:
                case AbilityElement.Stun:
                case AbilityElement.Ice:
                    switch (_random.Next(4))
                    {
                        case 0:
                            target = AbilityTarget.Explode;
                            break;
                        case 1:
                            target = AbilityTarget.Missile;
                            break;
                        case 2:
                            target = AbilityTarget.Aura;
                            break;
                        case 3:
                            target = AbilityTarget.Map;
                            break;
                    }
                    break;
                case AbilityElement.Morph:

                    switch (_random.Next(5))
                    {
                        case 0:
                            target = AbilityTarget.Self;
                            break;
                        case 1:
                            target = AbilityTarget.Explode;
                            break;
                        case 2:
                            target = AbilityTarget.Missile;
                            break;
                        case 3:
                            target = AbilityTarget.Aura;
                            break;
                        case 4:
                            target = AbilityTarget.Map;
                            break;
                    }

                    break;
                case AbilityElement.Teleport:
                    target = AbilityTarget.Map;

                    break;

            }
            int intensity = _random.Next(10) + 2;
            int duration = 1;
            if (
                (element == AbilityElement.HealthReg)
                || (element == AbilityElement.Morph)
                || (element == AbilityElement.Charm)
                || (element == AbilityElement.Scare)
                || (element == AbilityElement.Stun)
                || (element == AbilityElement.ManaReg)
                || (element == AbilityElement.HealthReg))
                duration = _random.Next(5) + 5;
            int cooldown = 10 + _random.Next(10);
            int cost = 5 + intensity * duration + _random.Next(10) + (((target == AbilityTarget.Map) || (target == AbilityTarget.Explode)) ? 5 : 0);
            int improveOver = -1;
            string name = "";
            string description = "";
            // Check existing abilities for similarities and suggest improvement when found
            for (int i = 0; i < _actor.abilities.Count; ++i)
            {
                if ((_actor.abilities[i].target == target) && (_actor.abilities[i].element == element))
                {
                    duration = _actor.abilities[i].duration;
                    intensity = _actor.abilities[i].intensity;
                    cooldown = _actor.abilities[i].cooldown;
                    cost = _actor.abilities[i].cost;
                    name = _actor.abilities[i].name;
                    bool improved = false;
                    do
                    {
                        switch (_random.Next(4))
                        {
                            case 0:
                                if ((duration < 12) &&
                                    ((element == AbilityElement.Health)
               || (element == AbilityElement.HealthReg)
                || (element == AbilityElement.Morph)
                || (element == AbilityElement.Charm)
                || (element == AbilityElement.Scare)
                || (element == AbilityElement.Stun)
                || (element == AbilityElement.ManaReg)
                || (element == AbilityElement.Health)
                || (element == AbilityElement.HealthReg)))
                                {
                                    duration += 1;
                                    description = "Duration " + _actor.abilities[i].duration + " -> " + duration.ToString();

                                    switch (_random.Next(3))
                                    {
                                        case 0:
                                            cost += 1;
                                            description += "\n(but: Cost " + _actor.abilities[i].cost + " -> " + cost.ToString() + "MP)";

                                            break;
                                        case 1:
                                            cooldown += 1;
                                            description += "\n(but: Cooldown " + _actor.abilities[i].cooldown + " -> " + cooldown.ToString() + ")";

                                            break;
                                    }
                                    improved = true;
                                    if (name.IndexOf("Durable") < 0)
                                        name = "Durable " + name;
                                }
                                break;
                            case 1:
                                if (intensity < 25)
                                {
                                    intensity += 1;
                                    description = "Strong " + _actor.abilities[i].intensity + " -> " + intensity.ToString();

                                    switch (_random.Next(3))
                                    {
                                        case 0:
                                            cost += 1;
                                            description += "\n(but: Cost " + _actor.abilities[i].cost + " -> " + cost.ToString() + "MP)";
                                            break;
                                        case 1:
                                            cooldown += 1;
                                            description += "\n(but: Cooldown " + _actor.abilities[i].cooldown + " -> " + cooldown.ToString() + ")";
                                            break;
                                    }
                                    improved = true;
                                    if (name.IndexOf("Strong") < 0)
                                        name = "Strong " + name;
                                }
                                break;
                            case 2:
                                if (cooldown > 5)
                                {
                                    cooldown -= 1;
                                    improved = true;
                                }
                                if (name.IndexOf("Fast") < 0)
                                    name = "Fast " + name;
                                description = "Cooldown " + _actor.abilities[i].cost + " -> " + cost.ToString();
                                break;
                            case 3:
                                if (cost > 5)
                                {
                                    cost -= 1;
                                    improved = true;
                                }
                                if (name.IndexOf("Cheap") < 0)
                                    name = "Cheap " + name;
                                description = "Cost " + _actor.abilities[i].cost + " -> " + cost.ToString() + " MP";
                                break;

                        }
                    } while (!improved);
                    improveOver = i;
                    break;
                }
            }
            Ability r = new Ability(_content, cost, intensity, duration, cooldown, target, element, name, description);
            _abilities[id] = r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (_visible)
            {
                int _selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, "Select a new or improved ability:", new Vector2(_displayRect.Left, _displayRect.Top), Color.White);
                for (int x = 0; x < 3; ++x)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (_width * x), _displayRect.Top + 20, _width - 2, _displayRect.Height - 20), new Rectangle(39, 6, 1, 1), Color.White);
                    if (x != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (_width * x) + 1, _displayRect.Top + 21, _width - 4, _displayRect.Height - 22), new Rectangle(39, 6, 1, 1), Color.Black);


                    _spriteBatch.Draw(_abilities[x].icon.texture, new Rectangle(_displayRect.Left + (_width * x) + (int)(((float)_width - (float)_abilities[x].icon.clipRect.Width) / 2f), _displayRect.Top + 29, _abilities[x].icon.clipRect.Width, _abilities[x].icon.clipRect.Height), _abilities[x].icon.clipRect, Color.White);
                    _spriteBatch.DrawString(_font, _abilities[x].name, new Vector2(_displayRect.Left - 2 + (_width * x) + (_width - _font.MeasureString(_abilities[x].name).X * 1.1f) / 2, _displayRect.Top + 58), Color.DarkBlue, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);

                    _spriteBatch.DrawString(_font, _abilities[x].name, new Vector2(_displayRect.Left + (_width * x) + (_width - _font.MeasureString(_abilities[x].name).X * 1.1f) / 2, _displayRect.Top + 60), Color.Orange, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
                    _spriteBatch.DrawString(_font, _abilities[x].description, new Vector2(_displayRect.Left + 4 + (_width * x), _displayRect.Top + 76), Color.Black);

                    _spriteBatch.DrawString(_font, _abilities[x].description, new Vector2(_displayRect.Left + 5 + (_width * x), _displayRect.Top + 77), Color.White);
                    if (_abilities[x].duration > 1)
                    {
                        _spriteBatch.DrawString(_font, "Duration: " + _abilities[x].duration, new Vector2(_displayRect.Left + (_width * x) + 6, _displayRect.Bottom - 79), Color.Black);
                        _spriteBatch.DrawString(_font, "Duration: " + _abilities[x].duration, new Vector2(_displayRect.Left + (_width * x) + 5, _displayRect.Bottom - 80), Color.White);
                    }
                    _spriteBatch.DrawString(_font, "Strength: " + _abilities[x].intensity, new Vector2(_displayRect.Left + (_width * x) + 6, _displayRect.Bottom - 59), Color.Black);
                    _spriteBatch.DrawString(_font, "Strength: " + _abilities[x].intensity, new Vector2(_displayRect.Left + (_width * x) + 5, _displayRect.Bottom - 60), Color.White);

                    _spriteBatch.DrawString(_font, "Cooldown: " + _abilities[x].cooldown, new Vector2(_displayRect.Left + (_width * x) + 6, _displayRect.Bottom - 39), Color.Black);
                    _spriteBatch.DrawString(_font, "Cooldown: " + _abilities[x].cooldown, new Vector2(_displayRect.Left + (_width * x) + 5, _displayRect.Bottom - 40), Color.White);

                    _spriteBatch.DrawString(_font, "Cost: " + _abilities[x].cost + "MP", new Vector2(_displayRect.Left + (_width * x) + 6, _displayRect.Bottom - 19), Color.Black);
                    _spriteBatch.DrawString(_font, "Cost: " + _abilities[x].cost + "MP", new Vector2(_displayRect.Left + (_width * x) + 5, _displayRect.Bottom - 20), Color.White);

                    if (x == _selected)
                    {
                        DisplayToolTip(x);
                    }

                }
            }
            _spriteBatch.End();
        }



        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        public void DisplayToolTip(int icon)
        {
            string text = _abilities[icon].name + "\n" + _abilities[icon].description + "\n Strength:" + _abilities[icon].intensity + "\n Cooldown:" + _abilities[icon].cooldown + "\n Cost: " + _abilities[icon].cost + "MP" + ((_abilities[icon].duration > 1) ? ("\n Duration:" + _abilities[icon].duration) : "");
            int textwidth = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).X + 1;
            int textheight = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).Y + 1;
            int lineHeight = (int)_font.MeasureString("Wgj").Y + 1;
            Color color = Color.White;
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + icon * (_width + 1)
    - textwidth - 2

    , _displayRect.Top

    - textheight - 2
    , textwidth + 5, textheight + 5), new Rectangle(39, 6, 1, 1), new Color(Color.Black, 0.9f));
            int line = 0;
            while (text.IndexOf("\n") > -1)
            {
                color = Color.White;
                text = text.TrimStart();
                if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
                if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }
                text = text.TrimStart();
                string next = text.Substring(text.IndexOf("\n") + 1);
                text = text.Substring(0, text.IndexOf("\n"));
                text.TrimEnd();
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + icon * (_width + 1)
                      - textwidth
                      , _displayRect.Top
                      - textheight + line), Color.Black);
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + icon * (_width + 1)
                    - textwidth + 1
                    , _displayRect.Top
                    - textheight + 1 + line), color);
                text = next;

                line += lineHeight;
            }
            if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
            if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }

            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + icon * (_width)
                - textwidth + 1

                , _displayRect.Top

                - textheight + 1 + line), color);

        }




        #region Constructor
        public AbilityChoice(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
            _random = new Random(Guid.NewGuid().GetHashCode());
            _abilities = new Ability[3];
            GenerateAbility(0);
            GenerateAbility(1);
            GenerateAbility(2);

        }
        #endregion

    }
}
