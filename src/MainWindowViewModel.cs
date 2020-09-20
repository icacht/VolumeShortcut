using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VolumeShortcut
{
    using KeyCombination = System.ValueTuple<int, bool, bool, bool>;
    public delegate void KeyCombinationUpdateEventHandler(in KeyCombination up, in KeyCombination down);

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
        public class KeyPropety : INotifyPropertyChanged
        {
            public int Key { get; set; }
            public bool IsShift { get; set; }
            public bool IsCtrl { get; set; }
            public bool IsAlt { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public KeyCombination GetCombination()
            {
                return (Key, IsShift, IsCtrl, IsAlt);
            }

            public void SetProperty(in KeyCombination combination)
            {
                (Key, IsShift, IsCtrl, IsAlt) = combination;

                NotifyPropertyChanged(nameof(Key));
                NotifyPropertyChanged(nameof(IsShift));
                NotifyPropertyChanged(nameof(IsCtrl));
                NotifyPropertyChanged(nameof(IsAlt));
            }

            private void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public KeyPropety VolumeUp { get; }
        public KeyPropety VolumeDown { get; }
        public ICommand ApplyCommand { get; private set; }

        public event KeyCombinationUpdateEventHandler KeyCombinationUpdateEvent;

        public MainWindowViewModel()
        {
            VolumeUp = new KeyPropety();
            VolumeDown = new KeyPropety();
            ApplyCommand = new RelayCommand(
                null,
                (parameter) =>
                {
                    KeyCombinationUpdateEvent?.Invoke(VolumeUp.GetCombination(), VolumeDown.GetCombination());
                }
            );
        }

        public void SetProperty(in KeyCombination volumeUp, in KeyCombination volumeDown)
        {
            VolumeUp.SetProperty(volumeUp);
            VolumeDown.SetProperty(volumeDown);
        }
    }
}