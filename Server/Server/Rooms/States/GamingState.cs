using Server.Objects;
using Server.Objects.Areas;
using Server.Utiles;
using System;

namespace Server.Rooms.States
{
    internal abstract class GamingState : SyncObjectsState
    {
        public GamingState(GameRoom room) : base(room)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _room.OnDoorStatusChange += HandleDoorStatusChange;
        }

        protected override void HandleAttack(ClientSession session, C_ShootReq req)
        {
            ObjectBase hitObj = _room.GetObject<ObjectBase>(req.hitObjIndex);
            Player attacker = _room.GetObject<Player>(session.PlayerId);
            if (attacker.IsDead)
                return;
            if (hitObj == null)
            {
                _updates.attacks.Add(new AttackInfoBr()
                {
                    attackerIndex = session.PlayerId,
                    direction = req.direction,
                    firePos = req.firePos,
                    isDead = false,
                    hitObjIndex = req.hitObjIndex,
                    objectType = 0
                });
            }
            if (hitObj is IHittable hittable)
            {
                if (hittable.IsDead)
                    return;
                if (_room.CurrentState != RoomState.Between)
                    hittable.Hit();
                _updates.attacks.Add(new AttackInfoBr()
                {
                    attackerIndex = session.PlayerId,
                    direction = req.direction,
                    firePos = req.firePos,
                    isDead = hittable.IsDead,
                    hitObjIndex = req.hitObjIndex,
                    objectType = (ushort)hitObj.ObjectType
                });

            }
            _room.Broadcast(_updates);
            ResetPacket();
        }        
        public override void Exit()
        {
            base.Exit();
            _room.OnDoorStatusChange -= HandleDoorStatusChange;
        }
        private void HandleDoorStatusChange(DoorStatus targetStatus, Door door, Player player)
        {
            S_DoorStatus doorStatus = new();
            doorStatus.index = door.index;
            doorStatus.status = (ushort)door.GetNegate();//판별도 넣기
            door.Status = targetStatus;
            Console.WriteLine($"door: {doorStatus.status}, target:{targetStatus}");
            _room.Broadcast(doorStatus);
        }
    }
}