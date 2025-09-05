using Server.Rooms;
using Server.Utiles;
using ServerCore;
using System.Numerics;

namespace Server.Objects.Areas
{
    internal class PlantArea : Cube
    {
        private Area _myArea;
        public PlantArea(Vector3 pos, Vector3 size,Area myArea,Room room) : base(pos, size,room)
        {
            _myArea = myArea;
            ObjectType = ObjectType.PlantArea;
        }
        public override IDataPacket CreatePacket()
        {
            CubeInfo cube = (CubeInfo)base.CreatePacket();
            PlantAreaInfo plantArea = new()
            {
                area = (ushort)_myArea,
                cube =cube
            };
            return plantArea;
        }
    }
}
