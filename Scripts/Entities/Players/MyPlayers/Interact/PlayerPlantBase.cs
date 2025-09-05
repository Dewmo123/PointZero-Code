using Scripts.Entities.Players.MyPlayers;
using Scripts.GameSystem.CheckPlayer;
using System;
using UnityEngine;

namespace Scripts.Entities.Players.MyPlayers.Interact
{
    public enum Role
    {
        None,
        Attack,
        Defend
    }
    public abstract class PlayerPlantBase : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask boundLayer;
        [field: SerializeField] public Role Role { get; private set; }

        protected MyPlayer _player;
        protected MyPlayerMovement _movement;
        private Collider[] colliders = new Collider[1];
        public virtual void Initialize(NetworkEntity entity)
        {
            _player = entity as MyPlayer;
            _movement = _player.GetCompo<MyPlayerMovement>();
        }
        public abstract void HandlePlant();

        protected Area CheckInRange()
        {
            int count = Physics.OverlapBoxNonAlloc(_player.transform.position, Vector3.one, colliders, Quaternion.identity, boundLayer);
            //1개 초과인 경우는 없음
            if (count > 0 && colliders[0].TryGetComponent(out GameSystem.CheckPlayer.Area area) && area.CheckInRange(_player.transform.position))
                return area;
            return null;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 2);
        }
#endif
    }
}
