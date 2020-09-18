using System;
using System.Windows;
using System.Windows.Input;

namespace VolumeShortcut
{
    using KeyCombination = System.ValueTuple<int, bool, bool, bool>;

    #nullable enable
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private Action<object>? action;
        private Func<object, bool>? canExecute;

        public RelayCommand(Func<object, bool>? canExecute, Action<object>? action)
        {
            this.canExecute = canExecute;
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            if (action is null)
            {
                return false;
            }
            if (canExecute is null)
            {
                return true;
            }
            return canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            action?.Invoke(parameter);
        }
    }
    #nullable disable


    public sealed class MainWindowViewModel
    {
        public class KeyPropety
        {
            public int Key { get; set; }
            public bool IsShift { get; set; }
            public bool IsCtrl { get; set; }
            public bool IsAlt { get; set; }

            public KeyCombination GetCombination()
            {
                return (Key, IsShift, IsCtrl, IsAlt);
            }

            public void SetProperty(KeyCombination combination)
            {
                (var keyCode, var isShift, var isCtrl, var isAlt) = combination;
                Key = keyCode;
                IsShift = isShift;
                IsCtrl = isCtrl;
                IsAlt = isAlt;
            }
        }

        public KeyPropety VolumeUp { get; }
        public KeyPropety VolumeDown { get; }
        public ICommand ApplyCommand { get; private set; } = new RelayCommand(
            null,
            (parameter) => { MessageBox.Show("Apply"); }
        );

        public MainWindowViewModel()
        {
            VolumeUp = new KeyPropety();
            VolumeDown = new KeyPropety();
        }

        public void SetProperty(KeyCombination volumeUp, KeyCombination volumeDown)
        {
            VolumeUp.SetProperty(volumeUp);
            VolumeDown.SetProperty(volumeDown);
        }
    }
}