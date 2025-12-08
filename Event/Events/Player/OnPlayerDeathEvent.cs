using System;
using SUSDK.Domain;
using SUSDK.Entity;

namespace SUSDK.Event.Events.Player
{
  public class OnPlayerDeathEvent : EventArgs
  {
    public OnPlayerDeathEvent(GameEntity player, DamageType deathCause)
    {
      Player = player;
      DeathCause = deathCause;
    }

    public GameEntity Player { get; private set; }
    public DamageType DeathCause { get; private set; }
  }
}