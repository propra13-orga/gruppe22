using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public enum ItemProperty
    {
        Evade = 0,
        Block = 1,
        Penetrate = 2,
        ReduceDamage = 3,
        MaxHealth = 4,
        MaxMana = 5,
        Mana = 6,
        Health = 7,
        ManaRegen = 8,
        HealthRegen = 9,
        StealHealth = 10,
        StealMana = 11,
        FireDamage = 12,
        IceDamage = 13,
        FireProtect = 14,
        IceProtect = 15,
        DestroyWeapon = 16,
        DestroyArmor = 17,
        Resist = 18,
        Damage = 19,
        Null = -1
    }

    /// <summary>
    /// Effekt eines Item auf einer bestimmten Charakter-Statistik
    /// </summary>
    public class ItemEffect
    {
        /// <summary>
        /// Item-Effekt-Eigenschaft aus der Aufzählung.
        /// </summary>
        private ItemProperty _property = ItemProperty.Null;
        /// <summary>
        /// Effektwert.
        /// </summary>
        private int _effect = 0;
        /// <summary>
        /// Dauer des Item-Effektes.
        /// </summary>
        private int _duration = 0;
        /// <summary>
        /// Eigenschaft des Effektes.
        /// </summary>
        public int effect
        {
            get
            {
                return _effect;
            }
            set
            {
                _effect = value;
            }
        }
        /// <summary>
        /// Eigenschaft der Aufzählung Item-Eigenschaft.
        /// </summary>
        public ItemProperty property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }
        /// <summary>
        /// Eigenschaft zur Dauer.
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
        /// Konstruktor.
        /// </summary>
        /// <param name="property">Fähigkeit.</param>
        /// <param name="effect">Wirkung.</param>
        /// <param name="duration">Dauer.</param>
        public ItemEffect(ItemProperty property = ItemProperty.Null, int effect = 0, int duration = 0)
        {
            _property = property;
            _effect = effect;
            _duration = duration;
        }
    }
}
