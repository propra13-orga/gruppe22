using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A connection between two rooms
    /// </summary>
    public class Exit
    {
        /// <summary>
        /// Location where entrance is located
        /// </summary>
        private Backend.Coords _from;

        /// <summary>
        /// Location where exit is located
        /// </summary>
        private Backend.Coords _to;
        /// <summary>
        /// Filename of room where the entrance is located
        /// </summary>
        private string _fromRoom;
        /// <summary>
        /// Filename of room where the exit is located
        /// </summary>
        private string _toRoom;

        /// <summary>
        /// Read only accessor to coordinates of teleporter in entrance room
        /// </summary>
        public Backend.Coords from
        {
            get { return _from; }
        }

        /// <summary>
        /// Read only accessor to coordinates of teleporter in exit room
        /// </summary>
        public Backend.Coords to
        {
            get { return _to; }
        }

        /// <summary>
        /// Read only accessor to filename of room where the entrance is located
        /// </summary>
        public string fromRoom { get { return _fromRoom; } }


        /// <summary>
        /// Read only accessor to filename of room where the exit is located
        /// </summary>
        public string toRoom { get { return _toRoom; } }

        /// <summary>
        /// Constructor for Exit class
        /// </summary>
        /// <param name="from">Coordinates of teleporter in entrance room</param>
        /// <param name="fromRoom">Filename of entrance room</param>
        /// <param name="to">Coordinates of teleporter in exit room</param>
        /// <param name="toRoom">Filename of exit room</param>
        public Exit(Coords from, string fromRoom, Backend.Coords to = null, string toRoom = "")
        {
            _from = from;
            _fromRoom = fromRoom;
            _toRoom = toRoom;
            _to = to;

        }
    }
}
