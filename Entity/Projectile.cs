using System;
using SUSDK.Domain;
using UnityEngine;

namespace SUSDK.Entity
{
  public class DamageInfo
  {
    public GameObject DamageSource { get; set; }
    public DamageType DamageType { get; set; }
    public float Damage { get; set; }
  }
  
  public class Projectile : MonoBehaviour
  {
    public DamageInfo Info { get; set; }
    private void Start()
    {
      gameObject.name = "#Projectile_" + Guid.NewGuid();
      Destroy(gameObject, 30f);
    }
  }
}