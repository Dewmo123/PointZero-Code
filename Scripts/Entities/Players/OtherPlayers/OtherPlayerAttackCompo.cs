using DewmoLib.ObjectPool.RunTime;
using Scripts.Core.Sound;
using Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Entities.Players.OtherPlayers
{
    public class OtherPlayerAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] public Transform currentGun;//임시용 나중에 총 여러개 만든다 했을때 어케할지 고민해야할듯
        [SerializeField] private SoundSO sound;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO soundPlayer;
        public void Initialize(NetworkEntity entity)
        {
        }
        public void Shoot(Vector3 position, Vector3 direction)
        {
            var sp = poolManager.Pop(soundPlayer) as SoundPlayer;
            sp.PlaySound(sound, position);
            Instantiate(bulletPrefab, position, Quaternion.LookRotation(direction));
        }
    }
}
