using Addicted.Content;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Addicted.Gameplay.Movement
{
    /// <summary>
    /// Movimento "briga de rua" do jogador: anda nos dois eixos dentro da faixa do chão e dá dash.
    /// O colisor fica nos pés; as paredes da sala (camada World) limitam a área.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public sealed class BeltMovement2D : MonoBehaviour
    {
        [SerializeField] private BeltMovementSettings settings = BeltMovementSettings.Default;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference dashAction;
        [SerializeField] private LayerMask worldLayers;

        private Rigidbody2D _body;
        private BeltMotor _motor;
        private Rigidbody2D.SlideMovement _slide;

        /// <summary>Entrada usada quando não há ação de movimento ligada (testes, IA).</summary>
        public Vector2 InputOverride { get; set; }

        public BeltMotor Motor => _motor ??= new BeltMotor(settings);

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            BeltPhysics.SetupKinematic(_body);
            _slide = BeltPhysics.CreateSlide(worldLayers);
        }

        private void OnEnable()
        {
            if (moveAction != null)
                moveAction.action.Enable();

            if (dashAction != null)
            {
                dashAction.action.Enable();
                dashAction.action.performed += OnDash;
            }
        }

        private void OnDisable()
        {
            if (dashAction != null)
                dashAction.action.performed -= OnDash;
        }

        private void FixedUpdate()
        {
            Motor.Settings = settings;
            var input = moveAction != null ? moveAction.action.ReadValue<Vector2>() : InputOverride;
            var velocity = Motor.Tick(input, Time.fixedDeltaTime);
            _body.Slide(velocity, Time.fixedDeltaTime, _slide);
        }

        private void OnDash(InputAction.CallbackContext _) => Motor.RequestDash();
    }
}
