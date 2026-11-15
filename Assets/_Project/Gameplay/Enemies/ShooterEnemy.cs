using Addicted.Gameplay.Player;
using UnityEngine;

namespace Addicted.Gameplay.Enemies
{
    /// <summary>Atirador (cabeça em losango): mantém distância e atira no jogador.</summary>
    public sealed class ShooterEnemy : BeltEnemy
    {
        [SerializeField, Min(0f)] private float minDistance = 4f;
        [SerializeField, Min(0f)] private float maxDistance = 7f;
        [SerializeField, Min(0.1f)] private float fireInterval = 2.2f;
        [SerializeField, Min(0f)] private float projectileSpeed = 5f;
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private Material projectileMaterial;
        [SerializeField] private LayerMask playerLayers;
        [SerializeField, Min(0f)] private float muzzleHeight = 1f;

        private float _fireTimer;

        protected override void Awake()
        {
            base.Awake();
            _fireTimer = fireInterval * 0.6f;
        }

        protected override Vector2 DesiredVelocity(Vector2 toPlayer)
        {
            var distance = toPlayer.magnitude;
            if (distance < minDistance)
                return -toPlayer.normalized * moveSpeed;
            if (distance > maxDistance)
                return toPlayer.normalized * moveSpeed;

            // Dentro da faixa ideal: só alinha a profundidade com o jogador.
            return new Vector2(0f, Mathf.Clamp(toPlayer.y, -1f, 1f) * moveSpeed * 0.5f);
        }

        protected override void OnTick(PlayerCharacter player, Vector2 toPlayer, float deltaTime)
        {
            _fireTimer -= deltaTime;
            if (_fireTimer > 0f || toPlayer.sqrMagnitude < 0.01f)
                return;

            _fireTimer = fireInterval;
            Projectile.Spawn(transform.position, toPlayer.normalized * projectileSpeed, muzzleHeight,
                projectileSprite, projectileMaterial, playerLayers);
        }
    }
}
