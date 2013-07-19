using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Client
{
    class Lobby : Window
    {
        private TextInput _ipEntry;
        private Button _connect;
        private NetPlayer _network;
        private Button _launchServer;
        private Button _cancel;
        private Button _ok;
        private Statusbox _listPlayers;
        private TextInput _playerName;
        private bool _connecting = false;

        public override void Update(GameTime gameTime)
        {
            _network.Update(gameTime);
            base.Update(gameTime);
        }

        public NetPlayer network
        {
            get
            {
                return _network;
            }
            set
            {
                _network = value;
            }
        }

        public override void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {
            if (!DownStream)
            {
                switch (eventID)
                {
                    case Backend.Events.ButtonPressed:
                        switch ((Backend.Buttons)data[0])
                        {
                            case Backend.Buttons.Close:
                                _parent.HandleEvent(true, Backend.Events.Settings);
                                return;
                            case Backend.Buttons.StartServer:
                                System.Diagnostics.Process.Start("DungeonServer.exe");
                                if ((!network.connecting) && !_connecting)
                                {
                                    _connecting = true;
                                    _network.playername = _playerName.text;
                                    _network.server = _ipEntry.text;
                                    _launchServer.Hide();
                                    _connect.label = "Disconnect";
                                }
                                return;
                            case Backend.Buttons.Cancel:
                                _network.Disconnect();
                                _parent.HandleEvent(true, Backend.Events.ContinueGame);
                                return;
                            case Backend.Buttons.Connect:
                                _connect.Hide();
                                if (_connect.label == "Connect")
                                {
                                    _connecting = true;
                                    _network.playername = _playerName.text;
                                    _connect.label = "Disconnect";
                                    _launchServer.Hide();
                                    _network.server = _ipEntry.text;
                                }
                                else
                                {
                                    _connect.label = "Connect";
                                    _network.Disconnect();
                                    _launchServer.Show();
                                }
                                return;
                        }
                        break;
                    case Backend.Events.Disconnect:
                        if (!_connecting)
                            _parent.HandleEvent(true, Backend.Events.Settings);
                        else
                        {
                            _connect.label = "Connect";
                            _launchServer.Show();
                            _connect.Show();
                        }
                        break;
                    case Backend.Events.Connect:
                        _parent.HandleEvent(true, Backend.Events.Settings);
                        break;
                    case Backend.Events.ShowMessage:
                        if ((((string)data[0]).ToLower().Contains("guid")) && (((string)data[0]).ToLower().Contains("disconnected")))
                        {
                            foreach (UIElement child in _children)
                            {
                                child.Visible = false;
                            }
                            _children.Add(new YesNoDialog(this, _spriteBatch, _content, _displayRect, "Every client needs a unique GUID.\nYour GUID is already in use.\n Generate a new GUID?"));
                        }
                        _listPlayers.AddLine(data[0].ToString(), data.Length > 1 ? data[1] : null);
                        return;
                    case Backend.Events.TextEntered:
                        if (_focusID == 0) ChangeFocus();
                        else HandleEvent(false, Backend.Events.ButtonPressed, Backend.Buttons.Connect);
                        break;
                }


            }
            else
            {
                switch (eventID)
                {
                    case Backend.Events.ButtonPressed:
                        switch ((Backend.Buttons)data[0])
                        {
                            case Backend.Buttons.Yes:
                                _children.RemoveAt(_children.Count - 1);
                                foreach (UIElement child in _children)
                                {
                                    child.Visible = true;
                                }
                                _playerName.text = Guid.NewGuid().ToString();
                                HandleEvent(false, Backend.Events.ButtonPressed, Backend.Buttons.Connect);

                                return;
                            case Backend.Buttons.No:
                                _children.RemoveAt(_children.Count - 1);
                                foreach (UIElement child in _children)
                                {
                                    child.Visible = true;
                                }
                                return;

                        }
                        break;
                }
            }
            base.HandleEvent(DownStream, eventID, data);
        }

        public Lobby(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, NetPlayer netplayer = null)
            : base(parent, spriteBatch, content, displayRect)
        {

            string myGUID = "";
            if (Properties.Settings.Default.guid != "NONE")
            {
                myGUID = Properties.Settings.Default.guid;
            }
            else
            {
                myGUID = Guid.NewGuid().ToString();
                Properties.Settings.Default.guid = myGUID;
                Properties.Settings.Default.Save();
            }
            AddChild(_ipEntry = new TextInput(this, spriteBatch, content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 250, 30), "Server:", "localhost", "Enter a server to connect to or localhost to test locally", -1, true));
            AddChild(_playerName = new TextInput(this, spriteBatch, content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 50, _displayRect.Width - 250, 30), "Computer-ID:", myGUID, "Enter a unique ID for your computer", -1, true));
            AddChild(_listPlayers = new Statusbox(this, spriteBatch, content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 100, _displayRect.Width - 10, _displayRect.Height - 160), true, true));
            //_ok = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Right - 90, _displayRect.Bottom - 50, 80, 40), "Ok", (int)Backend.Buttons.Close);
            AddChild(_launchServer = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Right - 210, _displayRect.Top + 5, 200, 40), "Launch Server", (int)Backend.Buttons.StartServer));
            AddChild(_cancel = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Left + 10, _displayRect.Bottom - 50, 80, 40), "Cancel", (int)Backend.Buttons.Cancel));
            AddChild(_connect = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Right - 160, _displayRect.Top + 50, 150, 40), "Connect", (int)Backend.Buttons.Connect));
            if (netplayer == null)
            {
                _network = new NetPlayer(this);
                _network.playername = _playerName.text;
            }
            else
            {
                _launchServer.Hide();
                _network = netplayer;
                _network.parent = this;
                _ipEntry.text = _network.server;
                _playerName.text = _network.playername;
                if (netplayer.connected)
                {
                    _connect.label = "Disconnect";
                    _listPlayers.AddLine("Connected.", Color.Green);

                }
                else
                {
                    _connect.label = "Connect";
                    _listPlayers.AddLine("Disconnected.", Color.Red);
                }
            }
            // IP-Entry-Field (Dropdown?)
            // Button to launch server
            // Entry fField for player name
            // List of players
        }

    }
}
