using ToastNotifications.Core;

namespace RemoteService.Notification
{
    /// <summary>
    /// Interação lógica para IncomingCallView.xam
    /// </summary>
    public partial class IncomingCallView : NotificationDisplayPart
    {
        public IncomingCallView(ServiceNotification notification)
        {
            InitializeComponent();
            Bind(notification);
        }
    }
}
