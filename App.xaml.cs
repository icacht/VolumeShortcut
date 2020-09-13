using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VolumeShortcut
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        KeyHooker hooker;

        private void StartupApplication(object sender, StartupEventArgs e)
        {
            hooker = new KeyHooker();
            hooker.KeyDownEvent += KeyDownEventProcedure;
            hooker.Hook();
        }
        
        private void ExitApplication(object sender, ExitEventArgs e)
        {
            hooker.Dispose();
        }

        private void KeyDownEventProcedure(object obj, KeyHooker.KeyEventArgs e)
        {
            Console.WriteLine($"Keydown KeyCode {e.KeyCode}");
            if (e.IsVirtualInput)
            {
                return;
            }

            if (e.KeyCode == KeyInterop.VirtualKeyFromKey(Key.A))
            {
                return;
            }

            KeyInputer.KeyDownUp((uint)KeyInterop.VirtualKeyFromKey(Key.A));
            e.IsPrevent = true;
        }
    }
}
