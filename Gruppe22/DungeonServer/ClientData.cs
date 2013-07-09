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
        private NetConnection _connection = null;

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
            Connection = connection;
        }

        public ClientData(short id, string guid, int actorID=0)
        {
            ID = id;
            _guid = guid;
            _actorID = actorID;
        }

        public short ID { get; private set; }
        public NetConnection Connection { get; private set; }


        public static short GetFreeID(IDictionary<IPEndPoint, ClientData> clients)
        {
            List<short> usedIds = (from client in clients select client.Value.ID).ToList();
            if (usedIds.Count == 0) return 0;

            for (short id = 0; id <= usedIds.Count; id++)
                if (!usedIds.Contains(id))
                    return id;

            return -1;
        }
    }

}

