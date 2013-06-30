using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public class Story
    {
        private NPC _questgiver;
        private List<Quest> quests;

        public Story(NPC questnpc, List<Quest> gamequests)
        {
            this._questgiver = questnpc;
            this.quests = gamequests;
        }
    }
}
