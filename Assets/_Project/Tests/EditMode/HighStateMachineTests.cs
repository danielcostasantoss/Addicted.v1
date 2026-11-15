using Addicted.Content;
using Addicted.Gameplay.Addiction;
using NUnit.Framework;

namespace Addicted.Tests
{
    public class HighStateMachineTests
    {
        private const float Dt = 0.02f;

        private static HighSettings Settings => HighSettings.Default;

        private static void Advance(HighStateMachine machine, float seconds)
        {
            for (var t = 0f; t < seconds; t += Dt)
                machine.Tick(Dt);
        }

        [Test]
        public void ComecaNaRotina_ComPulsoDeRepouso()
        {
            var machine = new HighStateMachine(Settings);

            Assert.AreEqual(HighState.Rotina, machine.State);
            Assert.AreEqual(Settings.pulseRest, machine.Pulse);
            Assert.AreEqual(1f, machine.SpeedMultiplier);
        }

        [Test]
        public void Dose_LevaAoPico_EAceleraOPersonagem()
        {
            var machine = new HighStateMachine(Settings);

            machine.TakeDose();

            Assert.AreEqual(HighState.Pico, machine.State);
            Assert.AreEqual(Settings.addictionPerDose, machine.Addiction);
            Assert.Greater(machine.SpeedMultiplier, 1f);
            Assert.Less(machine.AttackIntervalMultiplier, 1f);
        }

        [Test]
        public void Pico_AcabaEmCrash_EDepoisVoltaParaRotina()
        {
            var machine = new HighStateMachine(Settings);
            machine.TakeDose();
            var picoSeconds = Settings.addictionPerDose / Settings.picoDrainPerSecond;

            Advance(machine, picoSeconds + 0.1f);
            Assert.AreEqual(HighState.Crash, machine.State);
            Assert.Less(machine.SpeedMultiplier, 1f);

            Advance(machine, Settings.crashDuration + 0.1f);
            Assert.AreEqual(HighState.Rotina, machine.State);
        }

        [Test]
        public void DosesDemaisEmSeguida_EstouramOLimiteCardiaco()
        {
            var machine = new HighStateMachine(Settings);

            machine.TakeDose();
            machine.TakeDose();
            Assert.AreEqual(HighState.Pico, machine.State, "duas doses ainda cabem no limite");

            machine.TakeDose();
            Assert.AreEqual(HighState.LimiteCardiaco, machine.State);
            Assert.IsFalse(machine.CanAttack);
        }

        [Test]
        public void LimiteCardiaco_IgnoraNovasDoses_EAcabaEmCrash()
        {
            var machine = new HighStateMachine(Settings);
            for (var i = 0; i < 3; i++)
                machine.TakeDose();

            machine.TakeDose();
            Assert.AreEqual(HighState.LimiteCardiaco, machine.State);

            Advance(machine, Settings.cardiacDuration + 0.1f);
            Assert.AreEqual(HighState.Crash, machine.State);
            Assert.Less(machine.Pulse, Settings.pulseLimit);
        }

        [Test]
        public void Pulso_VoltaAoRepousoComOTempo()
        {
            var machine = new HighStateMachine(Settings);
            machine.TakeDose();

            Advance(machine, 30f);

            Assert.AreEqual(Settings.pulseRest, machine.Pulse, 0.01f);
        }
    }
}
