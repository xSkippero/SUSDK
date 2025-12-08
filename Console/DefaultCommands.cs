using System.ComponentModel;

namespace SUSDK.Console
{
  public enum DefaultCommands
  {
    [Description("help")]
    Help,
    [Description("reload_scene")]
    ReloadScene,
    [Description("god")]
    God,
    [Description("set_health")]
    SetHealth,
    [Description("spawn")]
    Spawn,
    [Description("clear")]
    Clear,
    [Description("debug")]
    Debug
  }
}
