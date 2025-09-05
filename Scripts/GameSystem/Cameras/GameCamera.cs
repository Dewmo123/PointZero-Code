using Core.EventSystem;
using Scripts.Core.EventSystem;
using UnityEngine;

namespace Scripts.GameSystem.Cameras
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private EventChannelSO packetChannel;
        [SerializeField]private int uiLayer;
        private void Awake()
        {
            packetChannel.AddListener<HandleEnterRoom>(HandleEnter);
            packetChannel.AddListener<HandleLeaveRoom>(HandleLeave);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleEnterRoom>(HandleEnter);
            packetChannel.RemoveListener<HandleLeaveRoom>(HandleLeave);
        }
        private void HandleLeave(HandleLeaveRoom room)
        {
            Camera.main.cullingMask = 1<<uiLayer;
        }

        private void HandleEnter(HandleEnterRoom room)
        {
            Camera.main.cullingMask = ~0;
        }
    }
}
