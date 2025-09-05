using Scripts.GameSystem.CheckPlayer;
using Scripts.Network;
using UnityEngine;

namespace Scripts.Entities.Players.MyPlayers.Interact
{
    public class PlayerInteract : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private float radius;
        [SerializeField] private LayerMask doorLayer;
        [SerializeField] private Transform checkTrm;
        private MyPlayer _player;
        public void Initialize(NetworkEntity entity)
        {
            _player = entity as MyPlayer;
            _player.PlayerInput.OnInteractEvent += HandleInteract;
        }
        private void OnDestroy()
        {
            _player.PlayerInput.OnInteractEvent -= HandleInteract;
        }
        private C_DoorStatus _statusPacket = new();
        Collider[] _colliders = new Collider[1];
        private void HandleInteract()
        {
            int count = Physics.OverlapSphereNonAlloc(checkTrm.position, radius, _colliders, doorLayer);
            if (count == 0)
                return;
            Door door = _colliders[0].GetComponent<Door>();
            _statusPacket.status = (ushort)door.GetNegate();
            Debug.Log(door.Status);
            _statusPacket.index = door.Index;
            NetworkManager.Instance.SendPacket(_statusPacket);
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(checkTrm.position, radius);
        }
#endif
    }
}
