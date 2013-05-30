using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
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
    /// Effect of an item on a certain character statistic
    /// </summary>
    public class ItemEffect
    {
        private ItemProperty _property = ItemProperty.Null;
        private int _effect = 0;
        private int _duration = 0;
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

        public ItemEffect(ItemProperty property = ItemProperty.Null, int effect = 0, int duration = 0)
        {
            _property = property;
            _effect = effect;
            _duration = duration;
        }
    }
}
