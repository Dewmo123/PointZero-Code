using Scripts.Core.EventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.UI.Title
{
    public class MainTitle : ChangeableUI
    {
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
        public void Help()
        {
            var evt = UIEvents.ChangeUIEvent;
            evt.type = UIType.Help;
            uiChannel.InvokeEvent(evt);
        }
    }
}
