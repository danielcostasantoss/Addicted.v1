using UnityEngine;

namespace Addicted.Gameplay.Room
{
    /// <summary>Ordena o sprite pela profundidade: quem está mais abaixo na faixa do chão passa na frente.</summary>
    public sealed class YSort : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer target;
        [SerializeField] private int offset;

        public static int OrderFor(float y) => Mathf.RoundToInt(-y * 100f);

        private void LateUpdate()
        {
            if (target != null)
                target.sortingOrder = OrderFor(transform.position.y) + offset;
        }
    }
}
