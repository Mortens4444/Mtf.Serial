using Mtf.Serial.Enums;
using System.Collections.Generic;
using System.Drawing;

namespace Mtf.Serial.Services
{
    public static class ColorConverter
    {
        private static readonly Dictionary<(DosColor, Mode, TerminalType), Color> colors = new Dictionary<(DosColor, Mode, TerminalType), Color>
        {
            {(DosColor.Black, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(128, 128, 128)},
            {(DosColor.Black, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 0, 0)},
            {(DosColor.Black, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(129, 131, 131)},
            {(DosColor.Black, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(0, 0, 0)},
            {(DosColor.Black, Mode.Light, TerminalType.PuTTY), Color.FromArgb(85, 85, 85)},
            {(DosColor.Black, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 0, 0)},
            {(DosColor.Black, Mode.Light, TerminalType.xterm), Color.FromArgb(127, 127, 127)},
            {(DosColor.Black, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 0, 0)},
            {(DosColor.Black, Mode.Light, TerminalType.XWindow), Color.FromArgb(127, 127, 127)},
            {(DosColor.Black, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 0, 0)},

            {(DosColor.Red, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 0, 0)},
            {(DosColor.Red, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(128, 0, 0)},
            {(DosColor.Red, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(252, 57, 31)},
            {(DosColor.Red, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(194, 54, 33)},
            {(DosColor.Red, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 85, 85)},
            {(DosColor.Red, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 0, 0)},
            {(DosColor.Red, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 0, 0)},
            {(DosColor.Red, Mode.Dark, TerminalType.xterm), Color.FromArgb(205, 0, 0)},
            {(DosColor.Red, Mode.Light, TerminalType.XWindow), Color.FromArgb(255, 0, 0)},
            {(DosColor.Red, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 0, 0)},

            {(DosColor.Green, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(0, 255, 0)},
            {(DosColor.Green, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 128, 0)},
            {(DosColor.Green, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(37, 188, 36)},
            {(DosColor.Green, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(37, 188, 36)},
            {(DosColor.Green, Mode.Light, TerminalType.PuTTY), Color.FromArgb(49, 231, 34)},
            {(DosColor.Green, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 187, 0)},
            {(DosColor.Green, Mode.Light, TerminalType.xterm), Color.FromArgb(0, 255, 0)},
            {(DosColor.Green, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 205, 0)},
            {(DosColor.Green, Mode.Light, TerminalType.XWindow), Color.FromArgb(144, 238, 144)},
            {(DosColor.Green, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 128, 0)},

            {(DosColor.Yellow, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 255, 0)},
            {(DosColor.Yellow, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(128, 128, 0)},
            {(DosColor.Yellow, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(234, 236, 35)},
            {(DosColor.Yellow, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(173, 173, 39)},
            {(DosColor.Yellow, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 255, 85)},
            {(DosColor.Yellow, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 187, 0)},
            {(DosColor.Yellow, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 255, 0)},
            {(DosColor.Yellow, Mode.Dark, TerminalType.xterm), Color.FromArgb(205, 205, 0)},
            {(DosColor.Yellow, Mode.Light, TerminalType.XWindow), Color.FromArgb(255, 255, 224)},
            {(DosColor.Yellow, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 255, 0)},

            {(DosColor.Blue, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(0, 0, 255)},
            {(DosColor.Blue, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 0, 128)},
            {(DosColor.Blue, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(88, 51, 255)},
            {(DosColor.Blue, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(73, 46, 225)},
            {(DosColor.Blue, Mode.Light, TerminalType.PuTTY), Color.FromArgb(85, 85, 255)},
            {(DosColor.Blue, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 0, 187)},
            {(DosColor.Blue, Mode.Light, TerminalType.xterm), Color.FromArgb(92, 92, 255)},
            {(DosColor.Blue, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 0, 238)},
            {(DosColor.Blue, Mode.Light, TerminalType.XWindow), Color.FromArgb(173, 216, 230)},
            {(DosColor.Blue, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 0, 255)},

            {(DosColor.Magenta, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 0, 255)},
            {(DosColor.Magenta, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(128, 0, 128)},
            {(DosColor.Magenta, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(249, 53, 248)},
            {(DosColor.Magenta, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(211, 56, 211)},
            {(DosColor.Magenta, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 85, 255)},
            {(DosColor.Magenta, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 0, 187)},
            {(DosColor.Magenta, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 0, 255)},
            {(DosColor.Magenta, Mode.Dark, TerminalType.xterm), Color.FromArgb(205, 0, 205)},
            {(DosColor.Magenta, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 0, 255)},

            {(DosColor.Cyan, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(0, 255, 255)},
            {(DosColor.Cyan, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 128, 128)},
            {(DosColor.Cyan, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(20, 240, 240)},
            {(DosColor.Cyan, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(51, 187, 200)},
            {(DosColor.Cyan, Mode.Light, TerminalType.PuTTY), Color.FromArgb(85, 255, 255)},
            {(DosColor.Cyan, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 187, 187)},
            {(DosColor.Cyan, Mode.Light, TerminalType.xterm), Color.FromArgb(0, 255, 255)},
            {(DosColor.Cyan, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 205, 205)},
            {(DosColor.Cyan, Mode.Light, TerminalType.XWindow), Color.FromArgb(224, 255, 255)},
            {(DosColor.Cyan, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 255, 255)},

            {(DosColor.White, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 255, 255)},
            {(DosColor.White, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(192, 192, 192)},
            {(DosColor.White, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(233, 235, 235)},
            {(DosColor.White, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(203, 204, 205)},
            {(DosColor.White, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 255, 255)},
            {(DosColor.White, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 187, 187)},
            {(DosColor.White, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 255, 255)},
            {(DosColor.White, Mode.Dark, TerminalType.xterm), Color.FromArgb(229, 229, 229)},
            {(DosColor.White, Mode.Light, TerminalType.XWindow), Color.FromArgb(255, 255, 255)},
            {(DosColor.White, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 255, 255)},
        };

        public static Color GetColor(DosColor color, Mode mode, TerminalType type)
        {
            return colors.TryGetValue((color, mode, type), out var result) ? result : Color.Empty;
        }
    }
}
