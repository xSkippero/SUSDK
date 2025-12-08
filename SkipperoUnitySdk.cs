using SUSDK.Manager;
using UnityEngine;

namespace SUSDK
{
    public class SkipperoUnitySdk
    {
        public string EffectsPath { get; }
        public string ObjectsPath { get; }
        public string ProjectilesPath { get; }
        public string SoundsPath { get; }
        public string SpritesPath { get; }
        public bool ThreeDimensional { get; }

        public static SkipperoUnitySdk Instance { get; private set; }
        
        public SkipperoUnitySdk(bool threeDimensional, string effectsPath = "Effects", string objectsPath = "Objects", string projectilesPath = "Projectiles", string soundsPath = "Sounds", string spritesPath = "Sprites")
        {
            //Instance for the SDK
            Instance = this;
            
            //Configuration
            ThreeDimensional = threeDimensional;
            EffectsPath = effectsPath;
            ObjectsPath = objectsPath;
            ProjectilesPath = projectilesPath;
            SoundsPath = soundsPath;
            SpritesPath = spritesPath;
            
            //Create a GameObject to instantiate the managers
            var gameObject = new GameObject("SkipperoUnitySdk");
            gameObject.AddComponent<SoundManager>();
            gameObject.AddComponent<ProjectileManager>();
            gameObject.AddComponent<ObjectManager>();
            gameObject.AddComponent<EffectManager>();
            gameObject.AddComponent<SpriteManager>();
        }
    }
}