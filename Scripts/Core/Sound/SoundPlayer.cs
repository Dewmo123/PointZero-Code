using DewmoLib.ObjectPool.RunTime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Scripts.Core.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioMixerGroup _sfxGroup, _musicGroup;
        [SerializeField] private PoolItemSO _poolItem;
        private Pool _myPool;

        public GameObject GameObject => gameObject;

        public PoolItemSO PoolItem => _poolItem;

        private AudioSource _audioSource;
        private Transform _parent;
        private void Awake()
        {
            _parent = transform.parent;
            _audioSource = GetComponent<AudioSource>();
        }
        public void PlaySound(SoundSO data, Vector3 pos)
        {
            transform.position = pos;
            PlaySound(data);
        }
        public void PlaySound(SoundSO data,Transform trm)
        {
            transform.parent = trm;
            transform.position = trm.position;
            PlaySound(data);
        }

        private void PlaySound(SoundSO data)
        {
            if (data.audioType == SoundSO.AudioType.SFX)
            {
                _audioSource.outputAudioMixerGroup = _sfxGroup;
                _audioSource.maxDistance = data.maxDistance;
                _audioSource.minDistance = data.minDistance;
                _audioSource.spatialBlend = data.spatialBlend;
            }
            else if (data.audioType == SoundSO.AudioType.Music)
            {
                _audioSource.outputAudioMixerGroup = _musicGroup;
            }
            _audioSource.volume = data.volume;
            _audioSource.pitch = data.basePitch;
            if (data.randomizePitch)
            {
                _audioSource.pitch += Random.Range(-data.randomPitchModifier, data.randomPitchModifier);
            }
            _audioSource.clip = data.clip;
            _audioSource.loop = data.loop;
            if (!data.loop)
            {
                float time = _audioSource.clip.length + 0.2f;
                DOVirtual.DelayedCall(time, () =>
                {
                    transform.parent = _parent;
                    _myPool.Push(this);
                });
            }
            _audioSource.Play();
        }

        public void ResetItem()
        {

        }

        public void StopAndGoToPool()
        {
            _audioSource.Stop();
            _myPool.Push(this);
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }
    }

}