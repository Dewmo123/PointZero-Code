using Scripts.GameSystem.CheckPlayer;
using Scripts.Network;
using UnityEngine;

namespace Scripts.Entities.Players.MyPlayers.Interact
{
    public class PlayerDefuse : PlayerPlantBase
    {
        public override void HandlePlant()
        {
            if (!_movement.IsAiming && CheckInRange() is BombArea area)
            {
                _player.ChangeState("Defuse");
            }
        }
    }
}
