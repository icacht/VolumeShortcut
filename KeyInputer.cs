using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VolumeShortcut
{
    internal static class KeyInputer
    {
        private class API
        {
            internal const uint INPUT_KEYBOARD = 1;
            internal const uint MAPVK_VK_TO_VSC = 0;
            internal const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
            internal const uint KEYEVENTF_KEYUP = 0x0002;

            [StructLayout(LayoutKind.Sequential)]
            internal class MOUSEINPUT
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal class KEYBDINPUT
            {
                public ushort wVk;
                public ushort wScan;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal class HARDWAREINPUT
            {
                public uint uMsg;
                ushort wParamL;
                ushort wParamH;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal class INPUT
            {
                [StructLayout(LayoutKind.Explicit)]
                internal struct INPUTUnion
                {
                    [FieldOffset(0)] public MOUSEINPUT mi;
                    [FieldOffset(0)] public KEYBDINPUT ki;
                    [FieldOffset(0)] public HARDWAREINPUT hi;
                }

                public uint type;
                public INPUTUnion u; 
            }

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint SendInput(uint cInputs, INPUT pInputs, int cbSize);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern uint MapVirtualKey(uint uCode, uint uMapType);
            [DllImport("user32.dll")]
            internal static extern IntPtr GetMessageExtraInfo();
        }

        public static void KeyDownUp(uint keyCode, bool isExtend = false)
        {
            var input = new API.INPUT
            {
                type = API.INPUT_KEYBOARD
            };
            var extend_value = isExtend ? API.KEYEVENTF_EXTENDEDKEY : 0;
            input.u.ki = new API.KEYBDINPUT
            {
                wVk = (ushort)keyCode,
                wScan = (ushort)API.MapVirtualKey(keyCode, API.MAPVK_VK_TO_VSC),
                dwFlags = extend_value,
                time = 0,
                dwExtraInfo = API.GetMessageExtraInfo()
            };

            API.SendInput(1, input, Marshal.SizeOf(input));

            input.u.ki.dwFlags = extend_value | API.KEYEVENTF_KEYUP;
            API.SendInput(1, input, Marshal.SizeOf(input));
        }
    }
}