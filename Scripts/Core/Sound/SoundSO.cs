using UnityEngine;

namespace Scripts.Core.Sound
{
    [CreateAssetMenu(menuName = "SO/SoundClip")]
    public class SoundSO : ScriptableObject
    {
        public enum AudioType
        {
            SFX, Music
        }
        public AudioType audioType;
        public AudioClip clip;
        public bool loop = false;
        public bool randomizePitch = false;
        [Range(0f, 1f)]
        public float randomPitchModifier = 0.1f;
        [Range(0.1f, 2f)]
        public float volume = 1;
        [Range(0.1f, 3f)]
        public float basePitch = 1;
        [Range(0, 1)]
        public float spatialBlend = 0;

        public float minDistance, maxDistance;
    }

}