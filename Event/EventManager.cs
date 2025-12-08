using System.Collections.Generic;
using SUSDK.Domain;
using SUSDK.Entity;
using SUSDK.Event.Events.Enemy;
using SUSDK.Event.Events.Player;

namespace SUSDK.Event
{
  public abstract class EventManager
  {
    private static readonly List<IEventListener> Listeners = new List<IEventListener>();

    public static void RegisterListener(IEventListener listener)
    {
      if (!Listeners.Contains(listener)) Listeners.Add(listener);
    }

    public static void UnregisterListener(IEventListener listener)
    {
      if (Listeners.Contains(listener)) Listeners.Remove(listener);
    }
    
    public static void EntityHit(GameEntity enemy, float damage, DamageInfo damageInfo)
    {
      var enemyHitEvent = new OnEnemyHitEvent(enemy, damage, damageInfo);
      foreach (var listener in Listeners) listener.OnEntityHit(enemyHitEvent);
    }
    
    public static void EntityDeath(GameEntity enemy, DamageType type)
    {
      var enemyKillEvent = new OnEnemyDeathEvent(enemy, type);
      foreach (var listener in Listeners) listener.OnEntityDeath(enemyKillEvent);
    }
    
    public static void PlayerDeath(GameEntity player, DamageType cause)
    {
      var playerDeathEvent = new OnPlayerDeathEvent(player, cause);
      foreach (var listener in Listeners) listener.OnPlayerDeath(playerDeathEvent);
    }
    
    public static void PlayerHit(GameEntity player, float damage, DamageInfo bulletIdentity)
    {
      var playerDamageTakenEvent = new OnPlayerDamageTakenEvent(player, damage, bulletIdentity);
      foreach (var listener in Listeners) listener.OnPlayerHit(playerDamageTakenEvent);
    }
  }
}