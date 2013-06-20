using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Gruppe22
{
    public class ClientData
    {
        public ClientData(NetConnection connection, short id)
        {
            Connection = connection;
            ID = id;
        }

        public ClientData(short id)
        {
            ID = id;
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

