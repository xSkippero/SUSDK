using System;
using SUSDK.Domain;
using SUSDK.Entity;

namespace SUSDK.Event.Events.Enemy
{
  public class OnEnemyDeathEvent : EventArgs
  {
    public OnEnemyDeathEvent(GameEntity enemy, DamageType type)
    {
      Enemy = enemy;
      Type = type;
    }

    public GameEntity Enemy { get; private set; }
    public DamageType Type { get; private set; }
  }
}