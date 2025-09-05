using Scripts.GameSystem.CheckPlayer;

namespace Scripts.Entities.Players.MyPlayers.Interact
{
    public class PlayerPlant : PlayerPlantBase
    {
        public Network.Area CurrentArea { get; private set; }
        public override void HandlePlant()
        {
            if (!_movement.IsAiming && CheckInRange() is PlantArea area)
            {
                CurrentArea = area.MyArea;
                _player.ChangeState("Plant");
            }
        }

    }
}
