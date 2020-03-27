using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RemoteService.Command
{
    public class TaskbarIconDoubleClickCommand : ICommand
    {
        public void Execute(object parameter)
        {
            Window win = ((Window)parameter) as Window;
            if (!win.IsVisible)
            {
                win.Show();
            }

            if (win.WindowState == WindowState.Minimized)
            {
                win.WindowState = WindowState.Normal;
            }

            if (win.WindowState == WindowState.Normal)
            {
                win.Activate();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
