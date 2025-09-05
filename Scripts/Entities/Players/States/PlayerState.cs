using Blade.FSM;
using Scripts.Entities.Players.MyPlayers;

namespace Scripts.Entities.Players.States
{
    public class PlayerState : EntityState
    {
        protected MyPlayer _player;
        protected MyPlayerMovement _movement;
        public PlayerState(NetworkEntity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity as MyPlayer;
            _movement = _player.GetCompo<MyPlayerMovement>();
        }
    }
}
