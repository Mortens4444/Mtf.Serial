using Mtf.Serial.Enums;

namespace Mtf.Serial.Models
{
    public class TerminalColorMode
    {
        public DosColor DosColor { get; private set; }
        
        public Mode Mode { get; private set; }
        
        public TerminalType TerminalType { get; private set; }

        public TerminalColorMode(DosColor dosColor, Mode mode, TerminalType terminalType)
        {
            DosColor = dosColor;
            Mode = mode;
            TerminalType = terminalType;
        }
    }
}
