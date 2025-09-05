using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.UI
{
    public class MainCanvas : MonoBehaviour
    {
        private Dictionary<UIType, ChangeableUI> _uiDictionary = new();
        [SerializeField] private EventChannelSO uiChannel;
        [SerializeField] private UIType initUI;
        private ChangeableUI _currentUI;
        private void Awake()
        {
            InitUIs();
            _currentUI = null;
            uiChannel.AddListener<ChangeUIEvent>(HandleChangeUI);
            uiChannel.AddListener<CloseUIEvent>(HandleCloseUI);
            var evt = UIEvents.ChangeUIEvent;
            evt.type = initUI;
            uiChannel.InvokeEvent(evt);
        }
        private void OnDestroy()
        {
            uiChannel.RemoveListener<ChangeUIEvent>(HandleChangeUI);
            uiChannel.RemoveListener<CloseUIEvent>(HandleCloseUI);
        }
        private void InitUIs()
        {
            IEnumerable<ChangeableUI> uis = GetComponentsInChildren<ChangeableUI>();
            foreach (var item in uis)
                _uiDictionary.Add(item.type, item);
            foreach (var item in uis)
                item.Init(uiChannel);
        }

        private void HandleChangeUI(ChangeUIEvent @event)
        {
            _currentUI?.Close();
            ChangeableUI changeable = _uiDictionary.GetValueOrDefault(@event.type);
            Debug.Assert(changeable != null, $"{@event.type} is not implemented UI");
            changeable.Open();
            _currentUI = changeable;
        }
        private void HandleCloseUI(CloseUIEvent @event)
        {
            _currentUI?.Close();
            _currentUI = null;
        }

    }
}
