using UnityEngine;

namespace Addicted.Gameplay.Movement
{
    /// <summary>Configuração comum de Rigidbody2D.Slide para corpos cinemáticos sem gravidade.</summary>
    public static class BeltPhysics
    {
        public static Rigidbody2D.SlideMovement CreateSlide(LayerMask worldLayers)
        {
            var slide = new Rigidbody2D.SlideMovement
            {
                maxIterations = 3,
                surfaceSlideAngle = 90f,
                gravitySlipAngle = 90f,
                surfaceUp = Vector2.up,
                surfaceAnchor = Vector2.zero,
                gravity = Vector2.zero,
                useSimulationMove = true,
            };
            slide.SetLayerMask(worldLayers);
            return slide;
        }

        public static void SetupKinematic(Rigidbody2D body)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
}
