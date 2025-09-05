using EPOOutline;
using UnityEngine;
using UnityEngine.Rendering;

namespace Scripts.Entities.Players.OtherPlayers
{
    public class OtherPlayerRenderer : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Outlinable outlinable;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private MeshRenderer[] renderers;
        private OtherPlayer _player;
        public void Initialize(NetworkEntity entity)
        {
            _player = entity as OtherPlayer;
            _player.OnFind.AddListener(HandleFind);
        }

        private void HandleFind(bool arg0)
        {
            ShadowCastingMode mode = arg0 ? ShadowCastingMode.On : ShadowCastingMode.Off;
            outlinable.enabled = arg0;
            skinnedMeshRenderer.shadowCastingMode = mode;
            foreach (var item in renderers)
                item.shadowCastingMode = mode;
        }
    }
}
