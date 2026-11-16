using System;
using UnityEngine;

namespace Addicted.Content
{
    /// <summary>
    /// Regras provisórias do ciclo Rotina → Pico → Crash e do limite cardíaco.
    /// Os números são de protótipo, para ver os estados funcionando.
    /// </summary>
    [Serializable]
    public struct HighSettings
    {
        [Header("Barra do vício")]
        [Min(1f)] public float addictionMax;
        [Min(0f)] public float addictionPerDose;
        [Tooltip("Quanto a barra esvazia por segundo durante o Pico.")]
        [Min(0.01f)] public float picoDrainPerSecond;
        [Min(0f)] public float crashDuration;

        [Header("Pulso")]
        [Min(0f)] public float pulseRest;
        [Min(0f)] public float pulsePerDose;
        [Min(0f)] public float pulseRecoveryPerSecond;
        [Min(0f)] public float pulseLimit;
        [Min(0f)] public float pulseMax;
        [Min(0f)] public float cardiacDuration;

        [Header("Multiplicadores de velocidade")]
        [Min(0f)] public float picoSpeed;
        [Min(0f)] public float crashSpeed;
        [Min(0f)] public float cardiacSpeed;

        [Header("Multiplicadores do intervalo entre ataques")]
        [Min(0.05f)] public float picoAttackInterval;
        [Min(0.05f)] public float crashAttackInterval;

        public static HighSettings Default => new HighSettings
        {
            addictionMax = 100f,
            addictionPerDose = 60f,
            picoDrainPerSecond = 12f,
            crashDuration = 3f,
            pulseRest = 70f,
            pulsePerDose = 45f,
            pulseRecoveryPerSecond = 6f,
            pulseLimit = 180f,
            pulseMax = 200f,
            cardiacDuration = 2.5f,
            picoSpeed = 1.5f,
            crashSpeed = 0.6f,
            cardiacSpeed = 0.25f,
            picoAttackInterval = 0.55f,
            crashAttackInterval = 1.6f,
        };
    }
}
