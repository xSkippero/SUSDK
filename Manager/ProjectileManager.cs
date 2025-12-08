using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SUSDK.Manager
{
  public class ProjectileManager : MonoBehaviour
  {
    private static Dictionary<string, GameObject> _projectileDictionary;

    private void Awake()
    {
      _projectileDictionary = new Dictionary<string, GameObject>();
      FillProjectileDictionary();
    }

    public static GameObject GetProjectile(string itemName)
    {
      _projectileDictionary.TryGetValue(itemName, out var item);
      if (item != null) return item;
      Debug.LogWarning("Item \"" + itemName + "\" was not found");
      return new GameObject();
    }

    private static void FillProjectileDictionary()
    {
      var projectilePrefabs = Resources.LoadAll(SkipperoUnitySdk.Instance.ProjectilesPath, typeof(GameObject));

      foreach (var projectile in projectilePrefabs.Cast<GameObject>())
        _projectileDictionary.Add(projectile.name, projectile);
    }
  }
}