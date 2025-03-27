using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Mtf.Serial.Enums;

namespace Mtf.Serial.Services
{
    public static class TerminalEscapeCodeExecutor
    {
        public static void ExecuteAnsiEscapeCode(ref char[] chars, ref int index, TextBox textBox, TerminalType terminalType)
        {
            try
            {
                if (chars.Length == 0)
                {
                    return;
                }

                var position = 2;
                var primaryParam = ParseNumber(chars, ref position);
#if NET462_OR_GREATER
                var parameters = Array.Empty<int>();
#else
                var parameters = new int[] { };
#endif
                while (position < chars.Length)
                {
                    var operation = chars[index + position++];
                    ExecuteOperation(operation, ref index, primaryParam, ref parameters, textBox, terminalType, chars, ref position);
                }

                index += position;
            }
            catch (IndexOutOfRangeException)
            {
                index = chars.Length;
            }
            catch (Exception)
            {
                index++;
            }
        }

        private static int ParseNumber(char[] chars, ref int position)
        {
            var sb = new StringBuilder();
            while (position < chars.Length && Char.IsDigit(chars[position]))
            {
                _ = sb.Append(chars[position++]);
            }
            return int.TryParse(sb.ToString(), out var result) ? result : 0;
        }

        private static void ExecuteOperation(char operation, ref int index, int primaryParam, ref int[] parameters, TextBox textBox, TerminalType terminalType, char[] chars, ref int position)
        {
            switch (operation)
            {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                    MoveCursor(operation, primaryParam, textBox);
                    break;
                case 'J':
                    ClearScreen(primaryParam, textBox);
                    break;
                case 'K':
                    EraseLine(primaryParam, textBox);
                    break;
                case 'm':
                    SelectGraphicsRendition(textBox, terminalType, (GraphicsRendition)primaryParam);
                    break;
                case '?':
                    HandleCursorVisibility(chars, ref position);
                    break;
                case ';':
                    parameters = ParseMultipleParameters(chars, ref position);
                    foreach (var param in parameters)
                    {
                        SelectGraphicsRendition(param);
                    }
                    break;
                default:
                    break;
            }
        }

        private static void MoveCursor(char operation, int amount, TextBox textBox)
        {
            if (amount == 0)
            {
                amount = 1;
            }

            // TODO: Implement caret movement based on operation code; here are basic actions for each
            switch (operation)
            {
                case 'A': /* Move cursor up by `amount` lines */ break;
                case 'B': /* Move cursor down by `amount` lines */ break;
                case 'C': /* Move cursor forward by `amount` characters */ break;
                case 'D': /* Move cursor back by `amount` characters */ break;
                case 'E': /* Move to beginning of line `amount` lines down */ break;
                case 'F': /* Move to beginning of line `amount` lines up */ break;
                case 'G': /* Move to specified column `amount` */
                    textBox.SelectionStart = Math.Min(amount - 1, textBox.Text.Length);
                    break;
                default:
                    break;
            }
        }

        private static void ClearScreen(int type, TextBox textBox)
        {
            switch (type)
            {
                case 0: // Clear from cursor to end
                    textBox.Text = textBox.Text.Substring(0, textBox.SelectionStart);
                    break;
                case 1: // Clear from cursor to beginning
                    textBox.Text = textBox.Text.Substring(textBox.SelectionStart);
                    break;
                default:
                    textBox.Clear(); // Clear entire screen
                    break;
            }
        }

        private static void EraseLine(int type, TextBox textBox)
        {
            var selectionStart = textBox.SelectionStart;
            int newlineIndex;

            switch (type)
            {
                case 0: // Erase from cursor to end
                    newlineIndex = textBox.Text.IndexOf(Environment.NewLine, selectionStart);
                    textBox.Text = newlineIndex >= 0
                        ? textBox.Text.Substring(0, selectionStart) + textBox.Text.Substring(newlineIndex)
                        : textBox.Text.Substring(0, selectionStart);
                    break;
                case 1: // Erase from cursor to beginning
                    newlineIndex = textBox.Text.LastIndexOf(Environment.NewLine, selectionStart);
                    textBox.Text = newlineIndex >= 0
                        ? textBox.Text.Substring(0, newlineIndex) + textBox.Text.Substring(selectionStart)
                        : textBox.Text.Substring(selectionStart);
                    break;
                default:
                    textBox.Clear(); // Erase entire line
                    break;
            }
        }

        private static void HandleCursorVisibility(char[] chars, ref int position)
        {
            if (chars.Length > position + 2 && chars[position] == '2' && chars[position + 1] == '5')
            {
                position += 3; // Skip cursor visibility operation
            }
        }

        private static int[] ParseMultipleParameters(char[] chars, ref int position)
        {
            var parameters = new List<int>();
            while (position < chars.Length)
            {
                if (chars[position] == ';')
                {
                    position++;
                }

                parameters.Add(ParseNumber(chars, ref position));
            }
            return parameters.ToArray();
        }

        private static void SelectGraphicsRendition(int param)
        {
            // Implement graphics rendition selection based on ANSI code param
        }

        private static void SelectGraphicsRendition(TextBox textBox, TerminalType terminalType, GraphicsRendition rendition)
        {
            // Apply specific text style or formatting to `textBox` based on `rendition` and `terminalType`
        }
    }
}
