using Server.Rooms;
using Server.Utiles;
using ServerCore;
using System;
using System.Numerics;

namespace Server.Objects
{
    internal class Player : ObjectBase,IHittable
    {
        public bool isAiming;
        public Team team;
        public Quaternion gunRotation;
        public int animHash;
        public int speed;
        public string nickName;

        public Player(Room room) : base(room)
        {
        }

        public int Health { get; set; }
        public bool IsDead => Health <= 0;

        public void Revive()
        {
            Health = 100;
        }
        public void HandlePacket(C_UpdateLocation packet)
        {
            isAiming = packet.isAiming;
            position = packet.location.position.ToVector3();
            rotation = packet.location.rotation.ToQuaternion();
            gunRotation = packet.location.gunRotation.ToQuaternion();
            animHash = packet.location.animHash;
        }

        public override IDataPacket CreatePacket()
        {
            PlayerInfoPacket info = new() { Health = Health, index = index, isAiming = isAiming };
            return info;
        }

        public void Hit()
        {
            if (IsDead)
                return;
            Health = Math.Clamp(Health - 10, 0, 100);
            if (IsDead)
                _myRoom.ObjectDead(this);
                
        }
    }
}
