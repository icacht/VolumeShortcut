using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VolumeShortcut
{
    internal class KeyHooker : IDisposable
    {
        private class API
        {
            internal const int WH_KEYBOARD_LL = 0x000D;

            internal const int WM_KEYDOWN = 0x0100;
            internal const int WM_KEYUP = 0x0101;
            internal const int WM_SYSKEYDOWN = 0x0104;
            internal const int WM_SYSKEYUP = 0x0105;
         
            internal delegate IntPtr HOOKPROC(int code, IntPtr wParam, IntPtr lParam);

            [StructLayout(LayoutKind.Sequential)]
            internal class KBDLLHOOKSTRUCT
            {
                public uint vkCode;
                public uint scanCode;
                public uint flags;
                public uint time;
                public IntPtr dwExtraInfo;
            }
         
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hmod, uint dwThreadId);
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool UnhookWindowsHookEx(IntPtr hhk);
            [DllImport("user32.dll")]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);
        }

        public class KeyEventArgs : EventArgs
        {
            public uint KeyCode { get; }
            public bool IsVirtualInput { get; }
            public bool IsALTDown { get; }
            public bool IsPrevent { get; set; } = false;

            public KeyEventArgs(uint keyCode, bool isVirtualInput, bool isALTDown)
            {
                KeyCode = keyCode;
                IsVirtualInput = isVirtualInput;
                IsALTDown = isALTDown;
            }
        }

        public event EventHandler<KeyEventArgs> KeyUpEvent;
        public event EventHandler<KeyEventArgs> KeyDownEvent;

        private API.HOOKPROC hookProc;
        private IntPtr hookId = IntPtr.Zero;
        private bool disposed = false;

        public void Hook()
        {
            if(hookId != IntPtr.Zero)
            {
                return;
            }

            hookProc = HookProcedure;
            using var process = Process.GetCurrentProcess();
            using var module = process.MainModule;
            hookId = API.SetWindowsHookEx(API.WH_KEYBOARD_LL, hookProc, API.GetModuleHandle(module.ModuleName), 0);
        }

        public void UnHook()
        {
            if(hookId == IntPtr.Zero)
            {
                return;
            }

            API.UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
            {
                return;
            }
            UnHook();
            disposed = true;
        }

        private bool KeyboardProcedure(int message, API.KBDLLHOOKSTRUCT param)
        {
            var isInjected = ((param.flags & (1<<1)) | (param.flags & (1<<4))) != 0;
            var isALTDown = (param.flags & (1<<5)) != 0;
            var args = new KeyEventArgs(param.vkCode, isInjected, isALTDown);
            if (message == API.WM_KEYDOWN || message == API.WM_SYSKEYDOWN)
            {
                KeyDownEvent?.Invoke(this, args);
            }
            else if(message == API.WM_KEYUP || message == API.WM_SYSKEYUP)
            {
                KeyUpEvent?.Invoke(this, args);
            }
            return args.IsPrevent;
        }

        private IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (KeyboardProcedure((int)wParam, Marshal.PtrToStructure<API.KBDLLHOOKSTRUCT>(lParam)))
                {
                    return (IntPtr)1;
                }
            }
            return API.CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}