using Server.Objects;
using Server.Utiles;
using System;
using System.Collections.Generic;

namespace Server.Rooms.States
{
    abstract class SyncObjectsState : GameRoomState
    {
        protected S_UpdateInfos _updates = new();
        public SyncObjectsState(GameRoom room) : base(room)
        {
            _updates.playerInfos = new List<PlayerInfoPacket>(15);
            _updates.snapshots = new List<SnapshotPacket>(15);
            _updates.attacks = new List<AttackInfoBr>(15);
        }
        public override void Enter()
        {
            base.Enter();
            ResetPacket();
            _room.OnAttack += HandleAttack;
        }
        protected virtual void HandleAttack(ClientSession session, C_ShootReq req)
        {
            _updates.attacks.Add(new AttackInfoBr()
            {
                attackerIndex = session.PlayerId,
                direction = req.direction,
                firePos = req.firePos,
                isDead = false,
                hitObjIndex = 0,
                objectType = 0
            });
            _room.Broadcast(_updates);
            ResetPacket();
        }
        public override void Exit()
        {
            base.Exit();
            _room.OnAttack -= HandleAttack;
        }

        public void ResetPacket()
        {
            _updates.playerInfos.Clear();
            _updates.snapshots.Clear();
            _updates.attacks.Clear();
        }

        public override void Update()
        {
            base.Update();
            foreach (var session in _room.Sessions)
            {
                Player player = _room.GetObject<Player>(session.Value.PlayerId);
                // Console.WriteLine(player.index);
                _updates.snapshots.Add(new SnapshotPacket()
                {
                    index = player.index,
                    position = player.position.ToPacket(),
                    rotation = player.rotation.ToPacket(),
                    animHash = player.animHash,
                    gunRotation = player.gunRotation.ToPacket(),
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
                _updates.playerInfos.Add(new PlayerInfoPacket()
                {
                    Health = player.Health,
                    index = player.index,
                    isAiming = player.isAiming
                });
                //Console.WriteLine(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            }
            _room.Broadcast(_updates);
            ResetPacket();
        }
    }
}
