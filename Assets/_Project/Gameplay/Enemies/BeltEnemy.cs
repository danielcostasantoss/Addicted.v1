using Addicted.Gameplay.Combat;
using Addicted.Gameplay.Movement;
using Addicted.Gameplay.Player;
using UnityEngine;

namespace Addicted.Gameplay.Enemies
{
    /// <summary>Base dos inimigos da sala: corpo cinemático que anda na faixa do chão e sofre empurrão.</summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public abstract class BeltEnemy : MonoBehaviour
    {
        [SerializeField, Min(0f)] protected float moveSpeed = 2.4f;
        [SerializeField] private LayerMask worldLayers;
        [SerializeField] private SpriteRenderer body;

        private Rigidbody2D _rigidbody;
        private Health _health;
        private Rigidbody2D.SlideMovement _slide;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _health = GetComponent<Health>();
            BeltPhysics.SetupKinematic(_rigidbody);
            _slide = BeltPhysics.CreateSlide(worldLayers);
        }

        private void FixedUpdate()
        {
            var player = PlayerCharacter.Current;
            var velocity = _health.Knockback;

            if (player != null)
            {
                var toPlayer = (Vector2)(player.transform.position - transform.position);
                velocity += DesiredVelocity(toPlayer);
                if (body != null && Mathf.Abs(toPlayer.x) > 0.05f)
                    body.flipX = toPlayer.x < 0f;
                OnTick(player, toPlayer, Time.fixedDeltaTime);
            }

            _rigidbody.Slide(velocity, Time.fixedDeltaTime, _slide);
        }

        /// <summary>Velocidade desejada, dado o vetor até o jogador.</summary>
        protected abstract Vector2 DesiredVelocity(Vector2 toPlayer);

        protected virtual void OnTick(PlayerCharacter player, Vector2 toPlayer, float deltaTime) { }
    }
}
