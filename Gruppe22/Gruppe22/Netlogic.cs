using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public class NetLogic:Logic
    {
        public NetLogic(IHandleEvent parent, Map map = null, Random _random = null)
            : base(parent, map, _random)
        {
        }
    }
}
