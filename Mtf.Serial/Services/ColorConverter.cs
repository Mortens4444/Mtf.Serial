using Mtf.Serial.Enums;
using Mtf.Serial.Models;
using System.Collections.Generic;
using System.Drawing;

namespace Mtf.Serial.Services
{
    public static class ColorConverter
    {
        private static readonly Dictionary<TerminalColorMode, Color> colors = new Dictionary<TerminalColorMode, Color>
        {
            { new TerminalColorMode(DosColor.Black, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(128, 128, 128)},
            { new TerminalColorMode(DosColor.Black, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 0, 0)},
            { new TerminalColorMode(DosColor.Black, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(129, 131, 131)},
            { new TerminalColorMode(DosColor.Black, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(0, 0, 0)},
            { new TerminalColorMode(DosColor.Black, Mode.Light, TerminalType.PuTTY), Color.FromArgb(85, 85, 85)},
            { new TerminalColorMode(DosColor.Black, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 0, 0)},
            { new TerminalColorMode(DosColor.Black, Mode.Light, TerminalType.xterm), Color.FromArgb(127, 127, 127)},
            { new TerminalColorMode(DosColor.Black, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 0, 0)},
            { new TerminalColorMode(DosColor.Black, Mode.Light, TerminalType.XWindow), Color.FromArgb(127, 127, 127)},
            { new TerminalColorMode(DosColor.Black, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 0, 0)},

            { new TerminalColorMode(DosColor.Red, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 0, 0)},
            { new TerminalColorMode(DosColor.Red, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(128, 0, 0)},
            { new TerminalColorMode(DosColor.Red, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(252, 57, 31)},
            { new TerminalColorMode(DosColor.Red, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(194, 54, 33)},
            { new TerminalColorMode(DosColor.Red, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 85, 85)},
            { new TerminalColorMode(DosColor.Red, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 0, 0)},
            { new TerminalColorMode(DosColor.Red, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 0, 0)},
            { new TerminalColorMode(DosColor.Red, Mode.Dark, TerminalType.xterm), Color.FromArgb(205, 0, 0)},
            { new TerminalColorMode(DosColor.Red, Mode.Light, TerminalType.XWindow), Color.FromArgb(255, 0, 0)},
            { new TerminalColorMode(DosColor.Red, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 0, 0)},

            { new TerminalColorMode(DosColor.Green, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(0, 255, 0)},
            { new TerminalColorMode(DosColor.Green, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 128, 0)},
            { new TerminalColorMode(DosColor.Green, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(37, 188, 36)},
            { new TerminalColorMode(DosColor.Green, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(37, 188, 36)},
            { new TerminalColorMode(DosColor.Green, Mode.Light, TerminalType.PuTTY), Color.FromArgb(49, 231, 34)},
            { new TerminalColorMode(DosColor.Green, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 187, 0)},
            { new TerminalColorMode(DosColor.Green, Mode.Light, TerminalType.xterm), Color.FromArgb(0, 255, 0)},
            { new TerminalColorMode(DosColor.Green, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 205, 0)},
            { new TerminalColorMode(DosColor.Green, Mode.Light, TerminalType.XWindow), Color.FromArgb(144, 238, 144)},
            { new TerminalColorMode(DosColor.Green, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 128, 0)},

            { new TerminalColorMode(DosColor.Yellow, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 255, 0)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(128, 128, 0)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(234, 236, 35)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(173, 173, 39)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 255, 85)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 187, 0)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 255, 0)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Dark, TerminalType.xterm), Color.FromArgb(205, 205, 0)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Light, TerminalType.XWindow), Color.FromArgb(255, 255, 224)},
            { new TerminalColorMode(DosColor.Yellow, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 255, 0)},

            { new TerminalColorMode(DosColor.Blue, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(0, 0, 255)},
            { new TerminalColorMode(DosColor.Blue, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 0, 128)},
            { new TerminalColorMode(DosColor.Blue, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(88, 51, 255)},
            { new TerminalColorMode(DosColor.Blue, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(73, 46, 225)},
            { new TerminalColorMode(DosColor.Blue, Mode.Light, TerminalType.PuTTY), Color.FromArgb(85, 85, 255)},
            { new TerminalColorMode(DosColor.Blue, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 0, 187)},
            { new TerminalColorMode(DosColor.Blue, Mode.Light, TerminalType.xterm), Color.FromArgb(92, 92, 255)},
            { new TerminalColorMode(DosColor.Blue, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 0, 238)},
            { new TerminalColorMode(DosColor.Blue, Mode.Light, TerminalType.XWindow), Color.FromArgb(173, 216, 230)},
            { new TerminalColorMode(DosColor.Blue, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 0, 255)},

            { new TerminalColorMode(DosColor.Magenta, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 0, 255)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(128, 0, 128)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(249, 53, 248)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(211, 56, 211)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 85, 255)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 0, 187)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 0, 255)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Dark, TerminalType.xterm), Color.FromArgb(205, 0, 205)},
            { new TerminalColorMode(DosColor.Magenta, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 0, 255)},

            { new TerminalColorMode(DosColor.Cyan, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(0, 255, 255)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(0, 128, 128)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(20, 240, 240)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(51, 187, 200)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Light, TerminalType.PuTTY), Color.FromArgb(85, 255, 255)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(0, 187, 187)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Light, TerminalType.xterm), Color.FromArgb(0, 255, 255)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Dark, TerminalType.xterm), Color.FromArgb(0, 205, 205)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Light, TerminalType.XWindow), Color.FromArgb(224, 255, 255)},
            { new TerminalColorMode(DosColor.Cyan, Mode.Dark, TerminalType.XWindow), Color.FromArgb(0, 255, 255)},

            { new TerminalColorMode(DosColor.White, Mode.Light, TerminalType.WindowsXP), Color.FromArgb(255, 255, 255)},
            { new TerminalColorMode(DosColor.White, Mode.Dark, TerminalType.WindowsXP), Color.FromArgb(192, 192, 192)},
            { new TerminalColorMode(DosColor.White, Mode.Light, TerminalType.TerminalApp), Color.FromArgb(233, 235, 235)},
            { new TerminalColorMode(DosColor.White, Mode.Dark, TerminalType.TerminalApp), Color.FromArgb(203, 204, 205)},
            { new TerminalColorMode(DosColor.White, Mode.Light, TerminalType.PuTTY), Color.FromArgb(255, 255, 255)},
            { new TerminalColorMode(DosColor.White, Mode.Dark, TerminalType.PuTTY), Color.FromArgb(187, 187, 187)},
            { new TerminalColorMode(DosColor.White, Mode.Light, TerminalType.xterm), Color.FromArgb(255, 255, 255)},
            { new TerminalColorMode(DosColor.White, Mode.Dark, TerminalType.xterm), Color.FromArgb(229, 229, 229)},
            { new TerminalColorMode(DosColor.White, Mode.Light, TerminalType.XWindow), Color.FromArgb(255, 255, 255)},
            { new TerminalColorMode(DosColor.White, Mode.Dark, TerminalType.XWindow), Color.FromArgb(255, 255, 255)},
        };

        public static Color GetColor(DosColor color, Mode mode, TerminalType type)
        {
            return colors.TryGetValue(new TerminalColorMode(color, mode, type), out var result) ? result : Color.Empty;
        }
    }
}
