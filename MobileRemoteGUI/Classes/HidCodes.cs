using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace MobileSRC.MobileRemote.Classes
{
    static class HidCodes 
    {
        static Dictionary<HidKeys, int> hidCodes = new Dictionary<HidKeys, int>();
        static Dictionary<char, KeyValuePair<HidKeys, bool>> charCodes = new Dictionary<char, KeyValuePair<HidKeys, bool>>();

        public static HidKeys GetHidKey(char c)
        {
            if (charCodes.ContainsKey(c))
            {
                return charCodes[c].Key;
            }
            throw new InvalidOperationException("Key not found");
        }

        public static HidKeys GetHidKey(char c, out bool shiftDown)
        {
            if (charCodes.ContainsKey(c))
            {
                shiftDown = charCodes[c].Value;
                return charCodes[c].Key;
            }
            throw new InvalidOperationException("Key not found");
        }

        public static char GetHidChar(HidKeys key, bool shiftDown)
        {
            foreach (char c in charCodes.Keys)
            {
                if (charCodes[c].Key == key && charCodes[c].Value == shiftDown)
                {
                    return c;
                }
            }
            return (char)0;
        }

        public static int GetHidCode(Keys key)
        {
            return GetHidCode((HidKeys)key);
        }

        public static int GetHidCode(HidKeys key)
        {
            if (hidCodes.ContainsKey(key))
            {
                return hidCodes[key];
            }
            throw new InvalidOperationException("Key not found");
        }

        public static int GetHidCode(char c, out bool shift)
        {
            if (charCodes.ContainsKey(c))
            {
                shift = charCodes[c].Value;
                return GetHidCode(charCodes[c].Key);
            }
            throw new InvalidOperationException("Key not found");
        }

        static HidCodes()
        {
            // Add char to VK mappings
            charCodes.Add('a', new KeyValuePair<HidKeys, bool>(HidKeys.A, false));
            charCodes.Add('A', new KeyValuePair<HidKeys, bool>(HidKeys.A, true));
            charCodes.Add('b', new KeyValuePair<HidKeys, bool>(HidKeys.B, false));
            charCodes.Add('B', new KeyValuePair<HidKeys, bool>(HidKeys.B, true));
            charCodes.Add('c', new KeyValuePair<HidKeys, bool>(HidKeys.C, false));
            charCodes.Add('C', new KeyValuePair<HidKeys, bool>(HidKeys.C, true));
            charCodes.Add('d', new KeyValuePair<HidKeys, bool>(HidKeys.D, false));
            charCodes.Add('D', new KeyValuePair<HidKeys, bool>(HidKeys.D, true));
            charCodes.Add('e', new KeyValuePair<HidKeys, bool>(HidKeys.E, false));
            charCodes.Add('E', new KeyValuePair<HidKeys, bool>(HidKeys.E, true));
            charCodes.Add('f', new KeyValuePair<HidKeys, bool>(HidKeys.F, false));
            charCodes.Add('F', new KeyValuePair<HidKeys, bool>(HidKeys.F, true));
            charCodes.Add('g', new KeyValuePair<HidKeys, bool>(HidKeys.G, false));
            charCodes.Add('G', new KeyValuePair<HidKeys, bool>(HidKeys.G, true));
            charCodes.Add('h', new KeyValuePair<HidKeys, bool>(HidKeys.H, false));
            charCodes.Add('H', new KeyValuePair<HidKeys, bool>(HidKeys.H, true));
            charCodes.Add('i', new KeyValuePair<HidKeys, bool>(HidKeys.I, false));
            charCodes.Add('I', new KeyValuePair<HidKeys, bool>(HidKeys.I, true));
            charCodes.Add('j', new KeyValuePair<HidKeys, bool>(HidKeys.J, false));
            charCodes.Add('J', new KeyValuePair<HidKeys, bool>(HidKeys.J, true));
            charCodes.Add('k', new KeyValuePair<HidKeys, bool>(HidKeys.K, false));
            charCodes.Add('K', new KeyValuePair<HidKeys, bool>(HidKeys.K, true));
            charCodes.Add('l', new KeyValuePair<HidKeys, bool>(HidKeys.L, false));
            charCodes.Add('L', new KeyValuePair<HidKeys, bool>(HidKeys.L, true));
            charCodes.Add('m', new KeyValuePair<HidKeys, bool>(HidKeys.M, false));
            charCodes.Add('M', new KeyValuePair<HidKeys, bool>(HidKeys.M, true));
            charCodes.Add('n', new KeyValuePair<HidKeys, bool>(HidKeys.N, false));
            charCodes.Add('N', new KeyValuePair<HidKeys, bool>(HidKeys.N, true));
            charCodes.Add('o', new KeyValuePair<HidKeys, bool>(HidKeys.O, false));
            charCodes.Add('O', new KeyValuePair<HidKeys, bool>(HidKeys.O, true));
            charCodes.Add('p', new KeyValuePair<HidKeys, bool>(HidKeys.P, false));
            charCodes.Add('P', new KeyValuePair<HidKeys, bool>(HidKeys.P, true));
            charCodes.Add('q', new KeyValuePair<HidKeys, bool>(HidKeys.Q, false));
            charCodes.Add('Q', new KeyValuePair<HidKeys, bool>(HidKeys.Q, true));
            charCodes.Add('r', new KeyValuePair<HidKeys, bool>(HidKeys.R, false));
            charCodes.Add('R', new KeyValuePair<HidKeys, bool>(HidKeys.R, true));
            charCodes.Add('s', new KeyValuePair<HidKeys, bool>(HidKeys.S, false));
            charCodes.Add('S', new KeyValuePair<HidKeys, bool>(HidKeys.S, true));
            charCodes.Add('t', new KeyValuePair<HidKeys, bool>(HidKeys.T, false));
            charCodes.Add('T', new KeyValuePair<HidKeys, bool>(HidKeys.T, true));
            charCodes.Add('u', new KeyValuePair<HidKeys, bool>(HidKeys.U, false));
            charCodes.Add('U', new KeyValuePair<HidKeys, bool>(HidKeys.U, true));
            charCodes.Add('v', new KeyValuePair<HidKeys, bool>(HidKeys.V, false));
            charCodes.Add('V', new KeyValuePair<HidKeys, bool>(HidKeys.V, true));
            charCodes.Add('w', new KeyValuePair<HidKeys, bool>(HidKeys.W, false));
            charCodes.Add('W', new KeyValuePair<HidKeys, bool>(HidKeys.W, true));
            charCodes.Add('x', new KeyValuePair<HidKeys, bool>(HidKeys.X, false));
            charCodes.Add('X', new KeyValuePair<HidKeys, bool>(HidKeys.X, true));
            charCodes.Add('y', new KeyValuePair<HidKeys, bool>(HidKeys.Y, false));
            charCodes.Add('Y', new KeyValuePair<HidKeys, bool>(HidKeys.Y, true));
            charCodes.Add('z', new KeyValuePair<HidKeys, bool>(HidKeys.Z, false));
            charCodes.Add('Z', new KeyValuePair<HidKeys, bool>(HidKeys.Z, true));
            charCodes.Add('1', new KeyValuePair<HidKeys, bool>(HidKeys.D1, false));
            charCodes.Add('!', new KeyValuePair<HidKeys, bool>(HidKeys.D1, true));
            charCodes.Add('2', new KeyValuePair<HidKeys, bool>(HidKeys.D2, false));
            charCodes.Add('@', new KeyValuePair<HidKeys, bool>(HidKeys.D2, true));
            charCodes.Add('3', new KeyValuePair<HidKeys, bool>(HidKeys.D3, false));
            charCodes.Add('#', new KeyValuePair<HidKeys, bool>(HidKeys.D3, true));
            charCodes.Add('4', new KeyValuePair<HidKeys, bool>(HidKeys.D4, false));
            charCodes.Add('$', new KeyValuePair<HidKeys, bool>(HidKeys.D4, true));
            charCodes.Add('5', new KeyValuePair<HidKeys, bool>(HidKeys.D5, false));
            charCodes.Add('%', new KeyValuePair<HidKeys, bool>(HidKeys.D5, true));
            charCodes.Add('6', new KeyValuePair<HidKeys, bool>(HidKeys.D6, false));
            charCodes.Add('^', new KeyValuePair<HidKeys, bool>(HidKeys.D6, true));
            charCodes.Add('7', new KeyValuePair<HidKeys, bool>(HidKeys.D7, false));
            charCodes.Add('&', new KeyValuePair<HidKeys, bool>(HidKeys.D7, true));
            charCodes.Add('8', new KeyValuePair<HidKeys, bool>(HidKeys.D8, false));
            charCodes.Add('*', new KeyValuePair<HidKeys, bool>(HidKeys.D8, true));
            charCodes.Add('9', new KeyValuePair<HidKeys, bool>(HidKeys.D9, false));
            charCodes.Add('(', new KeyValuePair<HidKeys, bool>(HidKeys.D9, true));
            charCodes.Add('0', new KeyValuePair<HidKeys, bool>(HidKeys.D0, false));
            charCodes.Add(')', new KeyValuePair<HidKeys, bool>(HidKeys.D0, true));
            charCodes.Add('-', new KeyValuePair<HidKeys, bool>(HidKeys.Dash, false));
            charCodes.Add('_', new KeyValuePair<HidKeys, bool>(HidKeys.Dash, true));
            charCodes.Add('=', new KeyValuePair<HidKeys, bool>(HidKeys.Equal, false));
            charCodes.Add('+', new KeyValuePair<HidKeys, bool>(HidKeys.Equal, true));
            charCodes.Add('[', new KeyValuePair<HidKeys, bool>(HidKeys.LeftSquare, false));
            charCodes.Add('{', new KeyValuePair<HidKeys, bool>(HidKeys.LeftSquare, true));
            charCodes.Add(']', new KeyValuePair<HidKeys, bool>(HidKeys.RightSquare, false));
            charCodes.Add('}', new KeyValuePair<HidKeys, bool>(HidKeys.RightSquare, true));

            charCodes.Add('\\', new KeyValuePair<HidKeys, bool>(HidKeys.Pipe, false));
            charCodes.Add('|', new KeyValuePair<HidKeys, bool>(HidKeys.Pipe, true));
            charCodes.Add(';', new KeyValuePair<HidKeys, bool>(HidKeys.Colon, false));
            charCodes.Add(':', new KeyValuePair<HidKeys, bool>(HidKeys.Colon, true));
            charCodes.Add('\'', new KeyValuePair<HidKeys, bool>(HidKeys.Quotes, false));
            charCodes.Add('"', new KeyValuePair<HidKeys, bool>(HidKeys.Quotes, true));
            charCodes.Add(',', new KeyValuePair<HidKeys, bool>(HidKeys.Comma, false));
            charCodes.Add('<', new KeyValuePair<HidKeys, bool>(HidKeys.Comma, true));
            charCodes.Add('.', new KeyValuePair<HidKeys, bool>(HidKeys.Period, false));
            charCodes.Add('>', new KeyValuePair<HidKeys, bool>(HidKeys.Period, true));
            charCodes.Add('/', new KeyValuePair<HidKeys, bool>(HidKeys.QuestionMark, false));
            charCodes.Add('?', new KeyValuePair<HidKeys, bool>(HidKeys.QuestionMark, true));
            charCodes.Add('`', new KeyValuePair<HidKeys, bool>(HidKeys.Tilde, false));
            charCodes.Add('~', new KeyValuePair<HidKeys, bool>(HidKeys.Tilde, true));
            charCodes.Add('\r', new KeyValuePair<HidKeys, bool>(HidKeys.Enter, false));
            charCodes.Add('\b', new KeyValuePair<HidKeys, bool>(HidKeys.Back, false));
            charCodes.Add('\t', new KeyValuePair<HidKeys, bool>(HidKeys.Tab, false));
            charCodes.Add(' ', new KeyValuePair<HidKeys, bool>(HidKeys.Space, false));
            charCodes.Add((char)0x01, new KeyValuePair<HidKeys, bool>(HidKeys.Escape, false));

            // Add VK to HID mappings
            hidCodes.Add(HidKeys.A, 0x4);
            hidCodes.Add(HidKeys.B, 0x5);
            hidCodes.Add(HidKeys.C, 0x6);
            hidCodes.Add(HidKeys.D, 0x7);
            hidCodes.Add(HidKeys.E, 0x8);
            hidCodes.Add(HidKeys.F, 0x9);
            hidCodes.Add(HidKeys.G, 0x0A);
            hidCodes.Add(HidKeys.H, 0x0B);
            hidCodes.Add(HidKeys.I, 0x0C);
            hidCodes.Add(HidKeys.J, 0x0D);
            hidCodes.Add(HidKeys.K, 0x0E);
            hidCodes.Add(HidKeys.L, 0x0F);
            hidCodes.Add(HidKeys.M, 0x10);
            hidCodes.Add(HidKeys.N, 0x11);
            hidCodes.Add(HidKeys.O, 0x12);
            hidCodes.Add(HidKeys.P, 0x13);
            hidCodes.Add(HidKeys.Q, 0x14);
            hidCodes.Add(HidKeys.R, 0x15);
            hidCodes.Add(HidKeys.S, 0x16);
            hidCodes.Add(HidKeys.T, 0x17);
            hidCodes.Add(HidKeys.U, 0x18);
            hidCodes.Add(HidKeys.V, 0x19);
            hidCodes.Add(HidKeys.W, 0x1A);
            hidCodes.Add(HidKeys.X, 0x1B);
            hidCodes.Add(HidKeys.Y, 0x1C);
            hidCodes.Add(HidKeys.Z, 0x1D);
            hidCodes.Add(HidKeys.D1, 0x1E);
            hidCodes.Add(HidKeys.D2, 0x1F);
            hidCodes.Add(HidKeys.D3, 0x20);
            hidCodes.Add(HidKeys.D4, 0x21);
            hidCodes.Add(HidKeys.D5, 0x22);
            hidCodes.Add(HidKeys.D6, 0x23);
            hidCodes.Add(HidKeys.D7, 0x24);
            hidCodes.Add(HidKeys.D8, 0x25);
            hidCodes.Add(HidKeys.D9, 0x26);
            hidCodes.Add(HidKeys.D0, 0x27);
            hidCodes.Add(HidKeys.Enter, 0x28);
            hidCodes.Add(HidKeys.Escape, 0x29);
            hidCodes.Add(HidKeys.Back, 0x2A);
            hidCodes.Add(HidKeys.Tab, 0x2B);
            hidCodes.Add(HidKeys.Space, 0x2C);
            hidCodes.Add(HidKeys.Dash, 0x2D);
            hidCodes.Add(HidKeys.Equal, 0x2E);
            hidCodes.Add(HidKeys.LeftSquare, 0x2F);
            hidCodes.Add(HidKeys.RightSquare, 0x30);
            
            hidCodes.Add(HidKeys.Pipe, 0x31);
            hidCodes.Add(HidKeys.Colon, 0x33);
            hidCodes.Add(HidKeys.Quotes, 0x34);
            hidCodes.Add(HidKeys.Tilde, 0x35);
            hidCodes.Add(HidKeys.Comma, 0x36);
            hidCodes.Add(HidKeys.Period, 0x37);
            hidCodes.Add(HidKeys.QuestionMark, 0x38);

            hidCodes.Add(HidKeys.CapsLock, 0x39);

            hidCodes.Add(HidKeys.F1, 0x3A);
            hidCodes.Add(HidKeys.F2, 0x3B);
            hidCodes.Add(HidKeys.F3, 0x3C);
            hidCodes.Add(HidKeys.F4, 0x3D);
            hidCodes.Add(HidKeys.F5, 0x3E);
            hidCodes.Add(HidKeys.F6, 0x3F);
            hidCodes.Add(HidKeys.F7, 0x40);
            hidCodes.Add(HidKeys.F8, 0x41);
            hidCodes.Add(HidKeys.F9, 0x42);
            hidCodes.Add(HidKeys.F10, 0x43);
            hidCodes.Add(HidKeys.F11, 0x44);
            hidCodes.Add(HidKeys.F12, 0x45);

            hidCodes.Add(HidKeys.PrintScreen, 0x46);
            hidCodes.Add(HidKeys.Scroll, 0x47);

            // 48 = CTRL+BRK

            hidCodes.Add(HidKeys.Insert, 0x49);
            hidCodes.Add(HidKeys.Home, 0x4A);
            hidCodes.Add(HidKeys.PageUp, 0x4B);
            hidCodes.Add(HidKeys.Delete, 0x4C);
            hidCodes.Add(HidKeys.End, 0x4D);
            hidCodes.Add(HidKeys.PageDown, 0x4E);
            hidCodes.Add(HidKeys.Right, 0x4F);
            hidCodes.Add(HidKeys.Left, 0x50);
            hidCodes.Add(HidKeys.Down, 0x51);
            hidCodes.Add(HidKeys.Up, 0x52);
            hidCodes.Add(HidKeys.NumLock, 0x53);
            hidCodes.Add(HidKeys.Divide, 0x54);
            hidCodes.Add(HidKeys.Multiply, 0x55);
            hidCodes.Add(HidKeys.Subtract, 0x56);
            hidCodes.Add(HidKeys.Add, 0x57);

            //hidCodes.Add(HidKeys.Enter, 0x58);

            hidCodes.Add(HidKeys.NumPad1, 0x59);
            hidCodes.Add(HidKeys.NumPad2, 0x5A);
            hidCodes.Add(HidKeys.NumPad3, 0x5B);
            hidCodes.Add(HidKeys.NumPad4, 0x5C);
            hidCodes.Add(HidKeys.NumPad5, 0x5D);
            hidCodes.Add(HidKeys.NumPad6, 0x5E);
            hidCodes.Add(HidKeys.NumPad7, 0x5F);
            hidCodes.Add(HidKeys.NumPad8, 0x60);
            hidCodes.Add(HidKeys.NumPad9, 0x61);
            hidCodes.Add(HidKeys.NumPad0, 0x62);
            hidCodes.Add(HidKeys.Decimal, 0x63);
            /*
hidCodes.Add(HidKeys.Europe, 0x64);
hidCodes.Add(HidKeys.App, 0x65);
hidCodes.Add(HidKeys.Power, 0x66);
hidCodes.Add(HidKeys.Equal, 0x67);
             */
            hidCodes.Add(HidKeys.F13, 0x68);
            hidCodes.Add(HidKeys.F14, 0x69);
            hidCodes.Add(HidKeys.F15, 0x6A);
            hidCodes.Add(HidKeys.F16, 0x6B);
            hidCodes.Add(HidKeys.F17, 0x6C);
            hidCodes.Add(HidKeys.F18, 0x6D);
            hidCodes.Add(HidKeys.F19, 0x6E);
            hidCodes.Add(HidKeys.F20, 0x6F);
            hidCodes.Add(HidKeys.F21, 0x70);
            hidCodes.Add(HidKeys.F22, 0x71);
            hidCodes.Add(HidKeys.F23, 0x72);
            hidCodes.Add(HidKeys.F24, 0x73);
            hidCodes.Add(HidKeys.Execute, 0x74);
            hidCodes.Add(HidKeys.Help, 0x75);
            hidCodes.Add(HidKeys.Menu, 0x76);
            hidCodes.Add(HidKeys.Select, 0x77);
            //hidCodes.Add(HidKeys.MediaStop, 0x78);
            //hidCodes.Add(HidKeys.Again, 0x79);
            //hidCodes.Add(HidKeys.Undo, 0x7A);
            //hidCodes.Add(HidKeys.Cut, 0x7B);
            //hidCodes.Add(HidKeys.Copy, 0x7C);
            //hidCodes.Add(HidKeys.Paste, 0x7D);
            //hidCodes.Add(HidKeys.Find, 0x7E);
            hidCodes.Add(HidKeys.VolumeMute, 0x7F);
            hidCodes.Add(HidKeys.VolumeUp, 0x80);
            hidCodes.Add(HidKeys.VolumeDown, 0x81);

            hidCodes.Add(HidKeys.PS3_EJECT, 0x16);
            hidCodes.Add(HidKeys.PS3_AUDIO, 0x64);
            hidCodes.Add(HidKeys.PS3_ANGLE, 0x65);
            hidCodes.Add(HidKeys.PS3_SUBTITLE, 0x63);
            hidCodes.Add(HidKeys.PS3_CLEAR, 0x0f);
            hidCodes.Add(HidKeys.PS3_TIME, 0x28);
            hidCodes.Add(HidKeys.PS3_1, 0x00);
            hidCodes.Add(HidKeys.PS3_2, 0x01);
            hidCodes.Add(HidKeys.PS3_3, 0x02);
            hidCodes.Add(HidKeys.PS3_4, 0x03);
            hidCodes.Add(HidKeys.PS3_5, 0x04);
            hidCodes.Add(HidKeys.PS3_6, 0x05);
            hidCodes.Add(HidKeys.PS3_7, 0x06);
            hidCodes.Add(HidKeys.PS3_8, 0x07);
            hidCodes.Add(HidKeys.PS3_9, 0x08);
            hidCodes.Add(HidKeys.PS3_0, 0x09);
            hidCodes.Add(HidKeys.PS3_RED, 0x81);
            hidCodes.Add(HidKeys.PS3_GREEN, 0x82);
            hidCodes.Add(HidKeys.PS3_BLUE, 0x80);
            hidCodes.Add(HidKeys.PS3_YELLOW, 0x83);
            hidCodes.Add(HidKeys.PS3_DISPLAY, 0x70);
            hidCodes.Add(HidKeys.PS3_TOPMENU, 0x1a);
            hidCodes.Add(HidKeys.PS3_POPUPMENU, 0x40);
            hidCodes.Add(HidKeys.PS3_RETURN, 0x0e);
            hidCodes.Add(HidKeys.PS3_TRIANGLE, 0x5c);
            hidCodes.Add(HidKeys.PS3_CIRCLE, 0x5d);
            hidCodes.Add(HidKeys.PS3_X, 0x5e);
            hidCodes.Add(HidKeys.PS3_SQUARE, 0x5f);
            hidCodes.Add(HidKeys.PS3_UP, 0x54);
            hidCodes.Add(HidKeys.PS3_RIGHT, 0x55);
            hidCodes.Add(HidKeys.PS3_DOWN, 0x56);
            hidCodes.Add(HidKeys.PS3_LEFT, 0x57);
            hidCodes.Add(HidKeys.PS3_ENTER, 0x0b);
            hidCodes.Add(HidKeys.PS3_L1, 0x5a);
            hidCodes.Add(HidKeys.PS3_L2, 0x58);
            hidCodes.Add(HidKeys.PS3_L3, 0x51);
            hidCodes.Add(HidKeys.PS3_R1, 0x5b);
            hidCodes.Add(HidKeys.PS3_R2, 0x59);
            hidCodes.Add(HidKeys.PS3_R3, 0x52);
            hidCodes.Add(HidKeys.PS3_PLAYSTATION, 0x43);
            hidCodes.Add(HidKeys.PS3_SELECT, 0x50);
            hidCodes.Add(HidKeys.PS3_START, 0x53);
            hidCodes.Add(HidKeys.PS3_SCANBACK, 0x33);
            hidCodes.Add(HidKeys.PS3_SCANFORWARD, 0x34);
            hidCodes.Add(HidKeys.PS3_PREV, 0x30);
            hidCodes.Add(HidKeys.PS3_NEXT, 0x31);
            hidCodes.Add(HidKeys.PS3_STEPBACK, 0x60);
            hidCodes.Add(HidKeys.PS3_STEPFORWARD, 0x61);
            hidCodes.Add(HidKeys.PS3_PLAY, 0x32);
            hidCodes.Add(HidKeys.PS3_STOP, 0x38);
            hidCodes.Add(HidKeys.PS3_PAUSE, 0x39);
        }
    }
    // Summary:
    //     Specifies key codes and modifiers.
    [Flags]
    public enum HidKeys
    {
        // Summary:
        //     The bitmask to extract modifiers from a key value.
        Modifiers = -65536,
        //
        // Summary:
        //     No key pressed.
        None = 0,
        //
        // Summary:
        //     The left mouse button.
        LButton = 1,
        //
        // Summary:
        //     The right mouse button.
        RButton = 2,
        //
        // Summary:
        //     The CANCEL key.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        MButton = 4,
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        XButton1 = 5,
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        XButton2 = 6,
        //
        // Summary:
        //     The BACKSPACE key.
        Back = 8,
        //
        // Summary:
        //     The TAB key.
        Tab = 9,
        //
        // Summary:
        //     The LINEFEED key.
        LineFeed = 10,
        //
        // Summary:
        //     The CLEAR key.
        Clear = 12,
        //
        // Summary:
        //     The ENTER key.
        Enter = 13,
        //
        // Summary:
        //     The SHIFT key.
        ShiftKey = 16,
        //
        // Summary:
        //     The CTRL key.
        ControlKey = 17,
        //
        // Summary:
        //     The ALT key.
        Menu = 18,
        //
        // Summary:
        //     The PAUSE key.
        Pause = 19,
        //
        // Summary:
        //     The CAPS LOCK key.
        CapsLock = 20,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = 21,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = 23,
        //
        // Summary:
        //     The IME final mode key.
        FinalMode = 24,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = 25,
        //
        // Summary:
        //     The ESC key.
        Escape = 27,
        //
        // Summary:
        //     The IME convert key.
        IMEConvert = 28,
        //
        // Summary:
        //     The IME nonconvert key.
        IMENonconvert = 29,
        //
        // Summary:
        //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Summary:
        //     The IME mode change key.
        IMEModeChange = 31,
        //
        // Summary:
        //     The SPACEBAR key.
        Space = 32,
        //
        // Summary:
        //     The PAGE UP key.
        PageUp = 33,
        //
        // Summary:
        //     The PAGE DOWN key.
        PageDown = 34,
        //
        // Summary:
        //     The END key.
        End = 35,
        //
        // Summary:
        //     The HOME key.
        Home = 36,
        //
        // Summary:
        //     The LEFT ARROW key.
        Left = 37,
        //
        // Summary:
        //     The UP ARROW key.
        Up = 38,
        //
        // Summary:
        //     The RIGHT ARROW key.
        Right = 39,
        //
        // Summary:
        //     The DOWN ARROW key.
        Down = 40,
        //
        // Summary:
        //     The SELECT key.
        Select = 41,
        //
        // Summary:
        //     The PRINT key.
        Print = 42,
        //
        // Summary:
        //     The EXECUTE key.
        Execute = 43,
        //
        // Summary:
        //     The PRINT SCREEN key.
        PrintScreen = 44,
        //
        // Summary:
        //     The INS key.
        Insert = 45,
        //
        // Summary:
        //     The DEL key.
        Delete = 46,
        //
        // Summary:
        //     The HELP key.
        Help = 47,
        //
        // Summary:
        //     The 0 key.
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        D1 = 49,
        //
        // Summary:
        //     The 2 key.
        D2 = 50,
        //
        // Summary:
        //     The 3 key.
        D3 = 51,
        //
        // Summary:
        //     The 4 key.
        D4 = 52,
        //
        // Summary:
        //     The 5 key.
        D5 = 53,
        //
        // Summary:
        //     The 6 key.
        D6 = 54,
        //
        // Summary:
        //     The 7 key.
        D7 = 55,
        //
        // Summary:
        //     The 8 key.
        D8 = 56,
        //
        // Summary:
        //     The 9 key.
        D9 = 57,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWin = 91,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWin = 92,
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        Apps = 93,
        //
        // Summary:
        //     The computer sleep key.
        Sleep = 95,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //
        // Summary:
        //     The multiply key.
        Multiply = 106,
        //
        // Summary:
        //     The add key.
        Add = 107,
        //
        // Summary:
        //     The separator key.
        Separator = 108,
        //
        // Summary:
        //     The subtract key.
        Subtract = 109,
        //
        // Summary:
        //     The decimal key.
        Decimal = 110,
        //
        // Summary:
        //     The divide key.
        Divide = 111,
        //
        // Summary:
        //     The F1 key.
        F1 = 112,
        //
        // Summary:
        //     The F2 key.
        F2 = 113,
        //
        // Summary:
        //     The F3 key.
        F3 = 114,
        //
        // Summary:
        //     The F4 key.
        F4 = 115,
        //
        // Summary:
        //     The F5 key.
        F5 = 116,
        //
        // Summary:
        //     The F6 key.
        F6 = 117,
        //
        // Summary:
        //     The F7 key.
        F7 = 118,
        //
        // Summary:
        //     The F8 key.
        F8 = 119,
        //
        // Summary:
        //     The F9 key.
        F9 = 120,
        //
        // Summary:
        //     The F10 key.
        F10 = 121,
        //
        // Summary:
        //     The F11 key.
        F11 = 122,
        //
        // Summary:
        //     The F12 key.
        F12 = 123,
        //
        // Summary:
        //     The F13 key.
        F13 = 124,
        //
        // Summary:
        //     The F14 key.
        F14 = 125,
        //
        // Summary:
        //     The F15 key.
        F15 = 126,
        //
        // Summary:
        //     The F16 key.
        F16 = 127,
        //
        // Summary:
        //     The F17 key.
        F17 = 128,
        //
        // Summary:
        //     The F18 key.
        F18 = 129,
        //
        // Summary:
        //     The F19 key.
        F19 = 130,
        //
        // Summary:
        //     The F20 key.
        F20 = 131,
        //
        // Summary:
        //     The F21 key.
        F21 = 132,
        //
        // Summary:
        //     The F22 key.
        F22 = 133,
        //
        // Summary:
        //     The F23 key.
        F23 = 134,
        //
        // Summary:
        //     The F24 key.
        F24 = 135,
        //
        // Summary:
        //     The NUM LOCK key.
        NumLock = 144,
        //
        // Summary:
        //     The SCROLL LOCK key.
        Scroll = 145,
        //
        // Summary:
        //     The left SHIFT key.
        LShiftKey = 160,
        //
        // Summary:
        //     The right SHIFT key.
        RShiftKey = 161,
        //
        // Summary:
        //     The left CTRL key.
        LControlKey = 162,
        //
        // Summary:
        //     The right CTRL key.
        RControlKey = 163,
        //
        // Summary:
        //     The left ALT key.
        LMenu = 164,
        //
        // Summary:
        //     The right ALT key.
        RMenu = 165,
        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        MediaNextTrack = 176,
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        LaunchApplication1 = 182,
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        LaunchApplication2 = 183,
        //
        // Summary:
        //     The PROCESS KEY key.
        ProcessKey = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key
        //     value is the low word of a 32-bit virtual-key value used for non-keyboard
        //     input methods.
        Packet = 231,
        //
        // Summary:
        //     The ATTN key.
        Attn = 246,
        //
        // Summary:
        //     The CRSEL key.
        Crsel = 247,
        //
        // Summary:
        //     The EXSEL key.
        Exsel = 248,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = 249,
        //
        // Summary:
        //     The PLAY key.
        Play = 250,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = 251,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = 252,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = 253,
        //
        // Summary:
        //     The CLEAR key.
        OemClear = 254,
        //
        // Summary:
        //     The bitmask to extract a key code from a key value.
        KeyCode = 65535,
        //
        // Summary:
        //     The SHIFT modifier key.
        Shift = 65536,
        //
        // Summary:
        //     The CTRL modifier key.
        Control = 131072,
        //
        // Summary:
        //     The ALT modifier key.
        Alt = 262144,

        Equal = 187,
        Dash = 189,
        LeftSquare = 219,
        RightSquare = 221,
        Pipe = 0xdc,
        Colon = 0xba,
        Quotes = 0xde,
        Comma = 0xbc,
        Period = 0xbe,
        QuestionMark = 0xbf,
        Tilde = 0xc0,

        PS3_EJECT = 255,
        PS3_AUDIO = 256,
        PS3_ANGLE = 257,
        PS3_SUBTITLE = 258,
        PS3_CLEAR = 259,
        PS3_TIME = 260,
        PS3_1 = 261,
        PS3_2 = 262,
        PS3_3 = 263,
        PS3_4 = 264,
        PS3_5 = 265,
        PS3_6 = 266,
        PS3_7 = 267,
        PS3_8 = 268,
        PS3_9 = 269,
        PS3_0 = 270,
        PS3_RED = 271,
        PS3_GREEN = 272,
        PS3_BLUE = 273,
        PS3_YELLOW = 274,
        PS3_DISPLAY = 275,
        PS3_TOPMENU = 276,
        PS3_POPUPMENU = 277,
        PS3_RETURN = 278,
        PS3_TRIANGLE = 279,
        PS3_CIRCLE = 280,
        PS3_X = 281,
        PS3_SQUARE = 282,
        PS3_UP = 283,
        PS3_RIGHT = 284,
        PS3_DOWN = 285,
        PS3_LEFT = 286,
        PS3_ENTER = 287,
        PS3_L1 = 288,
        PS3_L2 = 289,
        PS3_L3 = 290,
        PS3_R1 = 291,
        PS3_R2 = 292,
        PS3_R3 = 293,
        PS3_PLAYSTATION = 294,
        PS3_SELECT = 295,
        PS3_START = 296,
        PS3_SCANBACK = 297,
        PS3_SCANFORWARD = 298,
        PS3_PREV = 299,
        PS3_NEXT = 300,
        PS3_STEPBACK = 301,
        PS3_STEPFORWARD = 302,
        PS3_PLAY = 303,
        PS3_STOP = 304,
        PS3_PAUSE = 305,
    }
}