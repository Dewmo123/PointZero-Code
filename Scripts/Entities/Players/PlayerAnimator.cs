using Assets.Scripts.Entities;
using System;
using UnityEngine;

namespace Scripts.Entities.Players
{
    public class PlayerAnimator : EntityAnimator
    {
        public Transform leftHandTarget;
        private int _deadHash;
        private Player _player;
        private float _weight=1;
        private EntityAnimatorTrigger _entityAnimatorTrigger;
        public override void Initialize(NetworkEntity entity)
        {
            base.Initialize(entity);
            _player = entity as Player;
            _deadHash = Animator.StringToHash("Dead");
            _player.OnReload.AddListener(HandleReload);
            _entityAnimatorTrigger = _player.GetCompo<EntityAnimatorTrigger>();
            _entityAnimatorTrigger.OnReloadEndTrigger += HandleReloadEnd;
        }
        private void OnDestroy()
        {
            _entityAnimatorTrigger.OnReloadEndTrigger -= HandleReloadEnd;
        }
        private void HandleReloadEnd()
        {
            SetIKWeight(1);
            _player.IsReload = false;
        }

        private void HandleReload()
        {
            SetParam(Animator.StringToHash("Reload"));
            SetIKWeight(0);
            _player.IsReload = true;
        }
        public void SetDead()
        {
            SetParam(_deadHash, true);
        }
        public void Revive()
        {
            SetParam(_deadHash, false);
        }
        public void SetIKWeight(float weight)
        {
            _weight = weight;
        }
        private void OnAnimatorIK(int layerIndex)
        {

            // 왼손 IK 설정
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _weight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _weight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }
        }
    }
}
