using Core.EventSystem;
using Scripts.Entities.Players;
using Scripts.Network;
using Scripts.UI;

namespace Scripts.Core.EventSystem
{
    public static class UIEvents
    {
        public readonly static ChangeUIEvent ChangeUIEvent = new();
        public readonly static CloseUIEvent CloseUIEvent = new();
        public readonly static SetGaugeEvent SetGaugeEvent= new();
        public readonly static SetGaugeActiveEvent SetGaugeActiveEvent = new();
        public static readonly HandlePlayerDead HandlePlayerDead = new();
    }
    public class ChangeUIEvent : GameEvent
    {
        public UIType type;
    }
    public class CloseUIEvent : GameEvent { }
    public class SetGaugeEvent : GameEvent
    {
        public string text;
        public float fillAmount;
    }
    public class SetGaugeActiveEvent : GameEvent
    {
        public bool active;
    }
    public class HandlePlayerDead : GameEvent
    {
        public Player attacker;
        public Player hitPlayer;
    }
}
