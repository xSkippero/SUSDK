using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SUSDK.Manager
{
  public class SoundManager : MonoBehaviour
  {
    private static AudioSource _cantBePausedSource;
    private static AudioSource _normalSource;
    private static Dictionary<string, AudioClip> _soundDictionary;

    private void Awake()
    {
      _soundDictionary = new Dictionary<string, AudioClip>();
      FillSoundDictionary();
    }

    public static void PlaySound(string soundName, bool clean = false, bool cantBePaused = false, AudioSource source = null, bool ownSoundSettings = false, bool ignoresOtherPlaying = false)
    {
      K_playSound(soundName, source == null ? CreateAudioSource(cantBePaused) : source, clean, ownSoundSettings, ignoresOtherPlaying);
    }
    
    private static void FillSoundDictionary()
    {
      var audioClips = Resources.LoadAll(SkipperoUnitySdk.Instance.SoundsPath, typeof(AudioClip));

      foreach (var clip in audioClips.Cast<AudioClip>()) _soundDictionary.Add(clip.name, clip);
    }

    private static AudioSource CreateAudioSource(bool cantBePaused)
    {
      if (cantBePaused)
      {
        if (_cantBePausedSource) return _cantBePausedSource;
        var source = GameObject.Find("Player").AddComponent<AudioSource>();
        source.ignoreListenerPause = true;
        _cantBePausedSource = source;
        return source;

      }

      if (_normalSource) return _normalSource;
      {
        var source = GameObject.Find("Player").AddComponent<AudioSource>();
        _normalSource = source;
        return source;
      }

    }

    private static void K_playSound(string sound, AudioSource source, bool clean, bool ownSoundSettings, bool ignoresOtherPlaying)
    {
      if (!ignoresOtherPlaying && source.isPlaying) return;

      _soundDictionary.TryGetValue(sound, out var soundClip);

      if (!soundClip)
      {
        Debug.LogWarning("Sound \"" + sound + "\" was not found");
        return;
      }
      
      if (!ownSoundSettings)
      {
        source.pitch = !clean ? Random.Range(0.7f, 1f) : 1f;
      }
      
      source.PlayOneShot(soundClip);
    }
  }
}