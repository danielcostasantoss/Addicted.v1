using System.Collections;
using Addicted.Content;
using Addicted.Gameplay.Movement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Addicted.Tests
{
    /// <summary>Testa o controlador com a física 2D de verdade (Rigidbody2D.Slide + colisores).</summary>
    public class PlatformerMovement2DTests
    {
        private const float GroundTop = 0f;
        private GameObject _root;

        [SetUp]
        public void SetUp() => _root = new GameObject("TestRoot");

        [TearDown]
        public void TearDown() => Object.Destroy(_root);

        private void Box(Vector2 center, Vector2 size)
        {
            var go = new GameObject("Box");
            go.transform.SetParent(_root.transform);
            go.transform.position = center;
            go.AddComponent<BoxCollider2D>().size = size;
        }

        private PlatformerMovement2D Player(Vector2 position)
        {
            var go = new GameObject("Player");
            go.transform.SetParent(_root.transform);
            go.transform.position = position;
            go.AddComponent<BoxCollider2D>().size = new Vector2(0.8f, 1.6f);
            go.AddComponent<Rigidbody2D>();
            return go.AddComponent<PlatformerMovement2D>();
        }

        private static IEnumerator Wait(float seconds)
        {
            for (var t = 0f; t < seconds; t += Time.fixedDeltaTime)
                yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator Cai_EParaEmCimaDoChao()
        {
            Box(new Vector2(0f, GroundTop - 0.5f), new Vector2(40f, 1f));
            var player = Player(new Vector2(0f, 3f));

            yield return Wait(1.5f);

            var bottom = player.GetComponent<Collider2D>().bounds.min.y;
            Assert.AreEqual(GroundTop, bottom, 0.05f, "o pé do personagem deve ficar na superfície do chão");
            Assert.IsTrue(player.Motor.IsGrounded);
        }

        [UnityTest]
        public IEnumerator Dash_PercorreADistanciaConfigurada()
        {
            Box(new Vector2(0f, GroundTop - 0.5f), new Vector2(40f, 1f));
            var player = Player(new Vector2(0f, 0.85f));
            yield return Wait(0.3f);

            var startX = player.transform.position.x;
            player.Motor.RequestDash();
            yield return Wait(0.5f);

            var travelled = player.transform.position.x - startX;
            var expected = MovementSettings.Default.dashDistance;
            Assert.AreEqual(expected, travelled, 0.6f);
        }

        [UnityTest]
        public IEnumerator Dash_EBloqueadoPorParede()
        {
            Box(new Vector2(0f, GroundTop - 0.5f), new Vector2(40f, 1f));
            const float wallLeft = 1.5f;
            Box(new Vector2(wallLeft + 0.5f, 2f), new Vector2(1f, 4f));
            var player = Player(new Vector2(0f, 0.85f));
            yield return Wait(0.3f);

            player.Motor.RequestDash();
            yield return Wait(0.5f);

            var right = player.GetComponent<Collider2D>().bounds.max.x;
            Assert.LessOrEqual(right, wallLeft + 0.05f, "o personagem não pode atravessar a parede");
        }

        [UnityTest]
        public IEnumerator PuloAlcancaAAlturaConfigurada()
        {
            Box(new Vector2(0f, GroundTop - 0.5f), new Vector2(40f, 1f));
            var player = Player(new Vector2(0f, 0.85f));
            yield return Wait(0.3f);

            var startY = player.transform.position.y;
            var peak = startY;
            player.Motor.RequestJump();
            for (var t = 0f; t < 1f; t += Time.fixedDeltaTime)
            {
                yield return new WaitForFixedUpdate();
                peak = Mathf.Max(peak, player.transform.position.y);
            }

            Assert.AreEqual(MovementSettings.Default.jumpHeight, peak - startY, 0.3f);
        }
    }
}
