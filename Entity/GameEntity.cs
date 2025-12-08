using SUSDK.Domain;
using SUSDK.Event;
using SUSDK.Helper;
using UnityEngine;

namespace SUSDK.Entity
{
  public class GameEntity : MonoBehaviour
  {
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowActive = Shader.PropertyToID("_GlowActive");
    
    [Header("Health & Shield")]
    [Tooltip("The player's or entity's health.")]
    public float hitPoints;

    [Tooltip("The player's or entity's shield health.")]
    public float shieldHitPoints;

    [Header("Damage & Defense")]
    [Tooltip("Can the entity take damage?")]
    public bool canTakeDamage;

    [Tooltip("Does the entity have heavy armor that reduces damage")]
    public bool hasHeavyArmor;

    [Tooltip("Does the entity have an energy shield that absorbs additional damage")]
    public bool hasEnergyShield;

    [Header("Status Effects")]
    [Tooltip("Is the entity frozen")]
    public bool isFrozen;
    
    private bool _isDestroyed;
    private Material _instanceMaterial;
    private float _startShieldPoints;
    private float _startHitPoints;
    
    private float _glacioDamageTracker;
    private float _freezeThreshold;
    private float _freezeTimer;
    private const float FreezeDuration = 30f;

    public void Revive() {
      _isDestroyed = false;
      hitPoints = _startHitPoints;
      shieldHitPoints = _startShieldPoints;
      isFrozen = false;
      _glacioDamageTracker = 0f;
    }
    
    private void Start()
    {
      if (TryGetComponent<Renderer>(out var component))
      {
        _instanceMaterial = new Material(component.sharedMaterial);
        component.material = _instanceMaterial;
      }

      _startShieldPoints = shieldHitPoints;
      _startHitPoints = hitPoints;
      _freezeThreshold = _startShieldPoints * 0.75f;
      SetGlowForShieldType();
    }

    private void Update()
    {
      if (!isFrozen) return;
      _freezeTimer -= Time.deltaTime;
      if (!(_freezeTimer <= 0f)) return;
      isFrozen = false;
      _glacioDamageTracker = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (!collision.collider.gameObject.CompareTag("Projectile")) return;
      Destroy(collision.collider.gameObject);
      var projectile = collision.collider.GetComponent<Projectile>();
      HandleHit(projectile.Info);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
      if (!collision.collider.gameObject.CompareTag("Projectile")) return;
      var projectile = collision.collider.GetComponent<Projectile>();
      if (projectile == null) return;
      var info = projectile.Info;
      Destroy(collision.collider.gameObject);
      HandleHit(info);
    }

    public float GetMaxHitPoints()
    {
      return _startHitPoints;
    }
    
    public void HandleHit(DamageInfo damage)
    {
      if (this == null || gameObject == null || Equals(null)) return;
      if (damage == null || damage.DamageSource == null) return;
      if (damage.DamageSource.name.Equals(gameObject.name)) return;
      if (_isDestroyed || !canTakeDamage) return;

      var projectileBaseDamage = damage.Damage;
      var projectileType = damage.DamageType;

      var calculatedProjectileDamage = ProjectileHelper.CalculateProjectileDamage(
        damageType: projectileType,
        projectileDamage: projectileBaseDamage,
        againstShieldAmount: shieldHitPoints,
        againstHeavyArmor: hasHeavyArmor,
        againstEnergyShield: hasEnergyShield,
        isTargetFrozen: isFrozen
      );

      if (SkipperoUnitySdk.Instance.ThreeDimensional)
      {
        UpdateShieldGlow(CalculateShieldLevel()); 
      }

      if (projectileType == DamageType.Glacio)
      {
        TryApplyGlacioFreeze(calculatedProjectileDamage);
      }

      if (shieldHitPoints > 0)
      {
        shieldHitPoints -= calculatedProjectileDamage;
        if (shieldHitPoints < 1) shieldHitPoints = 0;
      }

      switch (shieldHitPoints)
      {
        case 0 when hitPoints - calculatedProjectileDamage < 1F:
        {
          hitPoints = 0;
          _isDestroyed = true;
          
          if (!gameObject.CompareTag("Player"))
          {
            EventManager.EntityDeath(this, projectileType);
            Destroy(gameObject);
          }
          else
          {
            EventManager.PlayerDeath(this, projectileType);
          }

          break;
        }
        case 0:
          hitPoints -= calculatedProjectileDamage;
          if (hitPoints <= 0)
          {
            hitPoints = 0;
            _isDestroyed = true;
          
            if (!gameObject.CompareTag("Player"))
            {
              EventManager.EntityDeath(this, projectileType);
              Destroy(gameObject);
            }
            else
            {
              EventManager.PlayerDeath(this, projectileType);
            }
          }
          break;
      }
      
      if (!gameObject.CompareTag("Player"))
      {
        EventManager.EntityHit(this, calculatedProjectileDamage, damage);
      }
      else
      {
        EventManager.PlayerHit(this, calculatedProjectileDamage, damage);
      }
    }

    private void TryApplyGlacioFreeze(float damage)
    {
      _glacioDamageTracker += damage;

      if (!(_glacioDamageTracker >= _freezeThreshold)) return;
      isFrozen = true;
      _glacioDamageTracker = 0f;
      _freezeTimer = FreezeDuration;
    }

    private void SetGlowForShieldType()
    {
      if(!SkipperoUnitySdk.Instance.ThreeDimensional) return;
      
      if (hasEnergyShield && hasHeavyArmor)
      {
        SetShieldGlow(new Color(0.88F, 0F, 1F), true);
        return;
      }

      if (hasHeavyArmor)
      {
        SetShieldGlow(new Color(1F, 0.3F, 0F), true);
        return;
      }

      if (hasEnergyShield)
      {
        SetShieldGlow(new Color(0F, 0.22F, 1F), true);
      }
    }

    private void SetShieldGlow(Color shieldColor, bool shieldActive, float shieldPower = 1)
    {
      _instanceMaterial.SetColor(GlowColor, shieldColor);
      _instanceMaterial.SetFloat(GlowActive, shieldPower);
      if (!shieldActive) _instanceMaterial.SetFloat(GlowActive, 0.0F);
    }

    private void UpdateShieldGlow(float shieldPower, bool shieldActive = true)
    {
      _instanceMaterial.SetFloat(GlowActive, shieldPower);
      if (!shieldActive) _instanceMaterial.SetFloat(GlowActive, 0.0F);
    }

    private float CalculateShieldLevel()
    {
      var shieldStrength = shieldHitPoints / _startShieldPoints;
      return Mathf.Clamp01(shieldStrength);
    }
  }
}
