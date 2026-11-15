using System;
using UnityEngine;

namespace Addicted.Content
{
    /// <summary>
    /// Parâmetros do movimento "briga de rua": vista lateral com profundidade,
    /// andando nos dois eixos dentro da faixa do chão.
    /// </summary>
    [Serializable]
    public struct BeltMovementSettings
    {
        [Min(0f)] public float moveSpeed;
        [Tooltip("Fração da velocidade usada no eixo de profundidade (cima/baixo).")]
        [Range(0.1f, 1f)] public float depthSpeedRatio;
        [Min(0f)] public float acceleration;
        [Min(0f)] public float deceleration;

        [Header("Dash")]
        [Min(0f)] public float dashDistance;
        [Min(0.01f)] public float dashDuration;
        [Min(0f)] public float dashCooldown;

        public float DashSpeed => dashDistance / dashDuration;

        public static BeltMovementSettings Default => new BeltMovementSettings
        {
            moveSpeed = 6f,
            depthSpeedRatio = 0.65f,
            acceleration = 70f,
            deceleration = 90f,
            dashDistance = 3.2f,
            dashDuration = 0.16f,
            dashCooldown = 0.5f,
        };
    }
}
