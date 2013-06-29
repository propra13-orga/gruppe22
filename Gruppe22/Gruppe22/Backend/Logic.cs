using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{
    public class Logic : IHandleEvent
    {
        protected Map _map;
        protected IHandleEvent _parent;
        protected Random _random;

        public Map map
        {
            get { return _map; }
            set { _map = value; }
        }

        public virtual void GenerateMaps()
        {

        }


        public virtual void Update(GameTime gametime)
        {

        }

        public virtual void ChangeMap(string filename, Coords pos)
        {

        }

        public Logic(IHandleEvent parent, Map map = null, Random _random = null)
        {
            _parent = parent;
            if (map != null)
            {
                _map = map;
            }
            if (_random == null) _random = new Random();
        }

        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
        }
    }
}
