using Core.EventSystem;
using DG.Tweening;
using UnityEngine;

namespace Scripts.UI
{
    public enum UIType
    {
        None,
        RoomList,
        CreateRoom,
        SetNickname,
        InGame,
        MainTitle,
        Help
    }
    public abstract class ChangeableUI : MonoBehaviour
    {
        [field:SerializeField]public UIType type { get; protected set; }
        public RectTransform RectTrm => transform as RectTransform;
        [SerializeField] protected float parentHeight = 1080;
        [SerializeField] protected Vector2 initPosition;
        protected EventChannelSO uiChannel;

        public virtual void Init(EventChannelSO channel)
        {
            RectTrm.anchoredPosition = new Vector2(0, initPosition.y - parentHeight);
            uiChannel = channel;
        }
        public virtual void Open()
        {
            RectTrm.DOAnchorPos(initPosition, 0.5f).SetUpdate(true);
        }

        public virtual void Close()
        {
            Vector2 hidePosition = initPosition + new Vector2(0f, -parentHeight);
            RectTrm.DOAnchorPos(hidePosition, 0.5f).SetUpdate(true);
        }
        [ContextMenu("HidePanel")]
        private void Hide()
        {
            Vector2 hidePosition = initPosition + new Vector2(0, -parentHeight);
            RectTrm.anchoredPosition = hidePosition;
        }
        [ContextMenu("OpenPanel")]
        private void ShowPanel()
        {
            RectTrm.anchoredPosition = initPosition;
        }
    }
}
