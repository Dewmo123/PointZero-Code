using DG.Tweening;
using Scripts.Entities.Players;
using Scripts.Network;
using TMPro;
using UnityEngine;

namespace Scripts.UI.InGame
{
    public class KillLogAttribute : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI attackerTxt;
        [SerializeField] private TextMeshProUGUI hitPlayerTxt;
        public async void SetTexts(Player attacker, Player hitPlayer)
        {
            attackerTxt.text = attacker.Name.Value;
            attackerTxt.color = GetTeamColor(attacker.MyTeam);
            hitPlayerTxt.text = hitPlayer.Name.Value;
            hitPlayerTxt.color = GetTeamColor(hitPlayer.MyTeam);
            await Awaitable.WaitForSecondsAsync(2);
            transform.DOScaleY(0, 0.3f).OnComplete(() => Destroy(gameObject));
        }
        private Color GetTeamColor(Team team) => team switch
        {
            Team.None => throw new System.NotImplementedException(),
            Team.Blue => Color.blue,
            Team.Red => Color.red,
            _=> throw new System.NotImplementedException()
        };
}
}
