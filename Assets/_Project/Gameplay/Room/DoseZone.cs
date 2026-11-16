using Addicted.Gameplay.Player;
using UnityEngine;

namespace Addicted.Gameplay.Room
{
    /// <summary>Zona de dose: ao pisar, o jogador toma a dose. Reaparece depois de um tempo.</summary>
    public sealed class DoseZone : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayers;
        [SerializeField, Min(0.1f)] private float radius = 0.8f;
        [SerializeField, Min(0f)] private float respawnTime = 6f;
        [SerializeField] private SpriteRenderer visual;

        private float _respawnTimer;

        public bool IsAvailable => _respawnTimer <= 0f;

        private void Update()
        {
            if (!IsAvailable)
            {
                _respawnTimer -= Time.deltaTime;
                if (IsAvailable && visual != null)
                    visual.enabled = true;
                return;
            }

            var hit = Physics2D.OverlapCircle(transform.position, radius, playerLayers);
            if (hit == null)
                return;

            var player = hit.GetComponentInParent<PlayerCharacter>();
            if (player == null)
                return;

            player.TakeDose();
            _respawnTimer = respawnTime;
            if (visual != null)
                visual.enabled = false;
        }
    }
}
