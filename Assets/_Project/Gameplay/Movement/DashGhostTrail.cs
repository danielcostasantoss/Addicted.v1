using UnityEngine;

namespace Addicted.Gameplay.Movement
{
    /// <summary>Rastro do dash: cópias do sprite em ciano e magenta que somem rápido.</summary>
    public sealed class DashGhostTrail : MonoBehaviour
    {
        [SerializeField] private BeltMovement2D movement;
        [SerializeField] private SpriteRenderer source;
        [SerializeField, Min(0.01f)] private float interval = 0.035f;
        [SerializeField, Min(0.01f)] private float ghostLifetime = 0.25f;
        [SerializeField] private Color colorA = new Color(0.24f, 0.95f, 1f, 0.7f);
        [SerializeField] private Color colorB = new Color(1f, 0.24f, 0.65f, 0.7f);

        private float _timer;
        private bool _useB;

        private void Update()
        {
            if (movement == null || source == null || !movement.Motor.IsDashing)
            {
                _timer = 0f;
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer > 0f)
                return;

            _timer = interval;
            SpawnGhost();
        }

        private void SpawnGhost()
        {
            var ghost = new GameObject("DashGhost");
            ghost.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
            ghost.transform.localScale = source.transform.lossyScale;

            var renderer = ghost.AddComponent<SpriteRenderer>();
            renderer.sprite = source.sprite;
            renderer.flipX = source.flipX;
            renderer.sharedMaterial = source.sharedMaterial;
            renderer.sortingOrder = source.sortingOrder - 1;
            renderer.color = _useB ? colorB : colorA;
            _useB = !_useB;

            ghost.AddComponent<FadeAndDestroy>().Begin(renderer, ghostLifetime);
        }
    }

    public sealed class FadeAndDestroy : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private float _lifetime;
        private float _age;
        private float _startAlpha;

        public void Begin(SpriteRenderer renderer, float lifetime)
        {
            _renderer = renderer;
            _lifetime = lifetime;
            _startAlpha = renderer.color.a;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_renderer == null || _age >= _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            var color = _renderer.color;
            color.a = _startAlpha * (1f - _age / _lifetime);
            _renderer.color = color;
        }
    }
}
