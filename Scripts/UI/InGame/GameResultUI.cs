using Core.EventSystem;
using Cysharp.Threading.Tasks;
using Scripts.Core.EventSystem;
using Scripts.Network;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.InGame
{
    [Serializable]
    public struct LogoColor
    {
        public Color inLine;
        public Color outLine;
    }
    public class GameResultUI : MonoBehaviour
    {
        [SerializeField] private Image result;
        [SerializeField] private Image panel;
        [SerializeField] private Image inLine;
        [SerializeField] private Image outLine;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private EventChannelSO packetChannel;
        [SerializeField] private LogoColor[] colors;

        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            packetChannel.AddListener<HandleGameEnd>(GameEndHandler);
            gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleGameEnd>(GameEndHandler);
        }

        private async void GameEndHandler(HandleGameEnd end)
        {
            gameObject.SetActive(true);
            if (end.winner == Team.None)
            {
                text.text = "Draw";
                outLine.color = colors[0].outLine;
                inLine.color = colors[0].inLine;
            }
            else
            {
                int idx = end.isWin ? 1 : 2;
                text.text = end.isWin ? "Victory" : "Defeat";
                inLine.color = colors[idx].inLine;
                outLine.color = colors[idx].outLine;
            }
            _animator.Play("GameResultOpen");
            await UniTask.WaitForSeconds(3);
            _animator.Play("GameResultClose");
            await UniTask.WaitUntil(() =>
            {
                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName("GameResultClose") && stateInfo.normalizedTime >= 1f;
            });
            gameObject.SetActive(false);
        }
    }
}
