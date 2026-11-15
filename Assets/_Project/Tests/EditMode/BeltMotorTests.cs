using Addicted.Content;
using Addicted.Gameplay.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Addicted.Tests
{
    public class BeltMotorTests
    {
        private const float Dt = 0.02f;

        private static BeltMovementSettings Settings => BeltMovementSettings.Default;

        private static Vector2 Run(BeltMotor motor, Vector2 input, int steps)
        {
            var velocity = Vector2.zero;
            for (var i = 0; i < steps; i++)
                velocity = motor.Tick(input, Dt);
            return velocity;
        }

        [Test]
        public void AndaNaHorizontal_AteAVelocidadeMaxima()
        {
            var motor = new BeltMotor(Settings);

            var velocity = Run(motor, Vector2.right, 50);

            Assert.AreEqual(Settings.moveSpeed, velocity.x, 0.001f);
            Assert.AreEqual(0f, velocity.y, 0.001f);
        }

        [Test]
        public void Profundidade_EMaisLentaQueAHorizontal()
        {
            var motor = new BeltMotor(Settings);

            var velocity = Run(motor, Vector2.up, 50);

            Assert.AreEqual(Settings.moveSpeed * Settings.depthSpeedRatio, velocity.y, 0.001f);
        }

        [Test]
        public void ParaAoSoltarODirecional()
        {
            var motor = new BeltMotor(Settings);
            Run(motor, Vector2.right, 50);

            var velocity = Run(motor, Vector2.zero, 50);

            Assert.AreEqual(0f, velocity.magnitude, 0.001f);
        }

        [Test]
        public void Facing_SegueADirecaoHorizontal()
        {
            var motor = new BeltMotor(Settings);

            motor.Tick(Vector2.left, Dt);

            Assert.AreEqual(-1, motor.Facing);
        }

        [Test]
        public void MultiplicadorDeVelocidade_EAplicado()
        {
            var motor = new BeltMotor(Settings) { SpeedMultiplier = 1.5f };

            var velocity = Run(motor, Vector2.right, 80);

            Assert.AreEqual(Settings.moveSpeed * 1.5f, velocity.x, 0.001f);
        }

        [Test]
        public void Dash_SemDirecional_VaiParaOndeOlha()
        {
            var motor = new BeltMotor(Settings);
            motor.Tick(Vector2.left, Dt);
            Run(motor, Vector2.zero, 20);

            motor.RequestDash();
            var velocity = motor.Tick(Vector2.zero, Dt);

            Assert.IsTrue(motor.IsDashing);
            Assert.AreEqual(-Settings.DashSpeed, velocity.x, 0.001f);
        }

        [Test]
        public void Dash_RespeitaRecarga()
        {
            var motor = new BeltMotor(Settings);
            motor.RequestDash();
            Run(motor, Vector2.zero, Mathf.CeilToInt(Settings.dashDuration / Dt) + 1);
            Assert.IsFalse(motor.IsDashing);

            motor.RequestDash();
            motor.Tick(Vector2.zero, Dt);

            Assert.IsFalse(motor.IsDashing, "ainda em recarga");
        }
    }
}
