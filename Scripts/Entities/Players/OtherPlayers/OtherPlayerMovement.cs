using Assets.Scripts.Entities.Players.OtherPlayers;
using DG.Tweening;
using Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Entities.Players.OtherPlayers
{
    public class OtherPlayerMovement : PlayerMovement
    {
        public static long InterpolationBackTime = 5; // 200ms 뒤쳐지게 보간

        public List<SnapshotPacket> _snapshots = new List<SnapshotPacket>();
        private Vector3 _serverPos;
        private Vector3 _prevInterpPos;

        private int _currentAnimHash;
        private OtherPlayerAttackCompo _attackCompo;
        public override void Initialize(NetworkEntity entity)
        {
            base.Initialize(entity);
            _attackCompo = entity.GetCompo<OtherPlayerAttackCompo>();
        }
        public void AddSnapshot(SnapshotPacket pak)
        {
            if (_snapshots.Count > 0)
            {
                var last = _snapshots[_snapshots.Count - 1];
                if (pak.timestamp == last.timestamp) return;
            }

            _snapshots.Add(pak);
            if (_snapshots.Count >= 500)
                _snapshots.RemoveRange(0, 300);
            // 시간 기반 정리
            //long cutoffTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - InterpolationBackTime * 2;//100 10
            //_snapshots.RemoveAll(p => p.timestamp < cutoffTime);//p.timeStam < 100 80< Remove
        }
        private void FixedUpdate()
        {
            long interpTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - InterpolationBackTime;
            if (_snapshots.Count < 2 || _player.isDead)
            {
                _animator.SetParam(_currentAnimHash, false);
                return;
            }

            SnapshotPacket older = default;
            SnapshotPacket newer = default;
            bool found = false;

            for (int i = 0; i < _snapshots.Count - 1; i++)
            {
                if (_snapshots[i].timestamp <= interpTime && _snapshots[i + 1].timestamp >= interpTime)
                {
                    older = _snapshots[i];
                    newer = _snapshots[i + 1];
                    found = true;
                    break;
                }
            }
            Vector3 interpPos;
            Quaternion interpRot;
            Quaternion interpGunRot;
            int animHash;
            if (found)
            {
                float t = (interpTime - older.timestamp) / (float)(newer.timestamp - older.timestamp);
                interpPos = Vector3.Lerp(older.position.ToVector3(), newer.position.ToVector3(), t);
                interpRot = Quaternion.Slerp(older.rotation.ToQuaternion(), newer.rotation.ToQuaternion(), t);
                interpGunRot = Quaternion.Slerp(older.gunRotation.ToQuaternion(), newer.gunRotation.ToQuaternion(), t);
                animHash = newer.animHash;
                SetAnimation(interpPos, animHash);
            }
            else
            {
                // fallback: 이전 보간 위치 -> 최신 snapshot 위치로 부드럽게 보간
                SnapshotPacket latest = _snapshots[_snapshots.Count - 1];
                Vector3 targetPos = latest.position.ToVector3();
                Quaternion targetRot = latest.rotation.ToQuaternion();
                Quaternion targetGunRot = latest.gunRotation.ToQuaternion();

                float smoothSpeed = 10f; // 높일수록 빠르게 따라감
                interpPos = Vector3.Lerp(_prevInterpPos, targetPos, Time.fixedDeltaTime * smoothSpeed);
                interpRot = Quaternion.Slerp(_player.transform.rotation, targetRot, Time.fixedDeltaTime * smoothSpeed);
                interpGunRot = Quaternion.Slerp(_attackCompo.currentGun.transform.rotation, targetGunRot, Time.fixedDeltaTime * smoothSpeed);
                animHash = latest.animHash;
            }
            SetAnimation(interpPos, animHash);
            _player.transform.DOMove(interpPos, Time.fixedDeltaTime);
            _player.transform.rotation = interpRot;
            _attackCompo.currentGun.transform.rotation = interpGunRot;
            _prevInterpPos = interpPos;
        }

        private void SetAnimation(Vector3 interpPos, int animHash)
        {
            Vector3 direction = (interpPos - _prevInterpPos).normalized;
            if (_currentAnimHash != animHash)
            {
                _animator.SetParam(_currentAnimHash, false);
                _currentAnimHash = animHash;
                _animator.SetParam(_currentAnimHash, true);
            }
            if (direction.magnitude > 0f)
            {
                float forwardDot = Vector3.Dot(_player.transform.forward, direction); // 앞/뒤
                float rightDot = Vector3.Dot(_player.transform.right, direction);     // 좌/우
                _animator.SetParam(_xHash, rightDot);
                _animator.SetParam(_zHash, forwardDot);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_serverPos, 0.3f);
        }

        public override void SetPosition(Vector3 position)
        {
            _player.transform.position = position;
        }
    }
}
