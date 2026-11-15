using Addicted.Content;
using Addicted.Gameplay.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Addicted.Tests
{
    public class PlatformerMotorTests
    {
        private const float Dt = 0.02f;

        private static MovementSettings Settings => MovementSettings.Default;

        private static PlatformerMotor GroundedMotor()
        {
            var motor = new PlatformerMotor(Settings);
            motor.Tick(0f, grounded: true, hitCeiling: false, Dt);
            return motor;
        }

        [Test]
        public void Corrida_AceleraAteAVelocidadeMaxima()
        {
            var motor = GroundedMotor();

            for (var i = 0; i < 50; i++)
                motor.Tick(1f, true, false, Dt);

            Assert.AreEqual(Settings.moveSpeed, motor.Velocity.x, 0.001f);
        }

        [Test]
        public void Corrida_ParaAoSoltarODirecional()
        {
            var motor = GroundedMotor();
            for (var i = 0; i < 50; i++)
                motor.Tick(1f, true, false, Dt);

            for (var i = 0; i < 50; i++)
                motor.Tick(0f, true, false, Dt);

            Assert.AreEqual(0f, motor.Velocity.x, 0.001f);
        }

        [Test]
        public void Pulo_NoChao_AplicaVelocidadeDePulo()
        {
            var motor = GroundedMotor();

            motor.RequestJump();
            var velocity = motor.Tick(0f, true, false, Dt);

            var expected = Settings.JumpVelocity - Settings.Gravity * Dt;
            Assert.AreEqual(expected, velocity.y, 0.001f);
            Assert.IsFalse(motor.IsGrounded);
        }

        [Test]
        public void Pulo_NoAr_SemCoyoteNemPuloExtra_EIgnorado()
        {
            var motor = GroundedMotor();
            for (var i = 0; i < 20; i++) // 0,4 s no ar, bem além do coyote time
                motor.Tick(0f, false, false, Dt);

            motor.RequestJump();
            var velocity = motor.Tick(0f, false, false, Dt);

            Assert.Less(velocity.y, 0f);
        }

        [Test]
        public void CoyoteTime_PermitePularLogoAposSairDaPlataforma()
        {
            var motor = GroundedMotor();
            motor.Tick(0f, false, false, Dt); // acabou de sair da borda

            motor.RequestJump();
            var velocity = motor.Tick(0f, false, false, Dt);

            Assert.Greater(velocity.y, 0f);
        }

        [Test]
        public void JumpBuffer_PuloApertadoAntesDeAterrissarEExecutado()
        {
            var motor = GroundedMotor();
            for (var i = 0; i < 20; i++)
                motor.Tick(0f, false, false, Dt);

            motor.RequestJump();
            motor.Tick(0f, false, false, Dt);         // ainda no ar
            var velocity = motor.Tick(0f, true, false, Dt); // aterrissou dentro do buffer

            Assert.Greater(velocity.y, 0f);
        }

        [Test]
        public void PuloDuplo_UsaPulosExtrasConfigurados()
        {
            var settings = Settings;
            settings.airJumps = 1;
            var motor = new PlatformerMotor(settings);
            motor.Tick(0f, true, false, Dt);
            for (var i = 0; i < 20; i++)
                motor.Tick(0f, false, false, Dt);

            motor.RequestJump();
            var velocity = motor.Tick(0f, false, false, Dt);

            Assert.Greater(velocity.y, 0f);
        }

        [Test]
        public void SoltarPulo_NaSubida_EncurtaOPulo()
        {
            var motor = GroundedMotor();
            motor.RequestJump();
            var before = motor.Tick(0f, true, false, Dt).y;

            motor.ReleaseJump();

            Assert.AreEqual(before * Settings.jumpCutMultiplier, motor.Velocity.y, 0.001f);
        }

        [Test]
        public void Teto_ZeraVelocidadeDeSubida()
        {
            var motor = GroundedMotor();
            motor.RequestJump();
            motor.Tick(0f, true, false, Dt);

            var velocity = motor.Tick(0f, false, hitCeiling: true, Dt);

            Assert.LessOrEqual(velocity.y, 0f);
        }

        [Test]
        public void Dash_PercorreADistanciaConfiguradaNaHorizontal()
        {
            var motor = GroundedMotor();
            motor.RequestDash();

            var distance = 0f;
            for (var guard = 0; guard < 100; guard++)
            {
                var velocity = motor.Tick(0f, true, false, Dt);
                if (!motor.IsDashing)
                    break;

                Assert.AreEqual(0f, velocity.y, 0.001f, "dash não sofre gravidade");
                distance += velocity.x * Dt;
            }

            Assert.AreEqual(Settings.dashDistance, distance, Settings.DashSpeed * Dt);
        }

        [Test]
        public void Dash_UsaADirecaoParaOndeOPersonagemOlha()
        {
            var motor = GroundedMotor();
            motor.Tick(-1f, true, false, Dt); // vira para a esquerda

            motor.RequestDash();
            var velocity = motor.Tick(0f, true, false, Dt);

            Assert.Less(velocity.x, 0f);
            Assert.AreEqual(-1, motor.Facing);
        }

        [Test]
        public void Dash_RespeitaCooldown()
        {
            var motor = GroundedMotor();
            motor.RequestDash();
            var steps = Mathf.CeilToInt(Settings.dashDuration / Dt) + 1;
            for (var i = 0; i < steps; i++)
                motor.Tick(0f, true, false, Dt);

            motor.RequestDash();
            motor.Tick(0f, true, false, Dt);

            Assert.IsFalse(motor.IsDashing);
        }

        [Test]
        public void DashNoAr_LimitadoPorAirDashes_ERecarregaNoChao()
        {
            var settings = Settings;
            settings.airDashes = 1;
            settings.dashCooldown = 0f;
            var motor = new PlatformerMotor(settings);
            motor.Tick(0f, true, false, Dt);
            motor.Tick(0f, false, false, Dt);

            motor.RequestDash();
            motor.Tick(0f, false, false, Dt);
            Assert.IsTrue(motor.IsDashing, "primeiro dash no ar");

            var steps = Mathf.CeilToInt(settings.dashDuration / Dt) + 1;
            for (var i = 0; i < steps; i++)
                motor.Tick(0f, false, false, Dt);

            motor.RequestDash();
            motor.Tick(0f, false, false, Dt);
            Assert.IsFalse(motor.IsDashing, "segundo dash no ar é recusado");

            for (var i = 0; i < 2; i++)
                motor.Tick(0f, true, false, Dt);
            motor.RequestDash();
            motor.Tick(0f, true, false, Dt);
            Assert.IsTrue(motor.IsDashing, "dash recarrega ao tocar o chão");
        }

        [Test]
        public void Queda_RespeitaVelocidadeMaxima()
        {
            var motor = GroundedMotor();

            for (var i = 0; i < 500; i++)
                motor.Tick(0f, false, false, Dt);

            Assert.AreEqual(-Settings.maxFallSpeed, motor.Velocity.y, 0.001f);
        }
    }
}
