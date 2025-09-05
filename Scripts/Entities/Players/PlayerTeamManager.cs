using Core.EventSystem;
using EPOOutline;
using UnityEngine;

namespace Scripts.Entities.Players
{
    public class PlayerTeamManager : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Outlinable outline;
        [ColorUsage(true)]
        [SerializeField] private Color teamColor;
        [SerializeField] private Color enemyColor;
        [SerializeField] private PlayerFOV playerFOV;
        [SerializeField] private Transform modelParent;
        [SerializeField] private SkinnedMeshRenderer modelRenderer;
        private Player _player;
        [SerializeField]private MeshRenderer[] renderers;
        private PlayerCanvas canvas;
        public void Initialize(NetworkEntity entity)
        {
            _player = entity as Player;
            canvas = entity.GetCompo<PlayerCanvas>();
        }
        public void SetTeam(bool isTeam)
        {
            if (isTeam)
            {
                outline.BackParameters.Color = teamColor;
                playerFOV.SetEnable(true);
                _player.OnDead.AddListener(() => playerFOV.SetEnable(false));
                _player.OnRevive.AddListener(() => playerFOV.SetEnable(true));
                canvas.gameObject.SetActive(true);
                canvas.nickname.color = Color.blue;
                SetLayer(8);
            }
            else
            {
                canvas.nickname.color = Color.red;
                outline.BackParameters.Color = enemyColor;
                playerFOV.SetEnable(false);
                canvas.gameObject.SetActive(false);
                SetLayer(7);
            }

        }
        private void SetLayer(int layer)
        {
            modelRenderer.gameObject.layer = layer;
            if (!_player.IsOwner)
                _player.gameObject.layer = layer;
            foreach (var item in renderers)
            {
                if (item.gameObject.layer == 0)
                    item.gameObject.layer = layer;
            }
        }
    }
}
