using JetBrains.Annotations;

namespace SUSDK.Domain
{
  [System.Serializable]
  public class GameSave<T>
  {
    [CanBeNull] public T Data { get; set; }
  }
}
