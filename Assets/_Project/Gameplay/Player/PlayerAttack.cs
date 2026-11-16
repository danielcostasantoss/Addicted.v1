using Addicted.Gameplay.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Addicted.Gameplay.Player
{
    /// <summary>Golpe corpo a corpo à frente do jogador. A área considera a profundidade (eixo Y).</summary>
    [RequireComponent(typeof(PlayerCharacter))]
    public sealed class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private InputActionReference attackAction;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField, Min(0.05f)] private float baseInterval = 0.32f;
        [SerializeField] private Vector2 hitboxSize = new Vector2(1.7f, 0.9f);
        [SerializeField, Min(0f)] private float knockback = 7f;
        [SerializeField] private SpriteRenderer slash;
        [SerializeField, Min(0f)] private float slashTime = 0.09f;

        private readonly Collider2D[] _hits = new Collider2D[16];
        private PlayerCharacter _player;
        private ContactFilter2D _filter;
        private float _cooldown;
        private float _slashTimer;

        private void Awake()
        {
            _player = GetComponent<PlayerCharacter>();
            _filter = new ContactFilter2D { useTriggers = true };
            _filter.SetLayerMask(enemyLayers);
            if (slash != null)
                slash.enabled = false;
        }

        private void OnEnable()
        {
            if (attackAction == null)
                return;

            attackAction.action.Enable();
            attackAction.action.performed += OnAttack;
        }

        private void OnDisable()
        {
            if (attackAction != null)
                attackAction.action.performed -= OnAttack;
        }

        private void OnAttack(InputAction.CallbackContext _) => TryAttack();

        /// <returns>Quantos inimigos foram atingidos, ou -1 se o golpe não saiu.</returns>
        public int TryAttack()
        {
            if (_cooldown > 0f || !_player.High.CanAttack)
                return -1;

            _cooldown = baseInterval * _player.High.AttackIntervalMultiplier;

            var facing = _player.Facing;
            var center = (Vector2)transform.position + new Vector2(facing * hitboxSize.x * 0.5f, 0f);
            var count = Physics2D.OverlapBox(center, hitboxSize, 0f, _filter, _hits);

            var landed = 0;
            for (var i = 0; i < count; i++)
            {
                var health = _hits[i].GetComponentInParent<Health>();
                if (health == null || health.IsDead)
                    continue;

                health.TakeHit(damage, new Vector2(facing * knockback, 0f));
                _player.RegisterHit();
                landed++;
            }

            if (slash != null)
            {
                slash.enabled = true;
                slash.flipX = facing < 0;
                slash.transform.localPosition = new Vector3(facing * 0.75f, slash.transform.localPosition.y, 0f);
                _slashTimer = slashTime;
            }

            return landed;
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;

            if (slash == null || !slash.enabled)
                return;

            _slashTimer -= Time.deltaTime;
            if (_slashTimer <= 0f)
                slash.enabled = false;
        }
    }
}
