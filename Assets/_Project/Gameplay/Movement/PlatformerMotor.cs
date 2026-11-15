using Addicted.Content;
using UnityEngine;

namespace Addicted.Gameplay.Movement
{
    /// <summary>
    /// Lógica de movimento de plataforma, sem dependência de MonoBehaviour nem de física.
    /// Recebe a entrada e o estado de colisão a cada passo fixo e devolve a velocidade desejada.
    /// Isso permite testar corrida, pulo e dash em EditMode.
    /// </summary>
    public sealed class PlatformerMotor
    {
        private MovementSettings _settings;
        private Vector2 _velocity;

        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private int _airJumpsLeft;

        private bool _dashRequested;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private int _airDashesLeft;
        private int _dashDirection;

        public PlatformerMotor(MovementSettings settings)
        {
            _settings = settings;
            _airJumpsLeft = settings.airJumps;
            _airDashesLeft = settings.airDashes;
        }

        public Vector2 Velocity => _velocity;
        public bool IsGrounded { get; private set; }
        public bool IsDashing => _dashTimer > 0f;
        /// <summary>1 para a direita, -1 para a esquerda.</summary>
        public int Facing { get; private set; } = 1;

        public MovementSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }

        /// <summary>Chamar quando o botão de pulo for apertado. Fica guardado por jumpBufferTime.</summary>
        public void RequestJump() => _jumpBufferTimer = _settings.jumpBufferTime;

        /// <summary>Chamar quando o botão de pulo for solto: encurta o pulo se ainda estiver subindo.</summary>
        public void ReleaseJump()
        {
            if (!IsDashing && _velocity.y > 0f)
                _velocity.y *= _settings.jumpCutMultiplier;
        }

        /// <summary>Chamar quando o botão de dash for apertado. É consumido no próximo Tick.</summary>
        public void RequestDash() => _dashRequested = true;

        /// <param name="moveX">Entrada horizontal em [-1, 1].</param>
        /// <param name="grounded">Há chão logo abaixo do corpo.</param>
        /// <param name="hitCeiling">Há teto logo acima do corpo.</param>
        /// <param name="deltaTime">Duração do passo fixo.</param>
        /// <returns>A velocidade a aplicar neste passo.</returns>
        public Vector2 Tick(float moveX, bool grounded, bool hitCeiling, float deltaTime)
        {
            moveX = Mathf.Clamp(moveX, -1f, 1f);

            // Ignora o "chão" enquanto sobe, para o pulo não ser cancelado no primeiro passo.
            IsGrounded = grounded && _velocity.y <= 0f;

            if (IsGrounded)
            {
                _coyoteTimer = _settings.coyoteTime;
                _airJumpsLeft = _settings.airJumps;
                if (!IsDashing)
                    _airDashesLeft = _settings.airDashes;
                _velocity.y = 0f;
            }
            else
            {
                _coyoteTimer -= deltaTime;
            }

            if (hitCeiling && _velocity.y > 0f)
                _velocity.y = 0f;

            _dashCooldownTimer -= deltaTime;

            if (Mathf.Abs(moveX) > 0.01f && !IsDashing)
                Facing = moveX > 0f ? 1 : -1;

            TryStartDash(moveX);

            if (IsDashing)
            {
                _dashTimer -= deltaTime;
                if (_dashTimer > 0f)
                {
                    _velocity = new Vector2(_dashDirection * _settings.DashSpeed, 0f);
                    _jumpBufferTimer -= deltaTime;
                    return _velocity;
                }

                // Fim do dash: sai na velocidade de corrida, sem manter o impulso.
                _velocity.x = _dashDirection * Mathf.Min(Mathf.Abs(_velocity.x), _settings.moveSpeed);
            }

            UpdateHorizontal(moveX, deltaTime);
            UpdateJump();
            ApplyGravity(deltaTime);

            _jumpBufferTimer -= deltaTime;
            return _velocity;
        }

        private void TryStartDash(float moveX)
        {
            if (!_dashRequested)
                return;

            _dashRequested = false;

            var canDash = !IsDashing
                          && _dashCooldownTimer <= 0f
                          && (IsGrounded || _airDashesLeft > 0);
            if (!canDash)
                return;

            if (!IsGrounded)
                _airDashesLeft--;

            _dashDirection = Mathf.Abs(moveX) > 0.01f ? (moveX > 0f ? 1 : -1) : Facing;
            Facing = _dashDirection;
            _dashTimer = _settings.dashDuration;
            _dashCooldownTimer = _settings.dashCooldown;
        }

        private void UpdateHorizontal(float moveX, float deltaTime)
        {
            var target = moveX * _settings.moveSpeed;
            var accelerating = Mathf.Abs(moveX) > 0.01f;
            var rate = IsGrounded
                ? (accelerating ? _settings.groundAcceleration : _settings.groundDeceleration)
                : (accelerating ? _settings.airAcceleration : _settings.airDeceleration);

            _velocity.x = Mathf.MoveTowards(_velocity.x, target, rate * deltaTime);
        }

        private void UpdateJump()
        {
            if (_jumpBufferTimer <= 0f)
                return;

            if (_coyoteTimer > 0f)
            {
                _coyoteTimer = 0f;
            }
            else if (_airJumpsLeft > 0)
            {
                _airJumpsLeft--;
            }
            else
            {
                return;
            }

            _jumpBufferTimer = 0f;
            _velocity.y = _settings.JumpVelocity;
            IsGrounded = false;
        }

        private void ApplyGravity(float deltaTime)
        {
            var gravity = _settings.Gravity;
            if (_velocity.y < 0f)
                gravity *= _settings.fallGravityMultiplier;

            _velocity.y = Mathf.Max(_velocity.y - gravity * deltaTime, -_settings.maxFallSpeed);
        }
    }
}
