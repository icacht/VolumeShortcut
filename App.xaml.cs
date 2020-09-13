using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VolumeShortcut
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        KeyHooker hooker;
        KeyChanger changer;

        private void StartupApplication(object sender, StartupEventArgs e)
        {
            hooker = new KeyHooker();
            changer = new KeyChanger();
            hooker.KeyDownEvent += changer.KeyDownEventProcedure;
            hooker.Hook();
        }
        
        private void ExitApplication(object sender, ExitEventArgs e)
        {
            hooker.Dispose();
        }
    }
}
