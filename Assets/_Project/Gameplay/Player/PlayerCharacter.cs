using Addicted.Content;
using Addicted.Gameplay.Addiction;
using Addicted.Gameplay.Movement;
using UnityEngine;

namespace Addicted.Gameplay.Player
{
    /// <summary>
    /// Liga o ciclo do vício ao personagem: troca o sprite por estado,
    /// ajusta a velocidade do movimento e conta o combo.
    /// </summary>
    [RequireComponent(typeof(BeltMovement2D))]
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private HighSettings highSettings = HighSettings.Default;
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private Sprite rotinaSprite;
        [SerializeField] private Sprite picoSprite;
        [SerializeField] private Sprite crashSprite;
        [SerializeField] private Sprite limiteSprite;
        [SerializeField, Min(0f)] private float comboWindow = 2.5f;

        private BeltMovement2D _movement;
        private HighStateMachine _high;
        private float _comboTimer;
        private float _hitFlashTimer;

        public static PlayerCharacter Current { get; private set; }

        public HighStateMachine High => _high ??= new HighStateMachine(highSettings);
        public BeltMovement2D Movement => _movement;
        public int Combo { get; private set; }
        public int Facing => _movement.Motor.Facing;

        private void Awake() => _movement = GetComponent<BeltMovement2D>();

        private void OnEnable() => Current = this;

        private void OnDisable()
        {
            if (Current == this)
                Current = null;
        }

        public void TakeDose() => High.TakeDose();

        public void RegisterHit()
        {
            Combo++;
            _comboTimer = comboWindow;
        }

        /// <summary>Levou um tiro: por enquanto só pisca e perde o combo (a vida ainda não foi definida).</summary>
        public void TakeHit()
        {
            Combo = 0;
            _hitFlashTimer = 0.15f;
        }

        private void Update()
        {
            High.Tick(Time.deltaTime);
            _movement.Motor.SpeedMultiplier = High.SpeedMultiplier;

            if (Combo > 0)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0f)
                    Combo = 0;
            }

            if (body == null)
                return;

            body.sprite = High.State switch
            {
                HighState.Pico => picoSprite,
                HighState.Crash => crashSprite,
                HighState.LimiteCardiaco => limiteSprite,
                _ => rotinaSprite,
            };
            body.flipX = Facing < 0;

            _hitFlashTimer -= Time.deltaTime;
            body.color = _hitFlashTimer > 0f ? new Color(1f, 0.42f, 0.29f) : Color.white;
        }
    }
}
