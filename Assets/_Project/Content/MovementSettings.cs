using System;
using UnityEngine;

namespace Addicted.Content
{
    /// <summary>
    /// Parâmetros de movimento de plataforma de um personagem.
    /// O pulo é definido por altura e tempo até o ápice; gravidade e velocidade inicial são derivadas.
    /// </summary>
    [Serializable]
    public struct MovementSettings
    {
        [Header("Corrida")]
        [Min(0f)] public float moveSpeed;
        [Min(0f)] public float groundAcceleration;
        [Min(0f)] public float groundDeceleration;
        [Min(0f)] public float airAcceleration;
        [Min(0f)] public float airDeceleration;

        [Header("Pulo")]
        [Min(0.01f)] public float jumpHeight;
        [Min(0.01f)] public float timeToJumpApex;
        [Tooltip("Multiplicador da gravidade na queda. Acima de 1 deixa o pulo mais 'pesado'.")]
        [Min(1f)] public float fallGravityMultiplier;
        [Tooltip("Fração da velocidade vertical mantida ao soltar o botão de pulo na subida.")]
        [Range(0f, 1f)] public float jumpCutMultiplier;
        [Min(0f)] public float maxFallSpeed;
        [Tooltip("Tempo após sair de uma plataforma em que o pulo ainda é aceito.")]
        [Min(0f)] public float coyoteTime;
        [Tooltip("Tempo em que um pulo apertado antes de tocar o chão fica guardado.")]
        [Min(0f)] public float jumpBufferTime;
        [Tooltip("Pulos extras no ar (0 = sem pulo duplo).")]
        [Min(0)] public int airJumps;

        [Header("Dash")]
        [Min(0f)] public float dashDistance;
        [Min(0.01f)] public float dashDuration;
        [Min(0f)] public float dashCooldown;
        [Tooltip("Dashes permitidos antes de tocar o chão de novo.")]
        [Min(0)] public int airDashes;

        public float Gravity => 2f * jumpHeight / (timeToJumpApex * timeToJumpApex);
        public float JumpVelocity => 2f * jumpHeight / timeToJumpApex;
        public float DashSpeed => dashDistance / dashDuration;

        public static MovementSettings Default => new MovementSettings
        {
            moveSpeed = 8f,
            groundAcceleration = 90f,
            groundDeceleration = 110f,
            airAcceleration = 60f,
            airDeceleration = 40f,
            jumpHeight = 3.5f,
            timeToJumpApex = 0.4f,
            fallGravityMultiplier = 1.8f,
            jumpCutMultiplier = 0.5f,
            maxFallSpeed = 22f,
            coyoteTime = 0.1f,
            jumpBufferTime = 0.12f,
            airJumps = 0,
            dashDistance = 3.5f,
            dashDuration = 0.15f,
            dashCooldown = 0.6f,
            airDashes = 1,
        };
    }
}
