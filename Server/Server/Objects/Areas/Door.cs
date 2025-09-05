using Server.Rooms;
using Server.Utiles;
using ServerCore;
using System.Numerics;

namespace Server.Objects.Areas
{
    internal class Door : Cube
    {
        public DoorStatus Status { get; set; }
        public Door(Vector3 pos, Vector3 size, Quaternion rotation, DoorStatus status,Room room) : base(pos, size,room)
        {
            this.rotation = rotation;
            Status = status;
            ObjectType = ObjectType.Door;
        }
        public override IDataPacket CreatePacket()
        {
            CubeInfo cube = (CubeInfo)base.CreatePacket();
            return new DoorInfo()
            {
                cube = cube,
                status = (ushort)Status
            }; ;
        }
        public DoorStatus GetNegate()
        {
            switch (Status)
            {
                case DoorStatus.Open:
                    return DoorStatus.Close;
                case DoorStatus.Close:
                    return DoorStatus.Open;
            }
            return 0;
        }
    }
}
