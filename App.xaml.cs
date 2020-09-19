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
        private KeyHooker hooker;
        private KeyChanger changer;
        private MainWindow mainWindow;

        private void StartupApplication(object sender, StartupEventArgs e)
        {
            hooker = new KeyHooker();
            changer = new KeyChanger();
            hooker.KeyDownEvent += changer.KeyDownEventProcedure;
            hooker.Hook();


            mainWindow = new MainWindow();
            var vm = ((MainWindowViewModel)MainWindow.DataContext);
            vm.KeyCombinationUpdateEvent += changer.KeyCombinationUpdate;

            var upCombination = ValueTuple.Create(0x77, true, true, false);
            var downCombination = ValueTuple.Create(0x76, true, true, false);
            vm.SetProperty(upCombination, downCombination);
            if (vm.ApplyCommand.CanExecute(null))
            {
                vm.ApplyCommand.Execute(null);
            }

            mainWindow.Visibility = Visibility.Visible;
        }
        
        private void ExitApplication(object sender, ExitEventArgs e)
        {
            hooker.KeyDownEvent -= changer.KeyDownEventProcedure;
            hooker.Dispose();
        }
    }
}
