using SUSDK.Entity;
using SUSDK.Helper;
using SUSDK.Manager;
using UnityEngine;

namespace SUSDK.Domain
{
    public class WeaponController : MonoBehaviour
    {
        public Weapon weaponInfo;
        private float _fireTimer;
        private float _timeSinceLastShot;
        private bool _isReloading;
        [HideInInspector]
        public bool isOverheating;
        
        private void LateUpdate()
        {
            if (_fireTimer > 0f)
                _fireTimer -= Time.deltaTime;
            
            if (_timeSinceLastShot >= 2f)
            {
                CoolDownWeapon(Time.deltaTime);
            }

            _timeSinceLastShot += Time.deltaTime;

            if(weaponInfo == null) return;
            
            if (!(weaponInfo.CurrentHeat > 0)) return;
            weaponInfo.CurrentHeat -= weaponInfo.HeatReductionPerSecond * Time.deltaTime;
            if (weaponInfo.CurrentHeat < 0) weaponInfo.CurrentHeat = 0;
        }
        
        public void TryFireWeapon(Vector3 aimPosition, bool isFiring)
        {
            if (_isReloading || isOverheating) return;

            if (!isFiring || !(_fireTimer <= 0f)) return;
            FireWeapon(aimPosition);
            _timeSinceLastShot = 0f;
        }
        
        private void FireWeapon(Vector3 aimPosition)
        {
            if (weaponInfo.ConsumesAmmo)
            {
                if (weaponInfo.LoadedAmmo <= 0 || weaponInfo.LoadedAmmo - weaponInfo.AmmoPerShot < 0)
                {
                    SoundManager.PlaySound(weaponInfo.WeaponEmptySound);
                    return;
                }
                weaponInfo.LoadedAmmo -= weaponInfo.AmmoPerShot;   
            }
            else
            {
                if (weaponInfo.CurrentHeat >= weaponInfo.MaxHeat) 
                {
                    SoundManager.PlaySound(weaponInfo.OverheatSound, ignoresOtherPlaying: true);
                    isOverheating = true;
                    return;
                }
                weaponInfo.CurrentHeat += weaponInfo.HeatPerShot;
            }
            
            FireProjectile(aimPosition);
            
            SoundManager.PlaySound(weaponInfo.FireSound, ignoresOtherPlaying: true);
            
            _fireTimer = 60f / weaponInfo.FireRpm;
        }
        
        private void FireProjectile(Vector3 aimPosition)
        {
            var baseDirection = (aimPosition - transform.position).normalized;
            baseDirection.z = Camera.main!.orthographic ? 0 : 1; 
            var amount = Mathf.RoundToInt(weaponInfo.ProjectileAmount);
            for (var i = 0; i < amount; i++)
            {
                var spreadAngle = Random.Range(-weaponInfo.ProjectileSpread, weaponInfo.ProjectileSpread);
                Vector3 spreadDirection;
                if (Camera.main.orthographic)
                {
                    spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * baseDirection;
                }
                else
                {
                    spreadDirection = Quaternion.Euler(spreadAngle, spreadAngle, 0) * baseDirection;
                }
                SpawnProjectile(spreadDirection);
            }
        }
        
        private void SpawnProjectile(Vector3 direction)
        {
            var prefab = ProjectileHelper.GetProjectileType(this);
            var projectile = Instantiate(prefab, transform.position, Quaternion.identity);
            projectile.transform.localScale = Vector3.one * weaponInfo.ProjectileSize;

            var projectileComponent = projectile.AddComponent<Projectile>();
            projectileComponent.Info = new DamageInfo
            {
                DamageType = weaponInfo.DamageType,
                Damage = weaponInfo.ProjectileDamage,
                DamageSource = gameObject
            };

            if (gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                projectile.layer = LayerMask.NameToLayer("PlayerProjectile");
            }

            var rb2D = projectile.GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.linearVelocity = direction * weaponInfo.ProjectileSpeed;
            }

            var rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * weaponInfo.ProjectileSpeed;
            }
        }
        
        public void ReloadWeapon()
        {
            if (_isReloading || !weaponInfo.ConsumesAmmo || weaponInfo.LoadedAmmo >= weaponInfo.MaxAmmo || weaponInfo.CurrentAmmo <= 0) return;
            _isReloading = true;
            SoundManager.PlaySound(weaponInfo.ReloadSound, ignoresOtherPlaying: true);
            StartCoroutine(ReloadRoutine());
        }

        private System.Collections.IEnumerator ReloadRoutine()
        {
            yield return new WaitForSeconds(weaponInfo.ReloadCooldown);

            if (weaponInfo.ConsumesAmmo)
            {
                var diff = weaponInfo.MaxAmmo - weaponInfo.LoadedAmmo;
                var ammoToReload = Mathf.Min(diff, weaponInfo.CurrentAmmo);
                weaponInfo.CurrentAmmo -= ammoToReload;
                weaponInfo.LoadedAmmo += ammoToReload;
            }

            _isReloading = false;
        }
        
        public void AddReserveAmmo(float amount)
        {
            weaponInfo.CurrentAmmo = Mathf.Min(weaponInfo.CurrentAmmo + amount, weaponInfo.MaxAmmo);
        }
        
        public void AddLoadedAmmo(float amount)
        {
            weaponInfo.LoadedAmmo = Mathf.Min(weaponInfo.LoadedAmmo + amount, weaponInfo.MaxAmmo);
        }
        
        public void CoolDownWeapon(float time)
        {
            if(weaponInfo == null) return;
            
            weaponInfo.CurrentHeat -= time * weaponInfo.HeatReductionPerSecond * 5;
            if (!(weaponInfo.CurrentHeat < 0)) return;
            weaponInfo.CurrentHeat = 0;
            isOverheating = false;
        }
        
        public (float currentAmmo, float currentHeat) GetWeaponStatus()
        {
            return (weaponInfo.CurrentAmmo, weaponInfo.CurrentHeat);
        }
        
        public void LogWeaponStatus()
        {
            var ammoText = weaponInfo.ConsumesAmmo
                ? $"Ammo: {weaponInfo.CurrentAmmo}/{weaponInfo.MaxAmmo}"
                : "Ammo: ∞ (Energy Weapon)";

            var heatText = !weaponInfo.ConsumesAmmo
                ? $"Heat: {weaponInfo.CurrentHeat:0.0}/{weaponInfo.MaxHeat}" + (weaponInfo.CurrentHeat > 0 ? $" | Cooling Rate: {weaponInfo.HeatReductionPerSecond}/s" : "")
                : "Heat: N/A";

            string cooldownText;

            if (isOverheating)
            {
                cooldownText = "Overheated – Cooling down...";
            }
            else if (_fireTimer > 0)
            {
                cooldownText = $"Cooldown: {_fireTimer:0.00}s";
            }
            else
            {
                cooldownText = "Ready to fire";
            }

            var reloadingText = weaponInfo.ConsumesAmmo && _isReloading
                ? "Reloading..."
                : "";

            Debug.Log($"[Weapon Status] {ammoText} | {heatText} | {cooldownText}" + (string.IsNullOrEmpty(reloadingText) ? "" : $" | {reloadingText}"));
        }

    }
}
