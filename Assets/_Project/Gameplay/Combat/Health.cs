using System;
using UnityEngine;

namespace Addicted.Gameplay.Combat
{
    /// <summary>Vida de um inimigo, com empurrão e piscada ao levar golpe.</summary>
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField] private SpriteRenderer flashRenderer;
        [SerializeField, Min(0f)] private float knockbackDamping = 14f;

        private int _current;
        private float _flashTimer;
        private Color _baseColor = Color.white;

        public event Action<Health> Died;

        public bool IsDead { get; private set; }
        public int Current => _current;
        /// <summary>Velocidade de empurrão atual; quem move o corpo deve somá-la.</summary>
        public Vector2 Knockback { get; private set; }

        private void Awake()
        {
            _current = maxHealth;
            if (flashRenderer != null)
                _baseColor = flashRenderer.color;
        }

        public void TakeHit(int damage, Vector2 knockback)
        {
            if (IsDead)
                return;

            _current -= damage;
            Knockback = knockback;
            _flashTimer = 0.1f;

            if (_current > 0)
                return;

            IsDead = true;
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        private void Update()
        {
            Knockback = Vector2.MoveTowards(Knockback, Vector2.zero, knockbackDamping * Time.deltaTime);

            if (flashRenderer == null)
                return;

            _flashTimer -= Time.deltaTime;
            flashRenderer.color = _flashTimer > 0f ? new Color(1f, 0.35f, 0.35f) : _baseColor;
        }
    }
}
