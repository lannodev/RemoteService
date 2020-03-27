using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteService.Dialogs
{
    /// <summary>
    /// Interação lógica para AlertDialog.xam
    /// </summary>
    public partial class AlertDialog : UserControl
    {
        public AlertDialog(string title, string buttonName)
        {
            InitializeComponent();
            Title.Text = title;
            ExecuteButton.Content = buttonName;
        }
    }
}
