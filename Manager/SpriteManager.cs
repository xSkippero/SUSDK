using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SUSDK.Manager
{
  public class SpriteManager : MonoBehaviour
  {
    private static Dictionary<string, Sprite> _spriteDirectionary;

    private void Awake()
    {
      _spriteDirectionary = new Dictionary<string, Sprite>();
      FillSpriteDirectionary();
    }

    public static Sprite GetSprite(string spriteName)
    {
      _spriteDirectionary.TryGetValue(spriteName, out var item);
      if (item == null)
      {
        UnityEngine.Debug.LogWarning("Item \"" + spriteName + "\" was not found");
        return null;
      }

      return item;
    }

    public static string[] GetProjectileKeys()
    {
      return _spriteDirectionary.Keys.ToArray();
    }

    private static void FillSpriteDirectionary()
    {
      var sprites = Resources.LoadAll(SkipperoUnitySdk.Instance.SpritesPath, typeof(Sprite));

      foreach (var sprite in sprites.Cast<Sprite>()) _spriteDirectionary.Add(sprite.name, sprite);
    }
  }
}