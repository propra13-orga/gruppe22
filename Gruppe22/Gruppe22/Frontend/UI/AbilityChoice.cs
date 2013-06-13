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
        private int _width = 120;
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
            if ((element == AbilityElement.Health)
               || (element == AbilityElement.HealthReg)
                || (element == AbilityElement.Morph)
                || (element == AbilityElement.Charm)
                || (element == AbilityElement.Scare)
                || (element == AbilityElement.Stun)
                || (element == AbilityElement.ManaReg)
                || (element == AbilityElement.Health)
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
                                    description = "Modification: Duration " + _actor.abilities[i].duration + " -> " + duration.ToString();

                                    switch (_random.Next(3))
                                    {
                                        case 0:
                                            cost += 1;
                                            description += " (but: Cost " + _actor.abilities[i].cost + " -> " + cost.ToString() + "MP)";

                                            break;
                                        case 1:
                                            cooldown += 1;
                                            description += " (but: Cooldown " + _actor.abilities[i].cooldown + " -> " + cooldown.ToString()+")";

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
                                    description = "Modification: Strong " + _actor.abilities[i].intensity + " -> " + intensity.ToString();

                                    switch (_random.Next(3))
                                    {
                                        case 0:
                                            cost += 1;
                                            description += " (but: Cost " + _actor.abilities[i].cost + " -> " + cost.ToString() + "MP)";
                                            break;
                                        case 1:
                                            cooldown += 1;
                                            description += " (but: Cooldown " + _actor.abilities[i].cooldown + " -> " + cooldown.ToString() + ")";
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
                                description = "Modification: Cooldown " + _actor.abilities[i].cost + " -> " + cost.ToString();
                                break;
                            case 3:
                                if (cost > 5)
                                {
                                    cost -= 1;
                                    improved = true;
                                }
                                if (name.IndexOf("Cheap") < 0)
                                    name = "Cheap " + name;
                                description = "Modification: Cost " + _actor.abilities[i].cost + " -> " + cost.ToString() + " MP";
                                break;

                        }
                    } while (!improved);
                    improveOver = i;
                    break;
                }
            }
            Ability r = new Ability(_content, cost, intensity, duration, cooldown, target, element);
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
                for (int x = 0; x < 4; ++x)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (_width * x), _displayRect.Top + 20, _width - 2, _displayRect.Height - 20), new Rectangle(39, 6, 1, 1), Color.White);
                    if (x != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (_width * x) + 1, _displayRect.Top + 21, _width - 4, _displayRect.Height - 22), new Rectangle(39, 6, 1, 1), Color.Black);

                    /*

                    if ((x < _actor.abilities.Count) && (_actor.abilities[icon].icon != null))
                    {
                        _spriteBatch.Draw(_actor.abilities[icon].icon.texture, new Rectangle(_displayRect.Left + 1, _displayRect.Top + y * (_height + 3) + 1, _width, _height), _actor.abilities[icon].icon.clipRect, Color.White);
                        if (_icons[icon].check)
                        {
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Right - 16, _displayRect.Top + y * (_height + 3) + 2, 8, 8), new Rectangle(48, 16, 16, 16), Color.White);
                        }
                        if (icon == _selected)
                        {
                            int textwidth = (int)_font.MeasureString(_icons[icon].tooltip).X + 1;
                            int textheight = (int)_font.MeasureString(_icons[icon].tooltip).Y + 1;
                            DisplayToolTip(icon, 0, y);
                        }
                    }
                    ++icon;
                     */
                }
            }
            _spriteBatch.End();
        }



        #region Constructor
        public AbilityChoice(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
            _random = new Random(Guid.NewGuid().GetHashCode());
            _abilities = new Ability[3];

        }
        #endregion

    }
}
