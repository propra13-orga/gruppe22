﻿using System;
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
    public enum ShopButtons
    {
        Buy = 1,
        Sell = 2,
        Modify = 3,
        Leave = 4,
        Steal = 5
    }
    public class Shop : Window
    {
        #region Private Fields
        /// <summary>
        /// 
        /// </summary>
        private SpriteFont _font = null;

        /// <summary>
        /// 
        /// </summary>
        private int _rows = 2;

        /// <summary>
        /// 
        /// </summary>
        private int _height = 32;

        /// <summary>
        /// 
        /// </summary>
        private int _selected1 = -1;

        /// <summary>
        /// 
        /// </summary>
        private int _selected2 = -1;

        /// <summary>
        /// 
        /// </summary>
        private int _top1 = 0;

        /// <summary>
        /// 
        /// </summary>
        private int _top2 = 0;

        /// <summary>
        /// 
        /// </summary>
        private Texture2D _arrows = null;


        /// <summary>
        /// 
        /// </summary>
        private Backend.Actor _seller = null;

        /// <summary>
        /// 
        /// </summary>
        private Backend.Actor _buyer = null;

        /// <summary>
        /// 
        /// </summary>
        private Button _steal = null;

        /// <summary>
        /// 
        /// </summary>
        private Button _buy = null;

        /// <summary>
        /// 
        /// </summary>
        private Button _sell = null;

        /// <summary>
        /// 
        /// </summary>
        private Button _modify = null;

        /// <summary>
        /// 
        /// </summary>
        private NumberEntry _sellergold = null;

        /// <summary>
        /// 
        /// </summary>
        private NumberEntry _buyergold = null;

        /// <summary>
        /// 
        /// </summary>
        private Button _leave = null;

        /// <summary>
        /// 
        /// </summary>
        private int _subfocus = 0;
        #endregion

        #region Public Methods



        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        public void DisplayToolTip(int column, int y, string text)
        {
            int textwidth = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).X + 1;
            int textheight = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).Y + 1;
            int lineHeight = (int)_font.MeasureString("Wgj").Y + 1;
            Color color = Color.Black;
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 5 + column * (_displayRect.Width / 2 + 5)


    , _displayRect.Top + 45 + (y + 1) * (_height + 3)

    , textwidth + 10, textheight + 5), new Rectangle(39, 6, 1, 1), new Color(Color.DarkGray, 0.9f));
            int line = 0;
            while (text.IndexOf("\n") > -1)
            {
                color = Color.Black;
                text = text.TrimStart();
                if (text.StartsWith("<red>"))
                {
                    color = Color.Red;
                    text = text.Substring(5);
                }
                if (text.StartsWith("<green>"))
                {
                    color = Color.Green;
                    text = text.Substring(7);
                }
                text = text.TrimStart();
                string next = text.Substring(text.IndexOf("\n") + 1);
                text = text.Substring(0, text.IndexOf("\n"));
                text.TrimEnd();
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + 8 + column * (_displayRect.Width / 2 + 5)


                      ,

                      _displayRect.Top + 48 + (y + 1) * (_height + 3)

     + line), Color.Black);
                text = next;

                line += lineHeight;
            }
            if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
            if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }

            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + 8 + column * (_displayRect.Width / 2 + 5)
 + 1
                ,
                      _displayRect.Top + 48 + (y + 1) * (_height + 3)

 + line), color);

        }

        private int _hoveredItem
        {
            get
            {
                Backend.Coords mousepos = new Backend.Coords(Mouse.GetState().X, Mouse.GetState().Y);
                int _mouseover = 0;
                if ((mousepos.x > _displayRect.Left + 4) && (mousepos.x < _displayRect.Left + _displayRect.Width / 2))
                {
                    if ((mousepos.y > _displayRect.Top + 45) && (mousepos.y < _displayRect.Bottom - 40))
                    {
                        mousepos.y -= (_displayRect.Top + 45);
                        mousepos.y /= (_height + 3);
                        _mouseover = -mousepos.y - 1;
                    }
                }
                else
                    if ((mousepos.x > _displayRect.Left + _displayRect.Width / 2 + 7) && (mousepos.x < _displayRect.Right - 10))
                    {
                        if ((mousepos.y > _displayRect.Top + 45) && (mousepos.y < _displayRect.Bottom - 40))
                        {
                            mousepos.y -= (_displayRect.Top + 45);
                            mousepos.y /= (_height + 3);
                            _mouseover = mousepos.y + 1;
                        }
                    }
                return _mouseover;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            int _mouseover = _hoveredItem;
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _seller.name, new Vector2(_displayRect.Left + 5, _displayRect.Top + 15), Color.Gray);
            _spriteBatch.DrawString(_font, _seller.name, new Vector2(_displayRect.Left + 7, _displayRect.Top + 13), Color.White);

            _spriteBatch.DrawString(_font, _buyer.name, new Vector2(_displayRect.Left + _displayRect.Width / 2 + 7, _displayRect.Top + 15), Color.Gray);
            _spriteBatch.DrawString(_font, _buyer.name, new Vector2(_displayRect.Left + _displayRect.Width / 2 + 9, _displayRect.Top + 13), Color.White);

            int y = 0;
            for (y = 0; y < _rows; ++y)
            {
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 45 + y * (_height + 3), _displayRect.Width / 2 - 38, _height + 2), new Rectangle(39, 6, 1, 1), (_selected1 == _top1 + y) ? Color.LightSkyBlue : Color.White);
                if ((-(_mouseover + 1) != y) && (_selected1 != _top1 + y))
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 6, _displayRect.Top + 45 + y * (_height + 3) + 1, _displayRect.Width / 2 - 40, _height), new Rectangle(39, 6, 1, 1), Color.Black);

                _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + _displayRect.Width / 2 + 7, _displayRect.Top + 45 + y * (_height + 3), _displayRect.Width / 2 - 38, _height + 2), new Rectangle(39, 6, 1, 1), (_selected2 == _top2 + y) ? Color.LightSkyBlue : Color.White);
                if (((_mouseover - 1) != y) && (_selected2 != _top2 + y))
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + _displayRect.Width / 2 + 8, _displayRect.Top + 45 + y * (_height + 3) + 1, _displayRect.Width / 2 - 40, _height), new Rectangle(39, 6, 1, 1), Color.Black);

            }

            y = 0;
            for (int i = _top1; i < Math.Min(_seller.inventory.Count, _top1 + _rows); ++i)
            {
                _spriteBatch.Draw(TextureFromData.Convert(_seller.inventory[i].icon, _content), new Rectangle(_displayRect.Left + 6, 45 + _displayRect.Top + y * (_height + 3) + 3, 32, 32), _seller.inventory[i].icon.rect, Color.White);
                string output = _seller.inventory[i].name;
                if (output.Length > 12)
                {
                    output = output.Substring(0, 12) + "...";
                }
                _spriteBatch.DrawString(_font, output, new Vector2(_displayRect.Left + 39, 45 + _displayRect.Top + y * (_height + 3) + 9), ((-_mouseover - 1 == y) || ((_subfocus != 1) && (_selected1 == i))) ? Color.Black : Color.White);
                output = _seller.inventory[i].value.ToString();
                _spriteBatch.DrawString(_font, output, new Vector2(_displayRect.Left + _displayRect.Width / 2 - 40 - _font.MeasureString(output).X, 45 + _displayRect.Top + y * (_height + 3) + 9), ((-_mouseover - 1 == y) || ((_subfocus != 1) && (_selected1 == i))) ? Color.Black : Color.White);

                ++y;
            }

            y = 0;
            for (int i = _top2; i < Math.Min(_buyer.inventory.Count, _top2 + _rows); ++i)
            {
                _spriteBatch.Draw(TextureFromData.Convert(_buyer.inventory[i].icon, _content), new Rectangle(_displayRect.Left + 9 + _displayRect.Width / 2, 45 + _displayRect.Top + y * (_height + 3) + 3, 32, 32), _buyer.inventory[i].icon.rect, Color.White);
                string output = _buyer.inventory[i].name;
                if (output.Length > 12)
                {
                    output = output.Substring(0, 12) + "...";
                }
                _spriteBatch.DrawString(_font, output, new Vector2(_displayRect.Left + 42 + _displayRect.Width / 2, 45 + _displayRect.Top + y * (_height + 3) + 9), ((_mouseover - 1 == y) || ((_subfocus != 2) && (_selected2 == i))) ? Color.Black : Color.White);
                output = _buyer.inventory[i].value.ToString();

                _spriteBatch.DrawString(_font, output, new Vector2(_displayRect.Right - 38 - _font.MeasureString(output).X, 45 + _displayRect.Top + y * (_height + 3) + 9), ((_mouseover - 1 == y) || ((_subfocus != 2) && (_selected2 == i))) ? Color.Black : Color.White);

                ++y;

            }


            if (_mouseover < 0)
            {
                if (Math.Abs(_mouseover) - 1 < _seller.inventory.Count)
                    DisplayToolTip(0,
                        Math.Abs(_mouseover) - 1,
                        _seller.inventory[Math.Abs(_mouseover) - 1].name + (_seller.inventory[Math.Abs(_mouseover) - 1].equipped ? " (equipped)" : "") + _seller.inventory[Math.Abs(_mouseover) - 1].abilityList);
            }
            if (_mouseover > 0)
            {
                if (Math.Abs(_mouseover) - 1 < _buyer.inventory.Count)
                    DisplayToolTip(1,
                        Math.Abs(_mouseover) - 1,
                        _buyer.inventory[Math.Abs(_mouseover) - 1].name + (_buyer.inventory[Math.Abs(_mouseover) - 1].equipped ? " (equipped)" : "") + _buyer.inventory[Math.Abs(_mouseover) - 1].abilityList);
            }

            // Scroll bars
            if (_seller.inventory.Count > _rows)
            {
                if (_top1 > 0)
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Left + _displayRect.Width / 2 - 30, _displayRect.Top + 45, 22, 22), new Rectangle(32, 0, 28, 28), Color.White);
                if (_top1 + _rows < _seller.inventory.Count)
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Left + _displayRect.Width / 2 - 30, _displayRect.Bottom - 80, 22, 22), new Rectangle(0, 0, 28, 28), Color.White);
            }
            else
            {
                _top1 = 0;
            }

            if (_buyer.inventory.Count > _rows)
            {
                if (_top2 > 0)
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 28, _displayRect.Top + 45, 22, 22), new Rectangle(32, 0, 28, 28), Color.White);
                if (_top2 + _rows < _buyer.inventory.Count)
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 28, _displayRect.Bottom - 80, 22, 22), new Rectangle(0, 0, 28, 28), Color.White);
            }
            else
            {
                _top2 = 0;
            }


            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override bool OnMouseDown(int button)
        {
            if (new Rectangle(_displayRect.Left + _displayRect.Width / 2 - 30, _displayRect.Top + 45, 22, 22).Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if (_top1 > 0) _top1 -= 1;
                return true;
            }
            else
                if (new Rectangle(_displayRect.Left + _displayRect.Width / 2 - 30, _displayRect.Bottom - 80, 22, 22).Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    if (_top1 + _rows < _seller.inventory.Count) _top1 += 1;
                    return true;
                }
                else
                    if (new Rectangle(_displayRect.Right - 28, _displayRect.Top + 45, 22, 22).Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        if (_top2 > 0) _top2 -= 1;
                        return true;
                    }

            if (new Rectangle(_displayRect.Right - 28, _displayRect.Bottom - 80, 22, 22).Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if (_top2 + _rows < _buyer.inventory.Count) _top2 += 1;
                return true;
            }
            else
            {
                int clicked = _hoveredItem;
                if (clicked != 0)
                {
                    if (clicked < 0)
                    {
                        if ((_selected1 != _top1 - clicked + 1) && (_top1 - clicked - 1 < _seller.inventory.Count))
                        {
                            _steal.Show();
                            _selected1 = _top1 - clicked - 1;

                            if (_buyer.gold > _seller.inventory[_selected1].value)
                                _buy.Show();
                            else
                                _buy.Hide();

                        }
                        else
                        {
                            _selected1 = -1;
                            _buy.Hide();
                            _steal.Hide();
                        }
                    }
                    else
                    {
                        if ((_selected2 != _top2 + clicked - 1) && (_top1 + clicked - 1 < _buyer.inventory.Count))
                        {
                            _modify.Show();

                            _selected2 = _top2 + clicked - 1;
                            if ((_seller.gold > _buyer.inventory[_selected2].value) && (_buyer.inventory[_selected2].value != 0))
                                _sell.Show();
                            else
                                _sell.Hide();

                        }
                        else
                        {
                            _sell.Hide();
                            _modify.Hide();
                            _selected2 = -1;
                        }
                    }
                    return true;
                }
                else
                    return base.OnMouseDown(button);
            }
        }

        public override bool OnKeyDown(Microsoft.Xna.Framework.Input.Keys k)
        {
            return base.OnKeyDown(k);
        }

        public override void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {
            switch (eventID)
            {
                case Backend.Events.ButtonPressed:
                    switch ((ShopButtons)(int)data[0])
                    {
                        case ShopButtons.Sell:
                            if ((_selected2 > -1) && (_buyer.inventory[_selected2].value != 0))
                            {
                                Backend.Item tmp = _buyer.inventory[_selected2];
                                _buyer.gold += tmp.value;
                                _seller.gold -= tmp.value;
                                _buyergold.value = _buyer.gold;
                                _sellergold.value = _seller.gold;
                                if (tmp.equipped) tmp.EquipItem();
                                _buyer.inventory.Remove(tmp);
                                _seller.AddItem(tmp);
                                if (_selected2 + 1 > _buyer.inventory.Count)
                                {
                                    if (_buyer.inventory.Count > 0)
                                    {
                                        _selected2 -= 1;

                                        if (_seller.gold > _buyer.inventory[_selected2].value)
                                            _sell.Show();
                                        else
                                            _sell.Hide();


                                    }
                                    else
                                    {
                                        _selected2 = -1;

                                        _sell.Hide();
                                        _modify.Hide();
                                    }
                                    if (_selected1 > -1)
                                        if (_buyer.gold > _seller.inventory[_selected1].value)
                                            _buy.Show();
                                        else
                                            _buy.Hide();
                                }

                            }
                            break;
                        case ShopButtons.Buy:
                            if ((_selected1 > -1) && _seller.inventory[_selected1].value != 0)
                            {
                                Backend.Item tmp = _seller.inventory[_selected1];
                                _buyer.gold -= tmp.value;
                                _seller.gold += tmp.value;
                                _buyergold.value = _buyer.gold;
                                _sellergold.value = _seller.gold;
                                if (tmp.equipped) tmp.EquipItem();
                                _seller.inventory.Remove(tmp);
                                _buyer.AddItem(tmp);
                                if (_selected1 + 1 > _seller.inventory.Count)
                                {
                                    if (_seller.inventory.Count > 0)
                                    {
                                        _selected1 -= 1;
                                        if (_buyer.gold > _seller.inventory[_selected1].value)
                                            _buy.Show();
                                        else
                                            _buy.Hide();
                                    }
                                    else
                                    {
                                        _selected1 = -1;
                                        _buy.Hide();
                                        _steal.Hide();
                                    }
                                }
                                if (_selected2 > -1)
                                    if (_seller.gold > _buyer.inventory[_selected2].value)
                                        _sell.Show();
                                    else
                                        _sell.Hide();
                            }
                            break;
                        case ShopButtons.Leave: // Cancel
                            _parent.HandleEvent(true, Backend.Events.ContinueGame, null);
                            break;
                        case ShopButtons.Modify:
                            if (_selected2 > -1)
                            {
                                AddChild(new ModifyWindow(this, _spriteBatch, _content, new Rectangle(0, 0, 200, 100), _buyer.inventory[_selected2]));
                            }
                            break;

                        case ShopButtons.Steal:
                            if (_selected1 > -1)
                            {
                                Backend.Item tmp = _seller.inventory[_selected1];
                                if (tmp.equipped) tmp.EquipItem();
                                _seller.inventory.Remove(tmp);
                                _buyer.AddItem(tmp);
                                if (_selected1 + 1 > _seller.inventory.Count)
                                {
                                    if (_seller.inventory.Count > 0)
                                    {
                                        _selected1 -= 1;

                                        if (_buyer.gold > _seller.inventory[_selected1].value)
                                            _buy.Show();
                                        else
                                            _buy.Hide();

                                    }
                                    else
                                    {
                                        _selected1 = -1;
                                        _buy.Hide();
                                        _steal.Hide();
                                    }
                                }
                            }
                            break;
                    }
                    break;
                default:
                    base.HandleEvent(DownStream, eventID, data);
                    break;
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Shop(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Backend.Actor actor1, Backend.Actor actor2)
            : base(parent, spriteBatch, content, displayRect)
        {
            _buyer = actor1;
            _seller = actor2;

            _background = _content.Load<Texture2D>("Minimap");
            _arrows = _content.Load<Texture2D>("Arrows");
            _rows = (int)((_displayRect.Height - 65) / (_height + 3));
            _font = _content.Load<SpriteFont>("SmallFont");
            AddChild(_steal = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Bottom - 35, 80, 30), "Steal", (int)ShopButtons.Steal, false));
            AddChild(_buy = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 90, _displayRect.Bottom - 35, 80, 30), "Buy", (int)ShopButtons.Buy, false));
            AddChild(_sell = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Right - 170, _displayRect.Bottom - 35, 80, 30), "Sell", (int)ShopButtons.Sell, false));
            AddChild(_modify = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Right - 85, _displayRect.Bottom - 35, 80, 30), "Modify", (int)ShopButtons.Modify, false));
            AddChild(_sellergold = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + (_displayRect.Width - 80) / 2 - 70, _displayRect.Top + 12, 102, 20), "Gold", _seller.gold, "Seller's gold", 3, false));
            AddChild(_buyergold = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + (_displayRect.Width - 107), _displayRect.Top + 12, 102, 20), "Gold", _buyer.gold, "Buyer's gold", 3, false));
            AddChild(_leave = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + (_displayRect.Width - 80) / 2, _displayRect.Bottom - 35, 80, 30), "Leave", (int)ShopButtons.Leave, false));
            _buy.Hide();
            _sell.Hide();
            _modify.Hide();
            _steal.Hide();
        }
        #endregion

    }
}
