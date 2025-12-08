using System;
using SUSDK.Domain;
using SUSDK.Entity;
using UnityEngine.SceneManagement;

namespace SUSDK.Event.Events.Player
{
  public class OnPlayerDamageTakenEvent : EventArgs
  {
    public OnPlayerDamageTakenEvent(GameEntity player, float damage, DamageInfo damageInfo)
    {
      Time = DateTime.Now;
      SceneName = SceneManager.GetActiveScene().name;
      Player = player;
      Damage = damage;
      DamageInfo = damageInfo;
    }

    public DateTime Time { get; private set; }
    public string SceneName { get; private set; }
    public float Damage { get; private set; }
    public DamageInfo DamageInfo { get; private set; }
    public GameEntity Player { get; private set; }
  }
}