using Assets.Scripts.Combat;
using Scripts.Core;
using UnityEngine.Events;

namespace Scripts.Entities.Players.OtherPlayers
{
    public class OtherPlayer : Player, IFindable
    {
        public UnityEvent<bool> OnFind;

        public int sightCount { get; set; } = 0;

        public override void Init(LocationInfoPacket packet, bool isOwner)
        {
            base.Init(packet, isOwner);
            var movement = GetCompo<OtherPlayerMovement>();
            transform.position = packet.position.ToVector3();
        }
        public void Escape()
        {
            OnFind?.Invoke(false);
        }

        public void Founded()
        {
            OnFind?.Invoke(true);
        }
    }
}
