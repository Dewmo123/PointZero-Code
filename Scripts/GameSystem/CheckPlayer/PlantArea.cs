using UnityEngine;

namespace Scripts.GameSystem.CheckPlayer
{
    public class PlantArea : Area
    {
        [field: SerializeField] public Network.Area MyArea { get; private set; }
        public override void InitArea(Vector3 size, Vector3 position, Quaternion rotation, int index, params ushort[] types)
        {
            base.InitArea(size, position, rotation, index, types);
            MyArea = (Network.Area)types[0];
        }
    }
}
