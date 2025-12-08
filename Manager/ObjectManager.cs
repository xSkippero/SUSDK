using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SUSDK.Manager
{
  public class ObjectManager : MonoBehaviour
  {
    public static Dictionary<string, GameObject> ObjectDictionary;

    private void Awake()
    {
      ObjectDictionary = new Dictionary<string, GameObject>();
      FillObjectDictionary();
    }

    public static GameObject GetObject(string objectName)
    {
      ObjectDictionary.TryGetValue(objectName, out var item);
      if (item) return item;
      Debug.LogWarning("Object \"" + objectName + "\" was not found");
      return new GameObject();

    }

    private static void FillObjectDictionary()
    {
      var objectPrefabs = Resources.LoadAll(SkipperoUnitySdk.Instance.ObjectsPath, typeof(GameObject));
      foreach (var @object in objectPrefabs.Cast<GameObject>()) ObjectDictionary.Add(@object.name, @object);
    }
  }
}