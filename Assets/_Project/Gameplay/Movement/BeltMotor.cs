using Addicted.Content;
using UnityEngine;

namespace Addicted.Gameplay.Movement
{
    /// <summary>
    /// Lógica do movimento "briga de rua" (dois eixos + dash), sem física nem MonoBehaviour.
    /// </summary>
    public sealed class BeltMotor
    {
        private BeltMovementSettings _settings;
        private Vector2 _velocity;
        private Vector2 _dashDirection;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private bool _dashRequested;

        public BeltMotor(BeltMovementSettings settings) => _settings = settings;

        public Vector2 Velocity => _velocity;
        public bool IsDashing => _dashTimer > 0f;
        /// <summary>1 para a direita, -1 para a esquerda.</summary>
        public int Facing { get; private set; } = 1;
        /// <summary>Multiplicador externo de velocidade (Pico, Crash...).</summary>
        public float SpeedMultiplier { get; set; } = 1f;

        public BeltMovementSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }

        public void RequestDash() => _dashRequested = true;

        public Vector2 Tick(Vector2 input, float deltaTime)
        {
            input = Vector2.ClampMagnitude(input, 1f);
            var hasInput = input.sqrMagnitude > 0.0001f;
            _dashCooldownTimer -= deltaTime;

            if (!IsDashing && Mathf.Abs(input.x) > 0.01f)
                Facing = input.x > 0f ? 1 : -1;

            if (_dashRequested)
            {
                _dashRequested = false;
                if (!IsDashing && _dashCooldownTimer <= 0f)
                {
                    _dashDirection = hasInput ? input.normalized : new Vector2(Facing, 0f);
                    if (Mathf.Abs(_dashDirection.x) > 0.01f)
                        Facing = _dashDirection.x > 0f ? 1 : -1;
                    _dashTimer = _settings.dashDuration;
                    _dashCooldownTimer = _settings.dashCooldown;
                }
            }

            if (IsDashing)
            {
                _dashTimer -= deltaTime;
                if (_dashTimer > 0f)
                {
                    _velocity = ApplyDepth(_dashDirection) * _settings.DashSpeed;
                    return _velocity;
                }

                _velocity = Vector2.ClampMagnitude(_velocity, _settings.moveSpeed * SpeedMultiplier);
            }

            var target = ApplyDepth(input) * (_settings.moveSpeed * SpeedMultiplier);
            var rate = hasInput ? _settings.acceleration : _settings.deceleration;
            _velocity = Vector2.MoveTowards(_velocity, target, rate * deltaTime);
            return _velocity;
        }

        private Vector2 ApplyDepth(Vector2 direction) =>
            new Vector2(direction.x, direction.y * _settings.depthSpeedRatio);
    }
}
