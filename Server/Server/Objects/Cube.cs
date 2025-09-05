using Server.Rooms;
using Server.Utiles;
using ServerCore;
using System.Drawing;
using System.Numerics;

namespace Server.Objects
{
    internal class Cube : ObjectBase
    {
        private Vector3 _size; 
        public Cube(Vector3 pos, Vector3 size,Room room) : base(room)
        {
            position = pos;
            _size = size;
        }
        
        public bool CheckInCube(Vector3 position)
        {
            Vector3 halfSize = _size * 0.5f;
            Vector3 min = this.position - halfSize;
            Vector3 max = this.position + halfSize;
            return position.X >= min.X && position.X <= max.X &&
            position.Y >= min.Y && position.Y <= max.Y &&
                   position.Z >= min.Z && position.Z <= max.Z;
        }

        public override IDataPacket CreatePacket()
        {
            CubeInfo info = new()
            {
                position = position.ToPacket(),
                rotation = rotation.ToPacket(),
                size = _size.ToPacket(),
                objectType = (ushort)ObjectType,
                index = index
            };
            return info;
        }
    }
}
