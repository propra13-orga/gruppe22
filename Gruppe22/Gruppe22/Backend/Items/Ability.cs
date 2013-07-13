using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    [Flags]
    public enum AbilityTarget
    {
        None = 0,
        Missile = 1,
        Self = 2,
        Aura = 4,
        Item = 8,
        Map = 16,
        Explode = 32
    }

    [Flags]
    public enum AbilityElement
    {
        None = 0,
        Fire = 1,
        Ice = 2,
        Health = 4,
        ManaReg = 8,
        HealthReg = 16,
        Stun = 32,
        Teleport = 64,
        Morph = 128,
        Charm = 256,
        Scare = 512
    }

    /// <summary>
    /// The class used to create 
    /// </summary>
    public class Ability
    {
        private int _cost;
        private int _intensity;
        private int _duration;
        private int _cooldown;
        private int _currentCool;
        private AbilityTarget _target;
        private AbilityElement _element;
        private int _improveOver = -1;
        private ImageData _icon = null;
        private string _name = "";
        private string _description = "";

        public int improveOver
        {
            get
            {
                return _improveOver;
            }
            set
            {
                _improveOver = value;
            }
        }

        /// <summary>
        /// The duration of an ability
        /// e.g. how long an enemy is stunned
        /// </summary>
        public int duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }

        /// <summary>
        /// The description of an ability
        /// </summary>
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// The icon which represents the ability
        /// </summary>
        public ImageData icon { get { if (_icon == null)GetIcon(); return _icon; } set { _icon = value; } }
        
        /// <summary>
        /// The name of an ability
        /// </summary>
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// The current cooldown
        /// </summary>
        public int currentCool
        {
            get
            {
                return _currentCool;
            }
            set
            {
                _currentCool = value;
            }
        }

        /// <summary>
        /// The amount of mana needed to active the ability
        /// </summary>
        public int cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
            }
        }

        /// <summary>
        /// the streng of an ability
        /// e.g. how much a healing spell heals the actor
        /// </summary>
        public int intensity
        {
            get
            {
                return _intensity;
            }
            set
            {
                _intensity = value;
            }
        }

        /// <summary>
        /// The cooldown of an ability
        /// (the time between two uses of this ability in seconds)
        /// </summary>
        public int cooldown
        {
            get
            {
                return _cooldown;
            }
            set
            {
                _cooldown = value;
            }
        }

        /// <summary>
        /// The target of an ability
        /// e.g. the casting actor himself
        /// </summary>
        public AbilityTarget target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        /// <summary>
        /// Saving method
        /// </summary>
        /// <param name="xmlw"></param>
        public void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("Ability");
            if (_icon != null)
            {
                xmlw.WriteAttributeString("iconfile", Convert.ToString(_icon.name));
                xmlw.WriteAttributeString("iconoffsetX", Convert.ToString(_icon.offset.x));
                xmlw.WriteAttributeString("iconoffsetY", Convert.ToString(_icon.offset.y));
                xmlw.WriteAttributeString("iconcropX", Convert.ToString(_icon.crop.x));
                xmlw.WriteAttributeString("iconcropY", Convert.ToString(_icon.crop.y));
                xmlw.WriteAttributeString("clipRectX", Convert.ToString(_icon.rect.X));
                xmlw.WriteAttributeString("clipRectY", Convert.ToString(_icon.rect.Y));
                xmlw.WriteAttributeString("clipRectW", Convert.ToString(_icon.rect.Width));
                xmlw.WriteAttributeString("clipRectH", Convert.ToString(_icon.rect.Height));
            }
            xmlw.WriteAttributeString("name", Convert.ToString(name));
            xmlw.WriteAttributeString("description", Convert.ToString(_description));

            xmlw.WriteAttributeString("cost", Convert.ToString(_cost));
            xmlw.WriteAttributeString("intensity", Convert.ToString(_intensity));
            xmlw.WriteAttributeString("duration", Convert.ToString(_duration));
            xmlw.WriteAttributeString("cooldown", Convert.ToString(_cooldown));
            xmlw.WriteAttributeString("currentCool", Convert.ToString(_currentCool));
            xmlw.WriteAttributeString("target", Convert.ToString(_target));
            xmlw.WriteAttributeString("element", Convert.ToString(_element));


            xmlw.WriteEndElement();
        }

        /// <summary>
        /// Loading method
        /// </summary>
        /// <param name="reader"></param>
        public void Load(XmlReader reader)
        {
            _name = reader.GetAttribute("name");
            _icon = new ImageData(
                reader.GetAttribute("iconfile"), 
                new Rectangle(Convert.ToInt32(reader.GetAttribute("clipRectX")),
                Convert.ToInt32(reader.GetAttribute("clipRectY")),
                Convert.ToInt32(reader.GetAttribute("clipRectW")),
                Convert.ToInt32(reader.GetAttribute("clipRectH"))),
                new Coords(Convert.ToInt32(reader.GetAttribute("iconoffsetX")),
                Convert.ToInt32(reader.GetAttribute("iconoffsetY"))),
                new Coords(Convert.ToInt32(reader.GetAttribute("iconcropX")),
                Convert.ToInt32(reader.GetAttribute("iconcropY"))));
            _description = reader.GetAttribute("description");
            _cost = Convert.ToInt32(reader.GetAttribute("cost"));
            _intensity = Convert.ToInt32(reader.GetAttribute("intensity"));
            _duration = Convert.ToInt32(reader.GetAttribute("duration"));
            _cooldown = Convert.ToInt32(reader.GetAttribute("cooldown"));
            _currentCool = Convert.ToInt32(reader.GetAttribute("currentCool"));
            if (reader.GetAttribute("target") != null)
                _target = (AbilityTarget)Enum.Parse(typeof(AbilityTarget), reader.GetAttribute("target"));
            if (reader.GetAttribute("element") != null)
                _element = (AbilityElement)Enum.Parse(typeof(AbilityElement), reader.GetAttribute("element"));

        }

        public AbilityElement element
        {
            get
            {
                return _element;
            }
            set
            {
                _element = value;
            }
        }

        /// <summary>
        /// Method to generate the name of an ability
        /// The name is determined by the target and element
        /// </summary>
        public void GenerateName()
        {
            switch (target)
            {
                case AbilityTarget.Aura:
                    name = "Aura";
                    break;

                case AbilityTarget.Explode:
                    name = "Circle";
                    break;

                case AbilityTarget.Item:
                    break;

                case AbilityTarget.Map:
                    name = "Target";
                    break;

                case AbilityTarget.Missile:
                    name = "Arrow";
                    break;

                case AbilityTarget.Self:
                    name = "Me";
                    break;
            }
            switch (element)
            {
                case AbilityElement.Teleport:
                    name = "Teleport";
                    break;

                case AbilityElement.Stun:
                    name = "Stun " + name;
                    break;

                case AbilityElement.Scare:
                    name = "Scare " + name;
                    break;

                case AbilityElement.Morph:
                    name = "Morph " + name;
                    break;

                case AbilityElement.ManaReg:
                    name = "Restore";
                    break;

                case AbilityElement.Ice:
                    name = "Ice " + name;
                    break;

                case AbilityElement.HealthReg:
                    name = "Regenerate";
                    break;

                case AbilityElement.Health:
                    name = "Heal";
                    break;

                case AbilityElement.Fire:
                    name = "Fire " + name;
                    break;

                case AbilityElement.Charm:
                    name = "Charm " + name;
                    break;
            }

        }

        /// <summary>
        /// Method to get the icon for an ability from data
        /// </summary>
        public void GetIcon()
        {
            switch (element)
            {
                case AbilityElement.Teleport:
                    _icon = new ImageData("items", new Rectangle(160, 735, 32, 32), new Coords(0,0), new Coords(0,0));
                    break;

                case AbilityElement.Stun:
                    _icon = new ImageData("items", new Rectangle(160, 768, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.Scare:
                    _icon = new ImageData("items", new Rectangle(160, 864, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.Morph:
                    _icon = new ImageData("items", new Rectangle(128, 767, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.ManaReg:
                    _icon = new ImageData("items", new Rectangle(160, 800, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.Ice:
                    _icon = new ImageData("items", new Rectangle(160, 863, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.HealthReg:
                    _icon = new ImageData("items", new Rectangle(160, 704, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.Health:
                    _icon = new ImageData("items", new Rectangle(160, 704, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.Fire:
                    _icon = new ImageData("items", new Rectangle(160, 834, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;

                case AbilityElement.Charm:
                    _icon = new ImageData("items", new Rectangle(128, 736, 32, 32), new Coords(0, 0), new Coords(0, 0));
                    break;
            }
        }

        /// <summary>
        /// The constructor for an ability setting default values.
        /// </summary>
        /// <param name="cost">by default 2</param>
        /// <param name="intensity">by default 1</param>
        /// <param name="duration">by default 0</param>
        /// <param name="cooldown">by default 5</param>
        /// <param name="target"></param>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public Ability( int cost = 2, int intensity = 1, int duration = 0, int cooldown = 5, AbilityTarget target = AbilityTarget.None, AbilityElement element = AbilityElement.None, string name = "", string description = "")
        {
            _cost = cost;
            _intensity = intensity;
            _duration = duration;
            _cooldown = cooldown;
            _target = target;
            _element = element;
            if (name != "")
                _name = name;
            else
                GenerateName();
            _description = description;
            GetIcon();
        }
    }
}
