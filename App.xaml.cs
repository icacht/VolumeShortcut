using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

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
        private System.Windows.Forms.NotifyIcon notifyIcon;

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

            var menu = new System.Windows.Forms.ContextMenuStrip();
            foreach(var item in new System.Windows.Forms.ToolStripMenuItem[]
                {
                    new System.Windows.Forms.ToolStripMenuItem("&Setting", null, WindowShowEvent),
                    new System.Windows.Forms.ToolStripMenuItem("&Exit", null, ExitEvent)
                })
            {
                menu.Items.Add(item);
            }

            var assm = Assembly.GetExecutingAssembly();
            using var resource = assm.GetManifestResourceStream(assm.GetName().Name + ".assets.icon.ico");
            if(resource is null)
            {
                throw new Exception("Icon File Not Found.");
            }

            notifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = new System.Drawing.Icon(resource),
                ContextMenuStrip = menu,
                Text = "Volume Shortcut",
                Visible = true,
            };
            notifyIcon.DoubleClick += WindowShowEvent;

            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
        }

        private void ExitApplication(object sender, ExitEventArgs e)
        {
            notifyIcon.DoubleClick -= WindowShowEvent;
            notifyIcon.Dispose();

            hooker.KeyDownEvent -= changer.KeyDownEventProcedure;
            hooker.Dispose();
        }

        private void WindowShowEvent(object sender, EventArgs e)
        {
            mainWindow.ShowWindow();
        }
        
        private void ExitEvent(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
