namespace Addicted.Core
{
    /// <summary>
    /// Gerador de números aleatórios com seed própria (ver ADR-001).
    /// Use uma instância por sistema (mapa, loot, inimigos) em vez de UnityEngine.Random,
    /// para que a mesma seed sempre gere a mesma run.
    /// </summary>
    public sealed class SeededRandom
    {
        private readonly System.Random _random;

        public int Seed { get; }

        public SeededRandom(int seed)
        {
            Seed = seed;
            _random = new System.Random(seed);
        }

        /// <summary>Inteiro em [minInclusive, maxExclusive).</summary>
        public int Range(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);

        /// <summary>Float em [minInclusive, maxInclusive].</summary>
        public float Range(float minInclusive, float maxInclusive) =>
            minInclusive + (float)_random.NextDouble() * (maxInclusive - minInclusive);

        /// <summary>Cria um gerador derivado e independente, para separar sistemas.</summary>
        public SeededRandom Fork() => new SeededRandom(_random.Next());
    }
}
