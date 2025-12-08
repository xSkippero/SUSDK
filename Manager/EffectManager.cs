using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SUSDK.Manager
{
  public class EffectManager : MonoBehaviour
  {
    private static Dictionary<string, GameObject> _effectDictionary;

    private void Awake()
    {
      _effectDictionary = new Dictionary<string, GameObject>();
      FillEffectDictionary();
    }
    
    public static void PlayEffect(string effectName, Transform parent, Vector3 position, Quaternion rotation)
    {
      var effectPrefab = GetEffect(effectName);
      var effect = Instantiate(effectPrefab, position, rotation);
      ParticleSystem.EmitParams emitOverride = new()
      {
        startLifetime = 10f
      };

      effect.transform.parent = parent;
      effect.GetComponent<ParticleSystem>().Emit(emitOverride, 20);

      Destroy(effect, 12f);
    }

    private static GameObject GetEffect(string effectName)
    {
      _effectDictionary.TryGetValue(effectName, out var effect);
      if (effect != null) return effect;
      Debug.LogWarning("Effect \"" + effect + "\" was not found");
      return new GameObject();
    }

    private static void FillEffectDictionary()
    {
      var effectPrefabs = Resources.LoadAll(SkipperoUnitySdk.Instance.EffectsPath, typeof(GameObject));

      foreach (var effect in effectPrefabs.Cast<GameObject>()) _effectDictionary.Add(effect.name, effect);
    }
  }
}