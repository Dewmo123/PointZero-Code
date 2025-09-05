using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Entities.Players
{
    public class PlayerCanvas : MonoBehaviour, IEntityComponent
    {
        public TextMeshProUGUI nickname;
        [SerializeField] private Image healthBar;
        private Player _player;
        public void Initialize(NetworkEntity entity)
        {
            _player = entity as Player;
            _player.Health.OnValueChanged += HandleHealthChanged;
            _player.Name.OnValueChanged += HandleNameChanged;
        }

        private void HandleNameChanged(string previousValue, string nextValue)
        {
            nickname.text = nextValue;
        }

        private void OnDestroy()
        {
            _player.Name.OnValueChanged -= HandleNameChanged;
            _player.Health.OnValueChanged -= HandleHealthChanged;
        }
        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
        private void HandleHealthChanged(int previousValue, int nextValue)
        {
            healthBar.fillAmount = nextValue / 100f;//임시
        }
    }
}
