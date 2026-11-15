using UnityEngine;

namespace Addicted.Content
{
    /// <summary>
    /// Define um personagem jogável (um vício). Um vício novo é um asset novo, não código novo.
    /// </summary>
    [CreateAssetMenu(fileName = "Addiction_", menuName = "Addicted/Addiction Definition")]
    public sealed class AddictionDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private MovementSettings movement = MovementSettings.Default;

        public string DisplayName => displayName;
        public string Description => description;
        public MovementSettings Movement => movement;
    }
}
