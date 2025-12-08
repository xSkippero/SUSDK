using SUSDK.Domain;
using UnityEngine;

namespace SUSDK.Helper
{
    public class WeaponHelper
    {
        private Weapon _weapon;
        
        public WeaponHelper CreateDemoWeapon(string weaponName, string fireSound = "shoot", string overheatSound = "overheat")
        {
            _weapon = new Weapon
            {
                Name = weaponName,
                DamageType = DamageType.Physical,
                FireSound = fireSound,
                OverheatSound = overheatSound,
                ProjectileDamage = 10f,
                ProjectileSpeed = 200f,
                ProjectileSize = 0.5f,
                ProjectileAmount = 1f,
                ProjectileSpread = 0.2f,
                FireRpm = 750f,
                HeatPerShot = 1f,
                MaxHeat = 30,
                HeatReductionPerSecond = 1f,
                ConsumesAmmo = false
            };
            return this;
        }

        public WeaponHelper CreateWeapon(string weaponName)
        {
            _weapon = new Weapon
            {
                Name = weaponName
            };
            return this;
        }
        
        public WeaponHelper WithName(string name)
        {
            _weapon.Name = name;
            return this;
        }
        
        public WeaponHelper WithEmptySound(string sound)
        {
            _weapon.WeaponEmptySound = sound;
            return this;
        }
        
        public WeaponHelper WithOverheatSound(string sound)
        {
            _weapon.OverheatSound = sound;
            return this;
        }

        public WeaponHelper WithGrade(float grade)
        {
            _weapon.Grade = grade;
            return this;
        }

        public WeaponHelper WithProjectileType(DamageType type)
        {
            _weapon.DamageType = type;
            return this;
        }

        public WeaponHelper WithProjectileDamage(float damage)
        {
            _weapon.ProjectileDamage = damage;
            return this;
        }

        public WeaponHelper WithProjectileSpeed(float speed)
        {
            _weapon.ProjectileSpeed = speed;
            return this;
        }

        public WeaponHelper WithProjectileSize(float size)
        {
            _weapon.ProjectileSize = size;
            return this;
        }

        public WeaponHelper WithProjectileAmount(float amount)
        {
            _weapon.ProjectileAmount = amount;
            return this;
        }

        public WeaponHelper WithProjectileSpread(float spread)
        {
            _weapon.ProjectileSpread = spread;
            return this;
        }

        public WeaponHelper WithRpm(float rpm)
        {
            _weapon.FireRpm = rpm;
            return this;
        }

        public WeaponHelper WithReloadCooldown(float cooldown)
        {
            _weapon.ReloadCooldown = cooldown;
            return this;
        }

        public WeaponHelper WithHeatPerShot(float heat)
        {
            _weapon.HeatPerShot = heat;
            return this;
        }

        public WeaponHelper WithFireSound(string sound)
        {
            _weapon.FireSound = sound;
            return this;
        }

        public WeaponHelper WithReloadSound(string sound)
        {
            _weapon.ReloadSound = sound;
            return this;
        }

        public WeaponHelper WithConsumesAmmo(bool consumesAmmo)
        {
            _weapon.ConsumesAmmo = consumesAmmo;
            return this;
        }

        public WeaponHelper WithAmmoPerShot(float amount)
        {
            _weapon.AmmoPerShot = amount;
            return this;
        }

        public WeaponHelper WithMaxAmmo(float maxAmmo)
        {
            _weapon.MaxAmmo = maxAmmo;
            return this;
        }

        public WeaponHelper WithCurrentAmmo(float currentAmmo)
        {
            _weapon.CurrentAmmo = currentAmmo;
            return this;
        }

        public WeaponHelper WithMaxHeat(float maxHeat)
        {
            _weapon.MaxHeat = maxHeat;
            return this;
        }

        public WeaponHelper WithCurrentHeat(float currentHeat)
        {
            _weapon.CurrentHeat = currentHeat;
            return this;
        }

        public WeaponHelper WithHeatReductionPerSecond(float reduction)
        {
            _weapon.HeatReductionPerSecond = reduction;
            return this;
        }

        public Weapon Build()
        {
            return _weapon;
        }
    }
}
