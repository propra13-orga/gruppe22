using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace DungeonServer
{
    public class ClientData
    {
        private string _guid = "";
        private short _id = 0;
        private int _actorID = 0;
        private bool _paused = true;
        private NetConnection _connection = null;

        public bool paused
        {
            get
            {
                return _paused;
            }
            set
            {
                _paused = value;
            }
        }
        public int actorID
        {
            get
            {
                return _actorID;
            }
            set
            {
                _actorID = value;
            }
        }
        public string guid
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }

        public short id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public NetConnection connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public ClientData(NetConnection connection, short id, string guid, int actorID=0)
            : this(id, guid, actorID)
        {
            _connection = connection;
        }

        public ClientData(short id, string guid, int actorID=0)
        {
            _guid = guid;
            _actorID = actorID;
            _paused = true;
        }


    }

}

