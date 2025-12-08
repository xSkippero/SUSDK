using SUSDK.Event.Events.Enemy;
using SUSDK.Event.Events.Player;

namespace SUSDK.Event
{
  public interface IEventListener
  {
    void OnEntityHit(OnEnemyHitEvent e);
    void OnEntityDeath(OnEnemyDeathEvent e);
    void OnPlayerDeath(OnPlayerDeathEvent e);
    void OnPlayerHit(OnPlayerDamageTakenEvent e);
  }
}