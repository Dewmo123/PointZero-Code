using Server.Rooms;
using Server.Utiles;
using ServerCore;
using System;
using System.Numerics;

namespace Server.Objects
{
    internal class Player : ObjectBase, IHittable
    {
        public bool isAiming;
        public Team team;
        public Quaternion gunRotation;
        public int animHash;
        public int speed;
        public string nickName;
        public long LastLocationTimestamp { get; private set; }

        public Player(Room room) : base(room)
        {
            TouchLocationTimestamp();
        }

        public int Health { get; set; }
        public bool IsDead => Health <= 0;

        public void Revive()
        {
            Health = 100;
        }

        public void HandlePacket(C_UpdateLocation packet)
            => SetLocation(
                packet.location.position.ToVector3(),
                packet.location.rotation.ToQuaternion(),
                packet.location.gunRotation.ToQuaternion(),
                packet.location.animHash,
                packet.isAiming);

        public void SetLocation(Vector3 newPosition, Quaternion newRotation, Quaternion newGunRotation, int newAnimHash, bool aiming)
        {
            isAiming = aiming;
            position = newPosition;
            rotation = newRotation;
            gunRotation = newGunRotation;
            animHash = newAnimHash;
            TouchLocationTimestamp();
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

        private void TouchLocationTimestamp()
            => LastLocationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
