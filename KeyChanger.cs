using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace VolumeShortcut
{
    // (keycode, shift, ctrl, alt)
    using KeyCombination = System.ValueTuple<int, bool, bool, bool>;

    internal class KeyChanger
    {
        private class API
        {
            internal const int VK_SHIFT = 0x0010;
            internal const int VK_CONTROL = 0x0011;

            [DllImport("user32.dll")]
            internal static extern short GetAsyncKeyState(int vKey);
        }

        private Dictionary<KeyCombination, int> KeyCombinationDict;

        public KeyChanger()
        {
            KeyCombinationDict = new Dictionary<KeyCombination, int>()
            {
                { (KeyInterop.VirtualKeyFromKey(Key.F8), true, true, false), KeyInterop.VirtualKeyFromKey(Key.VolumeUp) },
                { (KeyInterop.VirtualKeyFromKey(Key.F7), true, true, false), KeyInterop.VirtualKeyFromKey(Key.VolumeDown) },
            };
        }

        internal void KeyDownEventProcedure(object obj, KeyHooker.KeyEventArgs e)
        {
            if (e.IsVirtualInput)
            {
                return;
            }

            bool isShiftDown = API.GetAsyncKeyState(API.VK_SHIFT) < 0;
            bool isCtrlDown = API.GetAsyncKeyState(API.VK_CONTROL) < 0;

            var inputCode = KeyCombinationDict.GetValueOrDefault(
                ((int)e.KeyCode, isShiftDown, isCtrlDown, e.IsALTDown), 0
            );

            if (inputCode == 0)
            {
                return;
            }

            KeyInputer.KeyDownUp((uint)inputCode);
            e.IsPrevent = true;
        }

    }
}