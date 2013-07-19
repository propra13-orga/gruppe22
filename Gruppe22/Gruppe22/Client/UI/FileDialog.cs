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
    public class FileInfo
    {
        private Texture2D _screenshot = null;
        private DateTime _dateTime = DateTime.Now;
        private string _name = "";
        private string _playername = "";
        private int _level = 0;
        private int _health = 0;
        private int _maxhealth = 0;
        private int _mana = 0;
        private int _maxmana = 0;

        public int health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public string playername
        {
            get
            {
                return _playername;
            }
            set
            {
                _playername = value;
            }
        }

        public int maxHealth
        {
            get
            {
                return _maxhealth;
            }
            set
            {
                _maxhealth = value;
            }
        }

        public int maxMana
        {
            get
            {
                return _maxmana;
            }
            set
            {
                _maxmana = value;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public DateTime dateTime
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value;
            }
        }

        public Texture2D screenshot
        {
            get
            {
                return _screenshot;
            }
            set
            {
                _screenshot = value;
            }
        }

        public FileInfo(Texture2D screenshot = null, string name = "")
        {
            _screenshot = screenshot;

            _name = name;
        }


        public FileInfo(DateTime dateTime, Texture2D screenshot = null, string name = "")
            : this(screenshot, name)
        {
            _dateTime = System.IO.File.GetCreationTime(System.IO.Directory.GetCurrentDirectory() + "\\" + name + "\\screen.png");
        }
    }
    public class FileDialog : Window, Backend.IHandleEvent
    {
        private TextInput _filename = null;
        private Button _ok = null;
        private Button _cancel = null;
        private List<FileInfo> _files = null;
        private bool _save = false;
        private int _checked = -1;
        private int _totalPages = 0;
        private int _rows = 3;
        private int _page = 0;
        private int _height = 100;
        protected SpriteFont _font = null;
        protected Texture2D _arrows = null;
        protected Texture2D _background = null;



        private void _BuildList()
        {
            if (_files == null) _files = new List<FileInfo>(); else _files.Clear();
            foreach (string _dir in System.IO.Directory.GetDirectories("save\\"))
            {
                if (_dir.Substring(_dir.LastIndexOf('\\') + 1) != "auto")
                {
                    _files.Add(new FileInfo(_content.Load<Texture2D>(
                        System.IO.Directory.GetCurrentDirectory() + "\\" + _dir + "\\screen.png"), _dir.Substring(_dir.LastIndexOf('\\') + 1)));

                }
            }
        }


        public override bool OnMouseDown(int button)
        {
            if (_visible)
            {
                int x = Mouse.GetState().X;
                int y = Mouse.GetState().Y;
                _totalPages = (int)Math.Ceiling((float)_files.Count / (float)_rows);

                if (new Rectangle(_displayRect.Right - 35, _displayRect.Top + 20, 35, 50).Contains(new Point(x, y)))
                {
                    if (_page > 0)
                        _page -= 1;
                    return true;
                }

                if (new Rectangle(_displayRect.Right - 35, _displayRect.Top + 30 + _height * 3 - 40, 35, 50).Contains(new Point(x, y)))
                {
                    if (_page < _totalPages)
                        _page += 1;
                    return true;
                }
                int selected = Pos2Tile(x, y);
                if ((selected > -1) && (selected < _files.Count))
                {
                    _filename.text = _files[selected].name;
                    if (!_save)
                    {
                        HandleEvent(true, Backend.Events.ButtonPressed, Backend.Buttons.Close);
                    }
                    else
                    {
                        _checked = selected;
                    }
                }
            }
            return base.OnMouseDown(button);
        }
        public int Pos2Tile(int x, int y)
        {
            int result = -1;
            if (_displayRect.Contains(x, y))
            {
                y -= (_displayRect.Top + 30);
                y = y / (_height + 3);
                if (y >= 0)
                    result = y + (_page * _rows);
            }
            if ((x > _displayRect.Right - 30) || (result > _files.Count - 1)) return -1;

            return result;
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _totalPages = (int)Math.Ceiling((float)_files.Count / (float)_rows);

            if (_visible)
            {
                int _selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
                _spriteBatch.Begin();
                int icon = _page * _rows;

                for (int y = 0; y < _rows; ++y)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 10, _displayRect.Top + 30 + y * (_height + 5), _displayRect.Width - 40, _height + 2), new Rectangle(39, 6, 1, 1), Color.White);
                    if (icon != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 11, _displayRect.Top + y * (_height + 5) + 31, _displayRect.Width - 42, _height), new Rectangle(39, 6, 1, 1), (_checked == icon) ? Color.Blue : Color.Black);



                    if ((icon < _files.Count) && (_files[icon].screenshot != null))
                    {
                        _spriteBatch.Draw(_files[icon].screenshot, new Rectangle(_displayRect.Left + 15, _displayRect.Top + y * (_height + 5) + 35, 90, 90), new Rectangle(0, 0, 200, 200), Color.White);
                        _spriteBatch.DrawString(_font, _files[icon].name + " " + _files[icon].dateTime, new Vector2(_displayRect.Left + _height + 20, _displayRect.Top + y * (_height + 3) + 56), Color.Black);
                        _spriteBatch.DrawString(_font, _files[icon].name + " " + _files[icon].dateTime, new Vector2(_displayRect.Left + _height + 21, _displayRect.Top + y * (_height + 3) + 57), Color.White);

                    }
                    ++icon;
                }

                if (_totalPages > 1)
                {
                    if (_page > 0)
                        _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 27, _displayRect.Top + 30, 22, 22), new Rectangle(32, 0, 28, 28), Color.White);
                    if (_page < _totalPages - 1)
                        _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 27, _displayRect.Top + 30 + _height * 3 - 22, 22, 22), new Rectangle(0, 0, 28, 28), Color.White);
                }
                _spriteBatch.End();

            }


        }

        public override void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {
            switch (eventID)
            {
                case Backend.Events.ButtonPressed:
                    {
                        switch ((Backend.Buttons)(int)data[0])
                        {
                            case Backend.Buttons.Close:
                                _parent.HandleEvent(true, Backend.Events.SaveLoad, _save, _filename.text);
                                break;
                            case Backend.Buttons.Cancel:
                                _parent.HandleEvent(true, Backend.Events.ContinueGame);
                                break;
                        }
                    }
                    break;
                case Backend.Events.TextEntered:
                    _parent.HandleEvent(true, Backend.Events.SaveLoad, _save, _filename.text);
                    break;
            }
            base.HandleEvent(DownStream, eventID, data);
        }

        public FileDialog(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, bool save)
            : base(parent, spriteBatch, content, displayRect)
        {
            _save = save;
            _BuildList();
            AddChild(_filename = new TextInput(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 10, _displayRect.Top + 5, _displayRect.Width - 40, 20), "Name:", ((save) ? "Savegame" : _files[0].name), "Enter a name for your savegame", 20, save));
            AddChild(_ok = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Right - 115, _displayRect.Bottom - 40, 85, 30), ((save) ? "Save" : "Restore"), (int)Backend.Buttons.Close));
            AddChild(_cancel = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 10, _displayRect.Bottom - 40, 85, 30), "Cancel", (int)Backend.Buttons.Cancel));
            _background = _content.Load<Texture2D>("Minimap");
            _arrows = _content.Load<Texture2D>("Arrows");
            _font = _content.Load<SpriteFont>("SmallFont");
        }
    }
}
