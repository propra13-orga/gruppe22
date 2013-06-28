using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public class Logic:IHandleEvent
    {
        private Map _map;
        private IHandleEvent _parent;
        private Random _random;

        public Map map
        {
            get { return _map; }
            set { _map = value; }
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
    }
}
