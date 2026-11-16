using UnityEngine;

namespace Addicted.Gameplay.Enemies
{
    /// <summary>Perseguidor (cabeça triangular): corre até o jogador e para colado nele.</summary>
    public sealed class ChaserEnemy : BeltEnemy
    {
        [SerializeField, Min(0f)] private float stopDistance = 0.9f;

        protected override Vector2 DesiredVelocity(Vector2 toPlayer) =>
            toPlayer.magnitude <= stopDistance ? Vector2.zero : toPlayer.normalized * moveSpeed;
    }
}
