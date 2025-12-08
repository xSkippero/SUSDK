using System.Collections;
using UnityEngine;

namespace SUSDK.Util
{
  public class CoroutineStarter : MonoBehaviour
  {
    public static CoroutineStarter Instance { get; private set; }

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keeps the object across scenes
      }
      else
      {
        Destroy(gameObject); // Ensure only one instance exists
      }
    }

    public static void Start(IEnumerator coroutine)
    {
      if (Instance != null)
        Instance.StartCoroutine(coroutine);
      else
        UnityEngine.Debug.LogError("CoroutineStarter instance not found!");
    }
  }
}