using Assets.Scripts.Entities;
using Scripts.Core;
using Scripts.Core.GameSystem;
using Scripts.Entities.Players.MyPlayers;
using Scripts.Network;
using System;
using UnityEngine;

namespace Scripts.Entities.Players
{
    //Movement를 My,Other 두개로 나누려고 추상화 했는데 결국 other에선 위치동기화만 해버린
    public abstract class PlayerMovement : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected Transform model;
        protected Player _player;
        protected EntityAnimator _animator;
        public bool IsAiming { get; protected set; }

        protected int _xHash;
        protected int _zHash;


        public Vector3 ModelRot => model.transform.rotation.eulerAngles;

        public virtual void Initialize(NetworkEntity entity)
        {
            _xHash = Animator.StringToHash("X");
            _zHash = Animator.StringToHash("Z");
            _player = entity as Player;
            _animator = entity.GetCompo<PlayerAnimator>();
        }
        public void HandleAim(bool obj)
            => IsAiming = obj;
        public abstract void SetPosition(Vector3 position);
    }
}
