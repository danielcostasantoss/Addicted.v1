using Addicted.Content;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Addicted.Gameplay.Movement
{
    /// <summary>
    /// Controlador de plataforma 2D comum a todos os personagens.
    /// Usa um Rigidbody2D cinemático movido por Rigidbody2D.Slide; a lógica fica no PlatformerMotor.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public sealed class PlatformerMovement2D : MonoBehaviour
    {
        [Tooltip("Vício controlado. Se vazio, usa os parâmetros abaixo.")]
        [SerializeField] private AddictionDefinition addiction;
        [SerializeField] private MovementSettings fallbackSettings = MovementSettings.Default;

        [Header("Entrada")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference dashAction;

        [Header("Colisão")]
        [SerializeField] private LayerMask groundLayers = 1; // Default
        [SerializeField, Min(0.001f)] private float probeDistance = 0.05f;

        private readonly RaycastHit2D[] _hits = new RaycastHit2D[4];
        private Rigidbody2D _body;
        private PlatformerMotor _motor;
        private ContactFilter2D _filter;
        private Rigidbody2D.SlideMovement _slide;

        public PlatformerMotor Motor => _motor;

        private MovementSettings ActiveSettings => addiction != null ? addiction.Movement : fallbackSettings;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;

            _motor = new PlatformerMotor(ActiveSettings);

            _filter = new ContactFilter2D { useTriggers = false };
            _filter.SetLayerMask(groundLayers);

            _slide = new Rigidbody2D.SlideMovement
            {
                maxIterations = 3,
                surfaceSlideAngle = 90f,
                gravitySlipAngle = 90f,
                surfaceUp = Vector2.up,
                surfaceAnchor = Vector2.zero,
                gravity = Vector2.zero, // a gravidade é aplicada pelo motor
                useSimulationMove = true,
            };
            _slide.SetLayerMask(groundLayers);
        }

        private void OnEnable()
        {
            Enable(moveAction);
            if (Enable(jumpAction))
            {
                jumpAction.action.performed += OnJumpPressed;
                jumpAction.action.canceled += OnJumpReleased;
            }

            if (Enable(dashAction))
                dashAction.action.performed += OnDashPressed;
        }

        private void OnDisable()
        {
            if (jumpAction != null)
            {
                jumpAction.action.performed -= OnJumpPressed;
                jumpAction.action.canceled -= OnJumpReleased;
            }

            if (dashAction != null)
                dashAction.action.performed -= OnDashPressed;
        }

        private void FixedUpdate()
        {
            _motor.Settings = ActiveSettings;

            var moveX = moveAction != null ? moveAction.action.ReadValue<Vector2>().x : 0f;
            var grounded = Probe(Vector2.down);
            var hitCeiling = Probe(Vector2.up);

            var velocity = _motor.Tick(moveX, grounded, hitCeiling, Time.fixedDeltaTime);
            _body.Slide(velocity, Time.fixedDeltaTime, _slide);
        }

        private bool Probe(Vector2 direction) => _body.Cast(direction, _filter, _hits, probeDistance) > 0;

        private void OnJumpPressed(InputAction.CallbackContext _) => _motor.RequestJump();
        private void OnJumpReleased(InputAction.CallbackContext _) => _motor.ReleaseJump();
        private void OnDashPressed(InputAction.CallbackContext _) => _motor.RequestDash();

        private static bool Enable(InputActionReference reference)
        {
            if (reference == null || reference.action == null)
                return false;

            reference.action.Enable();
            return true;
        }

        private void OnValidate()
        {
            if (_motor != null)
                _motor.Settings = ActiveSettings;
        }
    }
}
