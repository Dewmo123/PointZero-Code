using DG.Tweening;
using Scripts.Core.Managers;
using Scripts.Entities.Players;
using Scripts.Network;
using UnityEngine;

namespace Scripts.GameSystem.CheckPlayer
{
    public class Door : Area
    {
        [SerializeField] private float openDistance = 5f;
        [SerializeField] Transform door;
        public DoorStatus Status { get; private set; }
        private Player _player;
        private C_DoorStatus statusPacket = new();
        private Vector3 _initPosition;
        public override void InitArea(Vector3 size, Vector3 position, Quaternion rotation, int index, params ushort[] types)
        {
            base.InitArea(size, position,rotation, index, types);
            _player = PlayerManager.Instance.MyPlayer;
            position.y -= 1;//offset
            _initPosition = position;
            SetStatus((DoorStatus)types[0]);
        }
        public void SetStatus(DoorStatus status)
        {
            this.Status = status;
            switch (this.Status)
            {
                case DoorStatus.Open:
                    door.DOMoveY(_initPosition.y - 10, 0.5f);
                    break;
                case DoorStatus.Close:
                    door.DOMoveY(_initPosition.y, 0.5f);
                    break;
            }
        }
        public DoorStatus GetNegate()
        {
            switch (Status)
            {
                case DoorStatus.Open:
                    return DoorStatus.Close;
                case DoorStatus.Close:
                    return DoorStatus.Open;
            }
            return 0;
        }
    }
}
