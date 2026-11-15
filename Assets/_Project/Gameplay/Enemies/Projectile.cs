using Addicted.Gameplay.Player;
using UnityEngine;

namespace Addicted.Gameplay.Enemies
{
    /// <summary>
    /// Tiro do atirador. A posição lógica fica no plano do chão (para respeitar a profundidade);
    /// o sprite é desenhado mais alto, na altura do peito.
    /// </summary>
    public sealed class Projectile : MonoBehaviour
    {
        private const float Radius = 0.35f;
        private const float Lifetime = 4f;

        private Vector2 _velocity;
        private LayerMask _playerLayers;
        private SpriteRenderer _renderer;
        private float _age;

        public static Projectile Spawn(Vector2 position, Vector2 velocity, float height,
            Sprite sprite, Material material, LayerMask playerLayers)
        {
            var root = new GameObject("Projectile");
            root.transform.position = position;

            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform);
            visual.transform.localPosition = new Vector3(0f, height, 0f);
            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            if (material != null)
                renderer.sharedMaterial = material;

            var projectile = root.AddComponent<Projectile>();
            projectile._velocity = velocity;
            projectile._playerLayers = playerLayers;
            projectile._renderer = renderer;
            return projectile;
        }

        private void Update()
        {
            transform.position += (Vector3)(_velocity * Time.deltaTime);
            _renderer.sortingOrder = Room.YSort.OrderFor(transform.position.y);

            var hit = Physics2D.OverlapCircle(transform.position, Radius, _playerLayers);
            if (hit != null)
            {
                var player = hit.GetComponentInParent<PlayerCharacter>();
                if (player != null)
                    player.TakeHit();
                Destroy(gameObject);
                return;
            }

            _age += Time.deltaTime;
            if (_age >= Lifetime)
                Destroy(gameObject);
        }
    }
}
