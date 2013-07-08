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
                                _parent.HandleEvent(true, Backend.Events.ContinueGame);
                                return;
                            case Backend.Buttons.StartServer:
                                System.Diagnostics.Process.Start("DungeonServer.exe");
                                return;
                            case Backend.Buttons.Cancel:
                                _network.Disconnect();
                                _parent.HandleEvent(true, Backend.Events.ContinueGame);
                                return;
                            case Backend.Buttons.Connect:
                                _connect.Hide();
                                if (_connect.label == "Connect")
                                {
                                    _network.playername = _playerName.text;
                                    _network.server = _ipEntry.text;
                                    _connect.label = "Disconnect";
                                }
                                else
                                {
                                    _connect.label = "Connect";
                                    _network.Disconnect();
                                }
                                return;
                        }
                        break;
                    case Backend.Events.ShowMessage:
                        if (data[0].ToString().ToLower().Contains("connected"))
                        {
                            _connect.Show();
                        }
                        _listPlayers.AddLine(data[0].ToString(), data.Length > 1 ? data[1] : null);
                        return;
                }
            }
            base.HandleEvent(DownStream, eventID, data);
        }

        public Lobby(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {

            string myGUID = "";
            if (Properties.Settings.Default.guid != "NONE")
            {
                myGUID = Properties.Settings.Default.guid;
            }
            else {
                myGUID = Guid.NewGuid().ToString();
            Properties.Settings.Default.guid = myGUID;
            Properties.Settings.Default.Save();
            }
                _ipEntry = new TextInput(this, spriteBatch, content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, 500, 30), "Server:", "localhost", "Enter a server to connect to or localhost to test locally", -1, true);
            _playerName = new TextInput(this, spriteBatch, content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 50, _displayRect.Width - 250, 30), "Computer-ID:", myGUID, "Enter a unique ID for your computer", -1, true);
            _listPlayers = new Statusbox(this, spriteBatch, content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 100, _displayRect.Width - 10, _displayRect.Height - 160), true, true);
            _ok = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Right - 90, _displayRect.Bottom - 50, 80, 40), "Ok", (int)Backend.Buttons.Close);
            _launchServer = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Right - 210, _displayRect.Top + 5, 200, 40), "Launch Server", (int)Backend.Buttons.StartServer);
            _cancel = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Left + 10, _displayRect.Bottom - 50, 80, 40), "Cancel", (int)Backend.Buttons.Cancel);
            _connect = new Button(this, spriteBatch, content, new Rectangle(_displayRect.Right - 160, _displayRect.Top + 50, 150, 40), "Connect", (int)Backend.Buttons.Connect);
            _network = new NetPlayer(this);
            _children.Add(_ipEntry);
            _children.Add(_launchServer);
            _children.Add(_playerName);
            _children.Add(_connect);
            _children.Add(_listPlayers);
            _children.Add(_ok);
            _children.Add(_cancel);
            // IP-Entry-Field (Dropdown?)
            // Button to launch server
            // Entry fField for player name
            // List of players
        }

    }
}
