using Assets.Scripts.Entities.Players.OtherPlayers;
using Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Entities.Players.OtherPlayers
{
    public class OtherPlayerMovement : PlayerMovement
    {
        public static long InterpolationBackTime = 120;
        public static long ServerTimeOffset { get; private set; }
        private static bool _isServerTimeInitialized;

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

        public static void SyncServerTime(long serverTime)
        {
            long measuredOffset = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - serverTime;
            if (!_isServerTimeInitialized)
            {
                ServerTimeOffset = measuredOffset;
                _isServerTimeInitialized = true;
                return;
            }

            ServerTimeOffset += (measuredOffset - ServerTimeOffset) / 5;
        }

        public void AddSnapshot(SnapshotPacket pak)
        {
            Vector3 snapshotPos = pak.position.ToVector3();
            if (!_isServerTimeInitialized)
            {
                ServerTimeOffset = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pak.timestamp;
                _isServerTimeInitialized = true;
            }
            if (_snapshots.Count > 0)
            {
                var last = _snapshots[_snapshots.Count - 1];
                if (pak.timestamp <= last.timestamp)
                    return;
            }
            else
            {
                _prevInterpPos = snapshotPos;
                _serverPos = _prevInterpPos;
            }

            _snapshots.Add(pak);
            if (_snapshots.Count >= 120)
                _snapshots.RemoveRange(0, 60);
        }

        private void Update()
        {
            if (_player.isDead)
            {
                _animator.SetParam(_currentAnimHash, false);
                return;
            }
            if (_snapshots.Count == 0)
                return;

            long serverNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ServerTimeOffset;
            long interpTime = serverNow - InterpolationBackTime;

            while (_snapshots.Count >= 2 && _snapshots[1].timestamp <= interpTime)
                _snapshots.RemoveAt(0);

            SnapshotPacket older = _snapshots[0];
            SnapshotPacket newer = _snapshots.Count > 1 ? _snapshots[1] : _snapshots[0];
            bool found = _snapshots.Count > 1 &&
                         older.timestamp <= interpTime &&
                         newer.timestamp >= interpTime &&
                         newer.timestamp > older.timestamp;

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
                _serverPos = newer.position.ToVector3();
            }
            else if (_snapshots.Count > 1)
            {
                SnapshotPacket latest = _snapshots[_snapshots.Count - 1];
                SnapshotPacket previous = _snapshots[_snapshots.Count - 2];
                float packetDelta = Mathf.Max(0.001f, (latest.timestamp - previous.timestamp) / 1000f);
                float extrapolationSeconds = Mathf.Max(0f, (interpTime - latest.timestamp) / 1000f);

                Vector3 velocity = (latest.position.ToVector3() - previous.position.ToVector3()) / packetDelta;
                interpPos = latest.position.ToVector3() + velocity * extrapolationSeconds;
                interpRot = latest.rotation.ToQuaternion();
                interpGunRot = latest.gunRotation.ToQuaternion();
                animHash = latest.animHash;
                _serverPos = latest.position.ToVector3();
            }
            else
            {
                SnapshotPacket latest = _snapshots[_snapshots.Count - 1];
                Vector3 targetPos = latest.position.ToVector3();
                Quaternion targetRot = latest.rotation.ToQuaternion();
                Quaternion targetGunRot = latest.gunRotation.ToQuaternion();

                interpPos = Vector3.Lerp(_player.transform.position, targetPos, Time.deltaTime);
                interpRot = Quaternion.Slerp(_player.transform.rotation, targetRot, Time.deltaTime);
                interpGunRot = Quaternion.Slerp(_attackCompo.currentGun.transform.rotation, targetGunRot, Time.deltaTime);
                animHash = latest.animHash;
                _serverPos = targetPos;
            }

            SetAnimation(interpPos, animHash);
            _player.transform.position = interpPos;
            _player.transform.rotation = interpRot;
            _attackCompo.currentGun.transform.rotation = interpGunRot;
            _prevInterpPos = interpPos;
        }

        private void SetAnimation(Vector3 interpPos, int animHash)
        {
            Vector3 delta = interpPos - _prevInterpPos;
            if (_currentAnimHash != animHash)
            {
                _animator.SetParam(_currentAnimHash, false);
                _currentAnimHash = animHash;
                _animator.SetParam(_currentAnimHash, true);
            }
            else
            {
                _animator.SetParam(_xHash, 0f);
                _animator.SetParam(_zHash, 0f);
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
