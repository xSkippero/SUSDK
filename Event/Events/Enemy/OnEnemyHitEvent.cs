using System;
using SUSDK.Entity;

namespace SUSDK.Event.Events.Enemy
{
  public class OnEnemyHitEvent : EventArgs
  {
    public OnEnemyHitEvent(GameEntity enemy, float damage, DamageInfo damageInfo)
    {
      Enemy = enemy;
      DamageInfo = damageInfo;
      Damage = damage;
    }

    public GameEntity Enemy { get; private set; }
    public DamageInfo DamageInfo { get; private set; }
    public float Damage { get; private set; }
  }
}