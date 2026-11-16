using Addicted.Content;
using UnityEngine;

namespace Addicted.Gameplay.Addiction
{
    public enum HighState
    {
        Rotina,
        Pico,
        Crash,
        LimiteCardiaco,
    }

    /// <summary>
    /// Ciclo do vício, sem dependência de cena: a dose leva ao Pico, o Pico acaba em Crash,
    /// e doses demais em pouco tempo estouram o pulso (limite cardíaco).
    /// </summary>
    public sealed class HighStateMachine
    {
        private HighSettings _settings;
        private float _timer;

        public HighStateMachine(HighSettings settings)
        {
            _settings = settings;
            Pulse = settings.pulseRest;
        }

        public HighState State { get; private set; } = HighState.Rotina;
        public float Addiction { get; private set; }
        public float Pulse { get; private set; }
        public HighSettings Settings => _settings;

        /// <summary>Segundos restantes do estado atual (0 na Rotina).</summary>
        public float TimeLeft => State switch
        {
            HighState.Pico => Addiction / _settings.picoDrainPerSecond,
            HighState.Rotina => 0f,
            _ => Mathf.Max(_timer, 0f),
        };

        public float SpeedMultiplier => State switch
        {
            HighState.Pico => _settings.picoSpeed,
            HighState.Crash => _settings.crashSpeed,
            HighState.LimiteCardiaco => _settings.cardiacSpeed,
            _ => 1f,
        };

        public float AttackIntervalMultiplier => State switch
        {
            HighState.Pico => _settings.picoAttackInterval,
            HighState.Crash => _settings.crashAttackInterval,
            _ => 1f,
        };

        public bool CanAttack => State != HighState.LimiteCardiaco;

        public void TakeDose()
        {
            if (State == HighState.LimiteCardiaco)
                return;

            Addiction = Mathf.Min(Addiction + _settings.addictionPerDose, _settings.addictionMax);
            Pulse = Mathf.Min(Pulse + _settings.pulsePerDose, _settings.pulseMax);

            if (Pulse >= _settings.pulseLimit)
            {
                State = HighState.LimiteCardiaco;
                _timer = _settings.cardiacDuration;
                Addiction = 0f;
                return;
            }

            State = HighState.Pico;
        }

        public void Tick(float deltaTime)
        {
            if (State != HighState.LimiteCardiaco)
                Pulse = Mathf.MoveTowards(Pulse, _settings.pulseRest, _settings.pulseRecoveryPerSecond * deltaTime);

            switch (State)
            {
                case HighState.Pico:
                    Addiction -= _settings.picoDrainPerSecond * deltaTime;
                    if (Addiction <= 0f)
                    {
                        Addiction = 0f;
                        EnterCrash();
                    }
                    break;

                case HighState.Crash:
                    _timer -= deltaTime;
                    if (_timer <= 0f)
                        State = HighState.Rotina;
                    break;

                case HighState.LimiteCardiaco:
                    _timer -= deltaTime;
                    if (_timer <= 0f)
                    {
                        // Sobreviveu: o pulso cai para logo abaixo do limite e vem o Crash.
                        Pulse = Mathf.Min(Pulse, _settings.pulseLimit - _settings.pulsePerDose);
                        EnterCrash();
                    }
                    break;
            }
        }

        private void EnterCrash()
        {
            State = HighState.Crash;
            _timer = _settings.crashDuration;
        }
    }
}
