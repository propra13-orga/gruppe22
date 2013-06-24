using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22.Client
{
    public class Toolbar : UIElement
    {
        private Texture2D _background = null;
        private SpriteFont _font = null;
        private Backend.Actor _actor = null;
        private int _cellWidth = 34;
        private List<GridElement> _functions;
        private bool _enabled = true;
        private int _lastCheck = 0;
        private bool _updating = false;
        private GridElement _dragItem;

        public GridElement dragItem
        {
            get
            {
                return _dragItem;
            }
            set
            {
                _dragItem = value;
            }
        }

        public Backend.Actor actor
        {
            get
            {
                return _actor;
            }
            set
            {
                _actor = value;
                for (int i = 0; i < 10; ++i)
                {
                    // System.Diagnostics.Debug.WriteLine(_actor.quickList[i]);

                    if (_actor.quickList[i] < 0)
                    {
                        Backend.Item item = _actor.Items(-_actor.quickList[i]);
                        if (item != null)
                            _functions[i] = new GridElement(_actor.quickList[i], item.name, item.icon, false, true, 0);
                        else
                            _functions[i] = new GridElement(0, "Function " + i.ToString() + " (" + i.ToString() + ")", new VisibleObject(_content, "items", new Rectangle(160, 704, 32, 32)), false, false, 0);
                    }
                    else
                    {
                        if (_actor.quickList[i] > 0)
                        {
                            _functions[i] = new GridElement(_actor.quickList[i], _actor.abilities[_actor.quickList[i]].name, _actor.abilities[_actor.quickList[i]].icon, false, true, 0);
                        }
                        else
                        {
                            _functions[i] = new GridElement(0, "Function " + i.ToString() + " (" + i.ToString() + ")", new VisibleObject(_content, "items", new Rectangle(160, 704, 32, 32)), false, false, 0);
                        }
                    }
                }

            }
        }

        public override void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {
            if (DownStream)
            {
                switch (eventID)
                {
                    case Backend.Events.AddDragItem:
                        for (int i = 0; i < 10; ++i)
                        {
                            if (_functions[i].id == (int)data[0])
                            {
                                _functions[i].id = 0;
                                _functions[i].enabled = false;
                            }
                        }
                        break;
                    case Backend.Events.ContinueGame:
                        _functions[10].check = false;
                        _functions[13].check = false;
                        _functions[11].check = false;
                        _functions[12].check = false;
                        if ((data[0] != null) && ((int)data[0] > 9))
                        {
                            _functions[(int)data[0]].check = true;
                        }
                        break;
                }
            }
            base.HandleEvent(DownStream, eventID, data);
        }
        public override void Update(GameTime gameTime)
        {
            if (!_updating)
            {
                _lastCheck += gameTime.ElapsedGameTime.Milliseconds;
                _updating = true;

                for (int i = 0; i < 10; ++i)
                {
                    if (((_functions[i].id > 0) && ((_actor.mana > _actor.abilities[_functions[i].id - 1].cost) && (_actor.abilities[_functions[i].id - 1].currentCool <= 0)))
                        || (_functions[i].id < 0))
                        _functions[i].enabled = true;
                    else
                    {
                        _functions[i].enabled = false;
                        if ((_lastCheck > 70) && (_functions[i].id > 0) && (_actor.abilities[_functions[i].id - 1].currentCool > 0)) _actor.abilities[_functions[i].id - 1].currentCool -= 1;
                    }
                }
                if (_lastCheck > 70)
                {
                    _lastCheck -= 70;

                    for (int i = 0; i < 13; ++i)
                    {
                        if (_functions[i].flash > 0)
                        {
                            _functions[i].flash -= 1;
                        }
                    }
                }
                _updating = false;
            }
        }

        public int Pos2Tile(int x, int y)
        {
            int result = -1;
            if (_displayRect.Contains(x, y))
            {
                x -= _displayRect.Left;
                y -= _displayRect.Top;
                x = x / (_cellWidth + 1);
                y = y / (_cellWidth + 1);
                result = x;
            }
            if (result < 14)
                return result;
            return -1;
        }

        public override bool OnKeyDown(Microsoft.Xna.Framework.Input.Keys k)
        {
            if (_enabled)
            {
                switch (k)
                {
                    case Keys.D1:
                        if ((_functions[0].id != 0) && (_functions[0].enabled) && (_functions[0].flash == 0))
                        {
                            _functions[0].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[0].id);

                        }
                        break;

                    case Keys.D2:
                        if ((_functions[1].id != 0) && (_functions[1].enabled) && (_functions[1].flash == 0))
                        {
                            _functions[1].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[1].id);

                        }
                        break;

                    case Keys.D3:
                        if ((_functions[2].id != 0) && (_functions[2].enabled) && (_functions[2].flash == 0))
                        {
                            _functions[2].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[2].id);

                        }
                        break;

                    case Keys.D4:
                        if ((_functions[3].id != 0) && (_functions[3].enabled) && (_functions[3].flash == 0))
                        {
                            _functions[3].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[3].id);

                        }
                        break;

                    case Keys.D5:
                        if ((_functions[4].id != 0) && (_functions[4].enabled) && (_functions[4].flash == 0))
                        {
                            _functions[4].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[4].id);

                        }
                        break;

                    case Keys.D6:
                        if ((_functions[5].id != 0) && (_functions[5].enabled) && (_functions[5].flash == 0))
                        {
                            _functions[5].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[5].id);

                        }
                        break;

                    case Keys.D7:
                        if ((_functions[6].id != 0) && (_functions[6].enabled) && (_functions[6].flash == 0))
                        {
                            _functions[6].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[6].id);

                        }
                        break;

                    case Keys.D8:
                        if ((_functions[7].id != 0) && (_functions[7].enabled) && (_functions[7].flash == 0))
                        {
                            _functions[7].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[7].id);

                        }
                        break;

                    case Keys.D9:
                        if ((_functions[8].id != 0) && (_functions[8].enabled) && (_functions[8].flash == 0))
                        {
                            _functions[8].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[8].id);

                        }
                        break;

                    case Keys.D0:
                        if ((_functions[9].id != 0) && (_functions[9].enabled) && (_functions[9].flash == 0))
                        {
                            _functions[9].flash = 5;
                            _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[9].id);
                        }
                        break;
                    case Keys.C:
                        _functions[10].check = !_functions[10].check;
                        _parent.HandleEvent(false, Backend.Events.ShowCharacter, _actor);
                        break;

                    case Keys.I:
                        _functions[11].check = !_functions[11].check;
                        _parent.HandleEvent(false, Backend.Events.ShowInventory, _actor);
                        break;


                    case Keys.S:
                        _functions[12].check = !_functions[12].check;
                        _parent.HandleEvent(false, Backend.Events.ShowAbilities, _actor);
                        break;

                    case Keys.Escape:
                        _functions[13].check = !_functions[13].check;
                        _parent.HandleEvent(false, Backend.Events.ShowMenu);
                        break;


                }
            }
            return base.OnKeyDown(k);

        }

        public override bool OnMouseUp(int button)
        {
            int cursel = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
            if (cursel != -1)
            {
                switch (cursel)
                {
                    case 10:
                        _functions[10].check = !_functions[10].check;
                        _parent.HandleEvent(false, Backend.Events.ShowCharacter, _actor);

                        break;

                    case 11:
                        _functions[11].check = !_functions[11].check;
                        _parent.HandleEvent(false, Backend.Events.ShowInventory, _actor);
                        break;

                    case 12:
                        _functions[12].check = !_functions[12].check;
                        _parent.HandleEvent(false, Backend.Events.ShowAbilities, _actor);
                        break;

                    case 13:
                        _functions[13].check = !_functions[13].check;
                        _parent.HandleEvent(false, Backend.Events.ShowMenu);
                        break;


                    default:
                        if (_dragItem != null)
                        {
                            for (int i = 0; i < 10; ++i)
                            {
                                if (_actor.quickList[i] == _dragItem.id)
                                {
                                    _actor.quickList[i] = 0;
                                    _functions[i].id = 0;
                                    _functions[i].enabled = false;
                                }
                            }
                            _functions[cursel] = _dragItem;
                            _actor.quickList[cursel] = _dragItem.id;

                        }
                        else
                        {
                            if ((_functions[cursel].id != 0) && (_functions[cursel].enabled) && (_functions[cursel].flash == 0))
                            {
                                _functions[cursel].flash = 5;
                                _parent.HandleEvent(false, Backend.Events.ActivateAbility, _actor, _functions[cursel].id);
                            }



                        }
                        break;
                }
            }
            _dragItem = null;
            return base.OnMouseUp(button);
        }

        public override bool OnMouseDown(int button)
        {
            return base.OnMouseDown(button);
        }

        public override void Draw(GameTime gametime)
        {
            int _selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
            if ((_selected > -1) && (!_functions[_selected].enabled))
                _selected = -1;
            _spriteBatch.Begin();
            for (int i = 0; i < 14; ++i)
            {
                if (_functions[i].check)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + i * (_cellWidth + 1), _displayRect.Top, _cellWidth + 2, _cellWidth + 2), new Rectangle(39, 6, 1, 1), Color.LightBlue);
                }
                else
                {
                    if (_functions[i].flash > 0)
                    {
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + i * (_cellWidth + 1), _displayRect.Top, _cellWidth + 2, _cellWidth + 2), new Rectangle(39, 6, 1, 1), _functions[i].flash % 2 == 0 ? Color.DarkRed : Color.Black);

                    }
                    else
                    {
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + i * (_cellWidth + 1), _displayRect.Top, _cellWidth + 2, _cellWidth + 2), new Rectangle(39, 6, 1, 1), Color.Gray);
                    }
                }

                if ((i != _selected) && (_functions[i].check == false) && (_functions[i].flash == 0))
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + i * (_cellWidth + 1) + 1, _displayRect.Top, _cellWidth, _cellWidth + 2), new Rectangle(39, 6, 1, 1), Color.Black);

                if (_functions[i].id != 0)
                    _spriteBatch.Draw(_functions[i].icon.texture, new Rectangle(_displayRect.Left + 2 + i * (_cellWidth + 1) + 1, 2 + _displayRect.Top + 1, 32, 32), _functions[i].icon.clipRect, _functions[i].enabled ? Color.White : Color.LightBlue);



                string key = "";
                switch (i)
                {
                    case 10:
                        key = "C";
                        break;
                    case 11:
                        key = "I";
                        break;
                    case 12:
                        key = "S";
                        break;
                    case 13:
                        key = "ESC";
                        break;
                    case 9:
                        key = "0";
                        break;
                    default:
                        key = (i + 1).ToString();
                        break;
                }
                _spriteBatch.DrawString(_font, key, new Vector2(_displayRect.Left + i * (_cellWidth + 1) + 3, _displayRect.Top + 2), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

                _spriteBatch.DrawString(_font, key, new Vector2(_displayRect.Left + i * (_cellWidth + 1) + 2, _displayRect.Top + 1), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

            }
            if (_selected > -1)
                DisplayToolTip(_selected);
            _spriteBatch.End();
        }




        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        public void DisplayToolTip(int icon)
        {
            string text = _functions[icon].tooltip;
            int textwidth = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).X + 1;
            int textheight = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).Y + 1;
            int lineHeight = (int)_font.MeasureString("Wgj").Y + 1;
            Color color = Color.White;
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + icon * (_cellWidth + 1)
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
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + icon * (_cellWidth + 1)
                      - textwidth
                      , _displayRect.Top
                      - textheight + line), Color.Black);
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + icon * (_cellWidth + 1)
                    - textwidth + 1
                    , _displayRect.Top
                    - textheight + 1 + line), color);
                text = next;

                line += lineHeight;
            }
            if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
            if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }

            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + icon * (_cellWidth)
                - textwidth + 1

                , _displayRect.Top

                - textheight + 1 + line), color);

        }




        public Toolbar(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Backend.Actor actor)
            : base(parent, spriteBatch, content, displayRect)
        {
            _displayRect.Height = _cellWidth + 2;
            _displayRect.Width = (_cellWidth + 1) * 14;
            _font = _content.Load<SpriteFont>("SmallFont");
            _background = _content.Load<Texture2D>("Minimap");
            _actor = actor;
            _functions = new List<GridElement>(14);
            // System.Diagnostics.Debug.WriteLine(_actor.name);
            for (int i = 0; i < 10; ++i)
            {
                //  System.Diagnostics.Debug.WriteLine(_actor.quickList[i]);

                if (_actor.quickList[i] < 0)
                {
                    Backend.Item item = _actor.Items(-_actor.quickList[i]);
                    if (item != null)
                        _functions.Add(new GridElement(_actor.quickList[i], item.name, item.icon, false, true, 0));
                    else
                        _functions.Add(new GridElement(0, "Function " + i.ToString() + " (" + i.ToString() + ")", new VisibleObject(_content, "items", new Rectangle(160, 704, 32, 32)), false, false, 0));
                }
                else
                {
                    if (_actor.quickList[i] > 0)
                    {
                        int icon = _actor.quickList[i] - 1;
                        string text = _actor.abilities[icon].name + "\n Strength:" + _actor.abilities[icon].intensity + "\n Cooldown:" + _actor.abilities[icon].cooldown + "\n Cost: " + _actor.abilities[icon].cost + "MP" + ((_actor.abilities[icon].duration > 1) ? ("\n Duration:" + _actor.abilities[icon].duration) : "");

                        _functions.Add(new GridElement(_actor.quickList[i], text, _actor.abilities[_actor.quickList[i] - 1].icon, false, true, 0));
                    }
                    else
                    {
                        _functions.Add(new GridElement(0, "Function " + i.ToString() + " (" + i.ToString() + ")", new VisibleObject(_content, "items", new Rectangle(160, 704, 32, 32)), false, false, 0));
                    }
                }

            }
            _functions.Add(new GridElement(1, "Character (C)", new VisibleObject(_content, "items", new Rectangle(128, 831, 32, 32)), false, true, 0));
            _functions.Add(new GridElement(1, "Inventory (I)", new VisibleObject(_content, "items", new Rectangle(288, 129, 32, 32)), false, true, 0));
            _functions.Add(new GridElement(1, "Skills (S)", new VisibleObject(_content, "items", new Rectangle(257, 768, 32, 32)), false, true, 0));

            _functions.Add(new GridElement(1, "Menu (ESC)", new VisibleObject(_content, "items", new Rectangle(320, 192, 32, 32)), false, true, 0));

        }
    }
}
