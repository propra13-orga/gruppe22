using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    /// <summary>
    /// Different type of Quest steps
    /// </summary>
    public enum QuestStepType
    {
        /// <summary>
        /// A quest step which must be finished in order to get to the next step (default)
        /// </summary>
        Normal,
        /// <summary>
        /// A quest step which may be finished but may also be replaced by any of the following steps marked as "Alternative" or "Normal"
        /// </summary>
        Alternative,
        /// <summary>
        /// An optional quest step, which will be blocked once the next step is finished
        /// </summary>
        Optional
    }

    public enum ChangeType{
        Start,
        Finish,
        Block
    }

    /// <summary>
    /// Enumeration to distinguish quest types
    /// </summary>
    public enum QuestType
    {
        /// <summary>
        /// Empty requirement (e.g. step is immediately highlighted as successful; quest is available to everyone)
        /// </summary>
        None,
        /// <summary>
        ///  Find one / many items of a specified type
        /// </summary>
        Item,
        /// <summary>
        /// Kill one / many enemies of a specified type
        /// </summary>
        Enemy,
        /// <summary>
        /// Talk to an NPC (and raise disposition etc.)
        /// </summary>
        TalkNPC,
        /// <summary>
        /// Kill an NPC
        /// </summary>
        KillNPC,
        /// <summary>
        /// Go to a specific location (map / coords)
        /// </summary>
        Location,
        /// <summary>
        /// Require a specified flag to be set (e.g. another quest finished)
        /// </summary>
        Flag,
        /// <summary>
        /// Require the player to reach a specific level
        /// </summary>
        Level
    };

    /// <summary>
    /// Current status of a quest / quest step
    /// </summary>
    public enum QuestProgress
    {
        /// <summary>
        /// The quest / quest step has not yet been started
        /// </summary>
        NotStarted = 0,
        /// <summary>
        /// The quest is currently in progress
        /// </summary>
        Working,
        /// <summary>
        /// The quest goal has been reached
        /// </summary>
        Finished,
        /// <summary>
        /// The quest step cannot be finished anymore (branching quest lines)
        /// </summary>
        Blocked
    }



    /// <summary>
    /// Requirement to meet to finish a quest step
    /// </summary>
    public class QuestRequirement
    {
        /// <summary>
        /// Type of requirement (e.g. monsters killed, items found, etc.)
        /// </summary>
        private QuestType _type;

        /// <summary>
        /// The quantity (e.g. number of monsters, number of items) needed
        /// </summary>
        private int _quantity;

        /// <summary>
        /// The ID identifying the type of enemy or item needed or NPC to talk to; -1 for "any"
        /// </summary>
        private int _id;

        /// <summary>
        /// A losely specified type of item (need at least type; possibly also a list of effects, e.g. strong healing potion)
        /// </summary>
        private Item _item;

        /// <summary>
        /// Amount of needed item acquired / enemies killed at quest start (to prevent immediate success)
        /// </summary>
        private int _start = 0;

        public QuestRequirement(QuestType type = QuestType.None, int id = 0, int quantity = 0, Item item = null, int start = 0)
        {

        }
    }

    /// <summary>
    /// Changes occuring as part of a quest
    /// </summary>
    public abstract class MapChange
    {
        /// <summary>
        /// The id of the map where the change should occur
        /// </summary>
        protected int _roomID;

        /// <summary>
        /// The id of the map where the change should occur
        /// </summary>
        public int roomID
        {
            get
            {
                return _roomID;
            }
            set
            {
                _roomID = value;
            }
        }

    }

    /// <summary>
    /// The reward provided on quest completion to player.
    /// </summary>
    public class PlayerChange:MapChange
    {
        /// <summary>
        /// The experience granted (or deducted) from the player
        /// </summary>
        private int _xp;
        /// <summary>
        /// The items the player gains
        /// </summary>
        private List<Item> _items;
        /// <summary>
        /// Items a player loses (specified by Type, Ability and (minimum) strength)
        /// </summary>
        private List<Item> _loseItems;
        /// <summary>
        /// Any flags to set for further quests
        /// </summary>
        private HashSet<Int32> _flags;
        /// <summary>
        /// Get the experience received for performing a quest(step)
        /// </summary>
        public int xp { get { return _xp; } }


        #region LoseItems
        /// <summary>
        /// A (loosely) specified list of items to remove
        /// </summary>
        public Item[] loseItems
        {
            get
            {
                return _loseItems.ToArray();
            }
        }


        /// <summary>
        /// Add a new item to the list to be removed from the player
        /// </summary>
        /// <param name="item"></param>
        public void AddLoseItem(Item item)
        {
            _loseItems.Add(item);
        }

        /// <summary>
        /// Add a whole list of items to be removed from  the player
        /// </summary>
        /// <param name="item"></param>
        public void AddLoseItem(Item[] item)
        {
            _loseItems.AddRange(item);
        }

        /// <summary>
        /// Reset the list of items to be removed from the player
        /// </summary>
        public void ClearLoseItems()
        {
            _loseItems.Clear();
        }

        /// <summary>
        /// Remove an item from the list of items to be granted to the player
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveLoseItem(int ID)
        {
            if ((ID > -1) && (ID < _items.Count))
            {
                _loseItems.RemoveAt(ID);
            }
        }


        /// <summary>
        /// Remove an item from the list of items to be granted to the player
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveLoseItem(Item item)
        {
            _loseItems.Remove(item);
        }
        #endregion

        #region Items
        /// <summary>
        /// A fully specified list of items the player will receive
        /// </summary>
        public Item[] items
        {
            get
            {
                return _items.ToArray();
            }
        }

        /// <summary>
        /// Add a new item to the list to be granted to the player
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// Add a whole list of items to be granted to the player
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item[] item)
        {
            _items.AddRange(item);
        }

        /// <summary>
        /// Reset the list of items to be granted to the player
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
        }

        /// <summary>
        /// Remove an item from the list of items to be granted to the player
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveItem(int ID)
        {
            if ((ID > -1) && (ID < _items.Count))
            {
                _items.RemoveAt(ID);
            }
        }


        /// <summary>
        /// Remove an item from the list of items to be granted to the player
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveItem(Item item)
        {
            _items.Remove(item);
        }
        #endregion

        #region Flags

        /// <summary>
        /// Get the flags
        /// </summary>
        public Int32[] flags
        {
            get
            {
                return _flags.ToArray();
            }
        }

        /// <summary>
        /// Add a flag on finishing the quest
        /// </summary>
        /// <param name="flag"></param>
        public void AddFlag(Int32 flag)
        {
            _flags.Add(flag);
        }

        /// <summary>
        /// Add a whole list of flags
        /// </summary>
        /// <param name="flags"></param>
        public void AddFlag(Int32[] flags)
        {
            foreach (Int32 value in flags)
            {
                _flags.Add(value);
            }
        }

        /// <summary>
        /// Reset the list of flags
        /// </summary>
        public void ClearFlags()
        {
            _flags.Clear();
        }

        /// <summary>
        /// Remove a specific flag
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveFlag(int ID)
        {
            _flags.Remove(ID);
        }

        #endregion
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xp"></param>
        /// <param name="items"></param>
        /// <param name="flags"></param>
        public PlayerChange(int xp = 0, List<Item> items = null, List<Item> loseItems = null, Int32[] flags = null)
        {
            _xp = xp;
            _items = (items == null) ? new List<Item>() : items;
            _loseItems = (loseItems == null) ? new List<Item>() : loseItems;
            _flags = new HashSet<Int32>();

            if (flags != null)
            {
                foreach (Int32 value in flags)
                {
                    _flags.Add(value);
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// Changes in (other) Quests/queststeps (for branching questlines)
    /// </summary>
    public class QuestChange : MapChange
    {
        /// <summary>
        /// A unique ID for the quest
        /// </summary>
        private int _quest;
        /// <summary>
        /// The NPC who offered the quest
        /// </summary>
        private int _npc;
        /// <summary>
        /// The step in the quest to change
        /// </summary>
        private int _queststep;

        /// <summary>
        /// The unique ID of the quest in current game
        /// </summary>
        public int quest
        {
            get
            {
                return _quest;
            }
            set
            {
                _quest = value;
            }
        }

        /// <summary>
        /// The step of the Quest to change
        /// </summary>
        public int queststep
        {
            get
            {
                return _queststep;
            }
            set
            {
                _queststep = value;
            }
        }

        /// <summary>
        /// The NPC (originally) offering the quest
        /// </summary>
        public int npc
        {
            get
            {
                return _npc;
            }
            set
            {
                _npc = value;
            }
        }
    }

    /// <summary>
    /// Changes to an NPC as part of a quest
    /// </summary>
    public class NPCChange : MapChange
    {
        /// <summary>
        /// The NPC to modify
        /// </summary>
        private int _npc;
        /// <summary>
        /// Items to give to the NPC
        /// </summary>
        private List<Item> _items;
        /// <summary>
        /// Increases / Decreases Health
        /// </summary>
        private int _health;
        /// <summary>
        /// Increases / decreases affection for the player
        /// </summary>
        private int _love;
        /// <summary>
        /// Adds new dialog lines
        /// </summary>
        private List<DialogLine> _dialog;
        /// <summary>
        /// Removes dialog lines
        /// </summary>
        private List<Int32> _removeDialog;
        /// <summary>
        /// Removes items
        /// </summary>
        private List<Item> _removeItem;

        /// <summary>
        /// Get/Set the id of the NPC to change
        /// </summary>
        public int npc
        {
            get
            {
                return _npc;
            }
            set
            {
                _npc = value;
            }
        }

        /// <summary>
        /// Get a list of items to give to the NPC
        /// </summary>
        public List<Item> items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }

        }

        /// <summary>
        /// Add new dialog lines to the NPC
        /// </summary>
        public List<DialogLine> dialog
        {
            get
            {
                return _dialog;
            }
            set
            {
                _dialog = value;
            }
        }

        /// <summary>
        /// Remove dialog lines from the NPC
        /// </summary>
        public List<Int32> removeDialog
        {
            get
            {
                return _removeDialog;
            }
            set
            {
                _removeDialog = value;
            }
        }

        /// <summary>
        /// Increase/Decrease Health
        /// </summary>
        public int health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        /// <summary>
        /// Increase/decrease affection for player
        /// </summary>
        public int love
        {
            get
            {
                return _love;
            }
            set
            {
                _love = value;
            }
        }

    }

    /// <summary>
    /// Add objects as part of a quest
    /// </summary>
    public class AddObject : MapChange
    {
        /// <summary>
        /// The coordinates where tile will be added
        /// </summary>
        private Coords _coords;
        /// <summary>
        /// A tile-object (WallTile, ActorTile, etc.)
        /// </summary>
        private Tile _tile;
        /// <summary>
        /// The coordinates to which the tile will be added
        /// </summary>
        public Coords coords
        {
            get
            {
                return _coords;
            }
            set
            {
                _coords = value;
            }
        }
        /// <summary>
        /// The (completely specified) tile to add to the map at specified coordinates
        /// </summary>
        public Tile tile
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = value;
            }
        }
    }

    /// <summary>
    /// Remove objects as part of a quest
    /// </summary>
    public class RemoveObject : MapChange
    {
        /// <summary>
        /// Coordinates of tile to remove (irrelevant to actortiles, which are tracked on the map)
        /// </summary>
        private Coords _coords;


        /// <summary>
        /// An actortile / walltile / itemtile / traptile / etc. to remove 
        /// </summary>
        private Tile _tile;

        /// <summary>
        /// Coordinates of tile to remove 
        /// </summary>
        /// <remarks>(relevant mainly to walltiles; itemtiles not on tile are ignored; actortiles are tracked)</remarks>
        public Coords coords
        {
            get
            {
                return _coords;
            }
            set
            {
                _coords = value;
            }
        }

        /// <summary>
        /// The tile to remove (only needs basic information)
        /// </summary>
        public Tile tile
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = value;
            }
        }
    }

    public class QuestStep
    {
        /// <summary>
        /// Current state of the step
        /// </summary>
        private QuestProgress _status = QuestProgress.NotStarted;
        /// <summary>
        /// General description of the step (displayed as Title in quest log)
        /// </summary>
        private string _description;
        /// <summary>
        /// Text explaining the objective
        /// </summary>
        private string _objective;
        /// <summary>
        /// Text to display on completion
        /// </summary>
        private string _completion;
        /// <summary>
        /// Map changes occuring when quest is started (e.g. monsters appear, NPCs offer different dialog, etc.)
        /// </summary>
        private List<MapChange> _starteffect;
        /// <summary>
        /// Map changes occuring when quest is successfully finished (e.g. monsters appear, NPCs offer different dialog, etc.)
        /// </summary>
        private List<MapChange> _endeffect;
        /// <summary>
        /// Map changes occuring when quest is failed (e.g. monsters appear, NPCs offer different dialog, etc.)
        /// </summary>
        private List<MapChange> _blockeffect;
        /// <summary>
        /// A list of requirements to be met in order to complete step
        /// </summary>
        private List<QuestRequirement> _requirements;
        /// <summary>
        /// Special type of step: Optional, alternative
        /// </summary>
        private QuestStepType _type = QuestStepType.Normal;

        /// <summary>
        /// Check whether all requirements are met by a specified actor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool Completed(Actor actor)
        {
            return false;
        }

        /// <summary>
        /// Return the type of quest step (optional/alternate/normal)
        /// </summary>
        public QuestStepType type{
            get{
                return _type;
            }
            set{
                _type=value;
            }
        }


        /// <summary>
        /// Return all effects to be applied when starting this step in the quest
        /// </summary>
        public MapChange[] startEffect{
        get{
        return _starteffect.ToArray();
        }
        }

        /// <summary>
        /// Return all effects to be applied when this step in the quest is finished
        /// </summary>
        public MapChange[] endEffect{
        get{
        return _endeffect.ToArray();        
        }
        }

        /// <summary>
        /// Return all effects to be applied when this step in the quest is blocked (i.e. will be unavailable further on)
        /// </summary>
        public MapChange[] blockEffect{
        get{
                return _blockeffect.ToArray();        

        }
        }

        /// <summary>
        /// Add an effect to be applied on map state change
        /// </summary>
        /// <param name="change"></param>
        /// <param name="type"></param>
        public void AddChange(MapChange change, ChangeType type){
            switch(type){
            case ChangeType.Block:
            _blockeffect.Add(change);
                break;
            case ChangeType.Finish:
                _endeffect.Add(change);
                break;
            case ChangeType.Start:
                _blockeffect.Add(change);
                break;
            }
    
    }

        /// <summary>
        /// Clear all changes which would happen on specified map state changes
        /// </summary>
        /// <param name="type"></param>
        public void ClearChanges(ChangeType type){
        switch(type){
            case ChangeType.Block:
            _blockeffect.Clear();
                break;
            case ChangeType.Finish:
                _endeffect.Clear();
                break;
            case ChangeType.Start:
                _starteffect.Clear();
                break;
            }
        }

        /// <summary>
        /// Remove a specific effect from the list of map state effects
        /// </summary>
        /// <param name="i"></param>
        /// <param name="type"></param>
        public void RemoveChange(int i, ChangeType type){
        switch(type){
            case ChangeType.Block:
                _blockeffect.RemoveAt(i);
                break;
            case ChangeType.Finish:
                                _endeffect.RemoveAt(i);
                break;
            case ChangeType.Start:
                _starteffect.RemoveAt(i);
                break;
            }
        }


        /// <summary>
        /// Remove a specific effect from the list of map state effects
/// </summary>
/// <param name="change"></param>
/// <param name="type"></param>
        public void RemoveChange(MapChange change, ChangeType type){
        switch(type){
            case ChangeType.Block:
                _blockeffect.Remove(change);
                break;
            case ChangeType.Finish:
            _endeffect.Remove(change);
                break;
            case ChangeType.Start:
                _starteffect.Remove(change);
                break;
            }
        }

        /// <summary>
        /// Add a specific requirement to be performed before queststep is finished
        /// </summary>
        /// <param name="requirement"></param>
        public void AddRequirement(QuestRequirement requirement){
        _requirements.Add(requirement);
        }

        
        /// <summary>
        /// Remove all requirements from the list of requirements
        /// </summary>
        /// <param name="q"></param>
        public void ClearRequirements(){
        _requirements.Clear();
        }

        
        /// <summary>
        /// Remove a specific requirement from the list of requirements
        /// </summary>
        /// <param name="q"></param>
        public void RemoveRequirement(int i){
        
        }

        /// <summary>
        /// Remove a specific requirement from the list of requirements
        /// </summary>
        /// <param name="q"></param>
        public void RemoveRequirement(QuestRequirement q){
        
        }

        /// <summary>
        /// Get / set current status of quest step
        /// </summary>
        public QuestProgress status{
            get{
                return _status;
            }
            set{
                _status=value;
            }
        }

        /// <summary>
        /// Get / set (short) title of quest
        /// </summary>
        public string text{
            get{
                return _description;
            }
            set{
            _description=value;
            }
        }

        /// <summary>
        /// Get / set (extensive) text to be displayed when step is begun
        /// </summary>
        public string objective{
            get{
                return _objective;
            }
            set{_objective=value;}
        }

        /// <summary>
        /// Get / Set text to be displayed on quest completion
        /// </summary>
        public string completion{
            get{
                return _completion;
            }
            set{
                _completion=value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description"></param>
        /// <param name="objective"></param>
        /// <param name="completion"></param>
        /// <param name="type"></param>
        /// <param name="requirements"></param>
        /// <param name="starteffect"></param>
        /// <param name="endeffect"></param>
        /// <param name="blockeffect"></param>
        public QuestStep(string description = "",
            string objective = "",
            string completion = "",
            QuestStepType type = QuestStepType.Normal,
            List<QuestRequirement> requirements = null,
            List<MapChange> starteffect = null,
            List<MapChange> endeffect = null,
            List<MapChange> blockeffect = null)
        {
            _description = description;
            _objective = objective;
            _completion = completion;
            _type=type;
            _requirements=(requirements==null)? new List<QuestRequirement>() : requirements;
            _starteffect = (starteffect == null) ? new List<MapChange>() : starteffect;

            _endeffect = (endeffect == null) ? new List<MapChange>() : endeffect;
            _blockeffect = (blockeffect == null) ? new List<MapChange>() : blockeffect;

        }
    }



    /// <summary>
    /// A class representing a (multiple step) quest in the game.
    /// </summary>
    public class Quest
    {

        #region Private Fields
        /// <summary>
        /// An object to pass relevant effects to (e.g. information regarding quest completion, changes on quest progress...)
        /// </summary>
        private IHandleEvent _parent=null;
        /// <summary>
        /// A title briefly describing the quest
        /// </summary>
        private string _description = "A quest";
        /// <summary>
        /// A more extensive description of the quest
        /// </summary>
        private string _objective = "Finish the quest!";
        /// <summary>
        /// Text to display on quest completion
        /// </summary>
        private string _completion = "You successfully finished the quest.";
        /// <summary>
        /// The current progress in the quest
        /// </summary>
        private QuestProgress _status = QuestProgress.NotStarted;
        /// <summary>
        /// A list of individual steps to perform in order to finish the quest(line)
        /// </summary>
        private List<QuestStep> _steps = null;
        /// <summary>
        /// The current step in the quest
        /// </summary>
        private int _currentStep = 0;
        /// <summary>
        /// A list of requirements to be met
        /// </summary>
        private List<QuestRequirement> _startreqs = null;
        /// <summary>
        /// Whether the quest can only be done once (default) or multiple times
        /// </summary>
        private bool _repeatable = false;
        #endregion

        #region Public Fields


        /// <summary>
        /// Description
        /// </summary>
        public string text
        {
            get
            {
                return _description;
            }
        }

        #endregion


        #region Public Methods
        /// <summary>
        /// Determine whether a specific actor has completed the quest or at least one or more steps in the quest
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool Completed(Actor actor){
            int temp=_currentStep;
        if((_status==QuestProgress.Working)&&
            (temp<_steps.Count)
        &&(temp>-1)){
        while(
            _steps[temp].Completed(actor)
            ||(temp==_currentStep)
            ||(_steps[temp].type!=QuestStepType.Normal)){
            if(_steps[temp].Completed(actor)){
            _steps[temp].status=QuestProgress.Finished;
                // Apply effects of quest state change
            }
            temp+=1;
        }

        };
            return false;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for a quest
        /// </summary>
        /// <param name="text"></param>
        /// <param name="xp">The amount of exp granted after completing the quest.</param>
        /// <param name="itemlist">A list of items the player will be rewarded with for completing the quest</param>
        public Quest()
        {
        }

        #endregion
    }
}
