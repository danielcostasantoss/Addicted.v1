using Addicted.Gameplay.Combat;
using UnityEngine;

namespace Addicted.Gameplay.Room
{
    /// <summary>
    /// Sala de combate: libera as ondas de inimigos em ordem e abre a saída quando a última acaba.
    /// Cada onda é um objeto-pai com os inimigos dentro, desativado até a sua vez.
    /// </summary>
    public sealed class RoomController : MonoBehaviour
    {
        [SerializeField] private GameObject[] waves;
        [SerializeField] private SpriteRenderer exitDoor;
        [SerializeField] private Color lockedColor = new Color(1f, 0.24f, 0.65f);
        [SerializeField] private Color openColor = new Color(0.78f, 0.95f, 0.29f);

        private int _waveIndex = -1;

        public static RoomController Current { get; private set; }

        public bool IsCleared { get; private set; }
        public int WaveNumber => Mathf.Min(_waveIndex + 1, WaveCount);
        public int WaveCount => waves != null ? waves.Length : 0;
        public int EnemiesAlive { get; private set; }

        private void OnEnable() => Current = this;

        private void OnDisable()
        {
            if (Current == this)
                Current = null;
        }

        private void Start()
        {
            foreach (var wave in waves)
                wave.SetActive(false);

            if (exitDoor != null)
                exitDoor.color = lockedColor;

            StartNextWave();
        }

        private void StartNextWave()
        {
            _waveIndex++;
            if (_waveIndex >= WaveCount)
            {
                IsCleared = true;
                if (exitDoor != null)
                    exitDoor.color = openColor;
                return;
            }

            var wave = waves[_waveIndex];
            wave.SetActive(true);

            var enemies = wave.GetComponentsInChildren<Health>();
            EnemiesAlive = enemies.Length;
            foreach (var enemy in enemies)
                enemy.Died += OnEnemyDied;

            if (EnemiesAlive == 0)
                StartNextWave();
        }

        private void OnEnemyDied(Health enemy)
        {
            enemy.Died -= OnEnemyDied;
            EnemiesAlive--;
            if (EnemiesAlive <= 0)
                StartNextWave();
        }
    }
}
