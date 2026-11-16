using System.Collections;
using System.Linq;
using Addicted.Gameplay.Addiction;
using Addicted.Gameplay.Combat;
using Addicted.Gameplay.Player;
using Addicted.Gameplay.Room;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Addicted.Tests
{
    /// <summary>Testes de fumaça da cena Escritorio: carregam a cena de verdade e exercitam os sistemas.</summary>
    public class EscritorioSceneTests
    {
        [UnitySetUp]
        public IEnumerator LoadScene()
        {
            yield return SceneManager.LoadSceneAsync("Escritorio", LoadSceneMode.Single);
            yield return null;
        }

        private static IEnumerator Wait(float seconds)
        {
            for (var t = 0f; t < seconds; t += Time.fixedDeltaTime)
                yield return new WaitForFixedUpdate();
        }

        private static SpriteRenderer Body(PlayerCharacter player) =>
            player.transform.Find("Body").GetComponent<SpriteRenderer>();

        [UnityTest]
        public IEnumerator Dose_TrocaOSpriteParaOPico()
        {
            var player = PlayerCharacter.Current;
            Assert.IsNotNull(player, "a cena precisa ter um jogador");
            yield return null;
            var rotina = Body(player).sprite;

            player.TakeDose();
            yield return null;

            Assert.AreEqual(HighState.Pico, player.High.State);
            Assert.AreNotEqual(rotina, Body(player).sprite);
            Assert.AreEqual("player_pico", Body(player).sprite.name);
        }

        [UnityTest]
        public IEnumerator Jogador_NaoSaiDaFaixaDoChao()
        {
            var player = PlayerCharacter.Current;
            // Sem teclado no teste: a ação de movimento lê zero, então empurra-se com dashes para cima.
            for (var i = 0; i < 8; i++)
            {
                player.Movement.Motor.RequestDash();
                player.Movement.Motor.Tick(Vector2.up, 0f);
                yield return Wait(0.7f);
            }

            Assert.Less(player.transform.position.y, -1.2f, "a parede de cima deve segurar o jogador");
        }

        [UnityTest]
        public IEnumerator Perseguidores_AndamAteOJogador()
        {
            var player = PlayerCharacter.Current;
            var chaser = Object.FindObjectsByType<Health>(FindObjectsSortMode.None)
                .First(h => h.name == "Chaser");
            var before = Vector2.Distance(chaser.transform.position, player.transform.position);

            yield return Wait(1f);

            var after = Vector2.Distance(chaser.transform.position, player.transform.position);
            Assert.Less(after, before);
        }

        [UnityTest]
        public IEnumerator Ataque_AcertaInimigoNaFrente_ESobeOCombo()
        {
            var player = PlayerCharacter.Current;
            var enemy = Object.FindObjectsByType<Health>(FindObjectsSortMode.None).First();
            enemy.transform.position = player.transform.position + new Vector3(0.9f, 0f, 0f);
            enemy.GetComponent<Rigidbody2D>().position = enemy.transform.position;
            Physics2D.SyncTransforms();
            var before = enemy.Current;

            var landed = player.GetComponent<PlayerAttack>().TryAttack();

            Assert.AreEqual(1, landed);
            Assert.AreEqual(before - 1, enemy.Current);
            Assert.AreEqual(1, player.Combo);
            yield return null;
        }

        [UnityTest]
        public IEnumerator LimparAsDuasOndas_AbreASaida()
        {
            var room = RoomController.Current;
            Assert.IsNotNull(room);
            Assert.AreEqual(2, room.WaveCount);
            Assert.AreEqual(1, room.WaveNumber);
            Assert.IsFalse(room.IsCleared);

            for (var wave = 0; wave < 2; wave++)
            {
                foreach (var enemy in Object.FindObjectsByType<Health>(FindObjectsSortMode.None))
                    enemy.TakeHit(99, Vector2.zero);
                yield return null;
            }

            Assert.IsTrue(room.IsCleared);
            Assert.AreEqual(0, Object.FindObjectsByType<Health>(FindObjectsSortMode.None).Length);
        }

        [UnityTest]
        public IEnumerator ZonaDeDose_AoPisar_DaADose()
        {
            var player = PlayerCharacter.Current;
            var zone = Object.FindFirstObjectByType<DoseZone>();
            player.GetComponent<Rigidbody2D>().position = zone.transform.position;
            player.transform.position = zone.transform.position;
            Physics2D.SyncTransforms();

            yield return null;
            yield return null;

            Assert.AreEqual(HighState.Pico, player.High.State);
            Assert.IsFalse(zone.IsAvailable);
        }
    }
}
