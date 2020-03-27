using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToastNotifications.Core;
using RemoteService.Utils;
using MaterialDesignThemes.Wpf;

namespace RemoteService.Notification
{
    public class ServiceNotification : NotificationBase, INotifyPropertyChanged
    {

        private IncomingCallView _displayPart;
        private Action<ServiceNotification> _confirmAction;
        private Action<ServiceNotification> _declineAction;


        public ICommand ConfirmCommand { get; set; }
        public ICommand DeclineCommand { get; set; }


        public ServiceNotification(string title, string subtitle, PackIconKind icon, string state, Action<ServiceNotification> confirmAction, Action<ServiceNotification> declineAction, MessageOptions messageOptions) : base(title, messageOptions)
        {
            Title = title;
            Subtitle = subtitle;
            Icon = icon;
            State = state;
            _confirmAction = confirmAction;
            _declineAction = declineAction;

            ConfirmCommand = new RelayCommand(x => _confirmAction(this));
            DeclineCommand = new RelayCommand(x => _declineAction(this));
        }

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new IncomingCallView(this));

        #region binding properties

        private string _title;
        private string _subtitle;
        private PackIconKind _icon;
        private string _state;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Subtitle
        {
            get
            {
                return _subtitle;
            }
            set
            {
                _subtitle = value;
                OnPropertyChanged();
            }
        }

        public PackIconKind Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
