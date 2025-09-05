using Scripts.Core;
using Scripts.Entities;
using UnityEngine;

namespace Scripts.GameSystem.CheckPlayer
{
    public abstract class Area : NetworkEntity
    {
        private Vector3 _size;
        [SerializeField] private BoxCollider myCollider;
        public virtual void InitArea(Vector3 size,Vector3 position,Quaternion rotation,int index,params ushort[] types)
        {
            _size = size;
            transform.position = position;
            transform.rotation = rotation;
            Index = index;
            myCollider.size = _size;
            base.InitEntity();
        }
        //플레이어 위치가 정확히 범위 안에 있는지 확인
        public bool CheckInRange(Vector3 target)
            => myCollider.bounds.Contains(target);
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _size);
        }
#endif
    }
}
