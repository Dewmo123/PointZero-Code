using Server.Rooms;
using System.Numerics;

namespace Server.Objects.Areas
{
    internal class BombArea : Cube
    {
        public BombArea(Vector3 pos, Vector3 size,Room room) : base(pos, size,room)
        {
            ObjectType = Utiles.ObjectType.BombArea;
        }
    }
}
