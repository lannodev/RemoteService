using System;
using ToastNotifications;
using ToastNotifications.Core;
using MaterialDesignThemes.Wpf;

namespace RemoteService.Notification
{
    public static class ServiceExtensions
    {
        public static void ShowNotification(this Notifier notifier,
            string title,
            string subtitle,
            PackIconKind icon,
            string state,
            Action<ServiceNotification> confirmAction,
            Action<ServiceNotification> declineAction,
            MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new ServiceNotification(title, subtitle, icon, state, confirmAction, declineAction, messageOptions));
        }
    }
}
