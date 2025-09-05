using Scripts.Core.EventSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.UI.Title
{
    public class HelpUI : ChangeableUI
    {
        [SerializeField] private Transform pagesTrm;
        private List<GameObject>_pages;
        private int _currentPage = 0;
        private void Awake()
        {
            _pages = new(5);
            foreach(Transform item in pagesTrm)
            {
                _pages.Add(item.gameObject);
                item.gameObject.SetActive(false);
            }
            _pages[_currentPage].SetActive(true);
        }
        public void Next()
        {
            if (_currentPage == _pages.Count - 1)
                return;
            _pages[++_currentPage].SetActive(true);
        }
        public void Prev()
        {
            if (_currentPage == 0)
                return;
            _pages[_currentPage--].SetActive(false);
        }
        public void Exit()
        {
            var evt = UIEvents.ChangeUIEvent;
            evt.type = UIType.MainTitle;
            uiChannel.InvokeEvent(evt);
        }
    }
}
