using Server.Rooms;
using Server.Utiles;
using ServerCore;
using System;
using System.Numerics;

namespace Server.Objects.Areas
{
    internal class BreakableWall : Cube, IHittable
    {
        public int Health { get; set; }

        public bool IsDead => Health <= 0;

        public BreakableWall(Vector3 pos, Vector3 size, Quaternion rotation, DoorStatus status, Room room) : base(pos, size,room)
        {
            ObjectType = Utiles.ObjectType.BreakableWall;
            this.rotation = rotation;
            Revive();
        }
        public override IDataPacket CreatePacket()
        {
            CubeInfo cube = (CubeInfo)base.CreatePacket();
            return new BreakableWallInfo()
            {
                Health = Health,
                cube = cube
            };
        }

        public void Revive()
        {
            Health = 200;
        }

        public void Hit()
        {
            Health = Math.Clamp(Health - 10, 0, 200);
            Console.WriteLine(Health);
        }
    }
}
