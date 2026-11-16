using Addicted.Gameplay.Addiction;
using Addicted.Gameplay.Player;
using Addicted.Gameplay.Room;
using UnityEngine;

namespace Addicted.Gameplay.UI
{
    /// <summary>
    /// HUD provisório (IMGUI) para o protótipo: barras de vício e pulso, estado, combo e ondas.
    /// Deve ser trocado por uma interface de verdade quando o layout for definido.
    /// </summary>
    public sealed class DebugHud : MonoBehaviour
    {
        private static readonly Color Panel = new Color(0.07f, 0.06f, 0.1f, 0.85f);
        private static readonly Color Track = new Color(0.16f, 0.14f, 0.24f);
        private static readonly Color Magenta = new Color(1f, 0.24f, 0.65f);
        private static readonly Color Coral = new Color(1f, 0.42f, 0.29f);
        private static readonly Color Cyan = new Color(0.24f, 0.95f, 1f);
        private static readonly Color Lime = new Color(0.78f, 0.95f, 0.29f);

        private GUIStyle _label;
        private GUIStyle _big;
        private GUIStyle _center;

        private void OnGUI()
        {
            var player = PlayerCharacter.Current;
            if (player == null)
                return;

            var scale = Screen.height / 720f;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            var width = Screen.width / scale;
            EnsureStyles();

            var high = player.High;
            var settings = high.Settings;

            Fill(new Rect(32, 62, 354, 90), Panel);
            Label(new Rect(44, 70, 60, 20), "VIDA", Color.gray);
            Label(new Rect(340, 70, 40, 20), "???", Color.gray);
            Fill(new Rect(106, 76, 224, 8), Track);

            Label(new Rect(44, 96, 60, 20), "VÍCIO", Magenta);
            Bar(new Rect(106, 102, 224, 8), high.Addiction / settings.addictionMax, Magenta);
            Label(new Rect(340, 96, 40, 20), Mathf.RoundToInt(high.Addiction).ToString(), Magenta);

            Label(new Rect(44, 122, 60, 20), "PULSO", Coral);
            var pulseRect = new Rect(106, 128, 224, 8);
            Bar(pulseRect, high.Pulse / settings.pulseMax, Coral);
            Fill(new Rect(pulseRect.x + pulseRect.width * settings.pulseLimit / settings.pulseMax, 124, 3, 16), Color.white);
            Label(new Rect(340, 122, 40, 20), Mathf.RoundToInt(high.Pulse).ToString(), Coral);

            if (high.State != HighState.Rotina)
            {
                var (text, color) = high.State switch
                {
                    HighState.Pico => ("PICO", Color.white),
                    HighState.Crash => ("CRASH", new Color(0.55f, 0.54f, 0.59f)),
                    _ => ("LIMITE CARDÍACO", Coral),
                };
                var box = new Rect(width / 2f - 130, 72, 260, 42);
                Fill(box, color);
                _center.normal.textColor = new Color(0.04f, 0.04f, 0.06f);
                GUI.Label(box, $"{text} · {high.TimeLeft:0.0} s", _center);
            }

            if (player.Combo > 1)
            {
                var box = new Rect(width - 109, 62, 77, 74);
                Fill(box, Panel);
                _big.normal.textColor = Cyan;
                GUI.Label(new Rect(box.x, box.y + 2, box.width, 46), $"×{player.Combo}", _big);
                _center.normal.textColor = Color.gray;
                GUI.Label(new Rect(box.x, box.y + 48, box.width, 20), "COMBO", _label);
            }

            var room = RoomController.Current;
            if (room != null)
            {
                var text = room.IsCleared
                    ? "SAÍDA ABERTA"
                    : $"ONDA {room.WaveNumber}/{room.WaveCount} · {room.EnemiesAlive} inimigo(s)";
                var box = new Rect(width - 292, 150, 260, 26);
                Fill(box, Panel);
                _center.normal.textColor = room.IsCleared ? Lime : Magenta;
                var size = _center.fontSize;
                _center.fontSize = 14;
                GUI.Label(box, text, _center);
                _center.fontSize = size;
            }

            _label.normal.textColor = new Color(1f, 1f, 1f, 0.6f);
            GUI.Label(new Rect(32, 690, 900, 20),
                "WASD/setas: andar · Shift: dash · Clique esquerdo ou Enter: atacar · Pise no cristal para tomar a dose",
                _label);
        }

        private void EnsureStyles()
        {
            if (_label != null)
                return;

            _label = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _center = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _big = new GUIStyle(GUI.skin.label) { fontSize = 38, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        }

        private void Label(Rect rect, string text, Color color)
        {
            _label.normal.textColor = color;
            GUI.Label(rect, text, _label);
        }

        private static void Bar(Rect rect, float fraction, Color color)
        {
            Fill(rect, Track);
            Fill(new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(fraction), rect.height), color);
        }

        private static void Fill(Rect rect, Color color)
        {
            var previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;
        }
    }
}
