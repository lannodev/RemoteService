using log4net;
using MaterialDesignThemes.Wpf;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using RemoteService.Dialogs;
using RemoteService.Model;
using RemoteService.Notification;
using RemoteService.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace RemoteService
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private SnackbarMessageQueue snackMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
        private ObservableCollection<Services> _serviceList = new ObservableCollection<Services>();
        private DispatcherTimer _task = new DispatcherTimer();
        private Notifier _notifier;
        private CimSession Session;
        private IEnumerable<CimInstance> AllServices;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Services _selectedService;
        private bool _isConnected;
        private bool _isLoading = false;
        private bool _autoStartService = true;
        private bool _enableNotification = true;
        private bool _minimizedWindow = false;
        private int _updateTime;
        private string _search;

        private string _server;
        private string _domain;
        private string _username;
        private string _serviceFilter;
        private string _serviceQuery = "SELECT * FROM Win32_Service";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SnackbarNotify.MessageQueue = snackMessageQueue;
            AutoStartService = false;
            EnableNotification = false;

            _notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(notificationLifetime: TimeSpan.FromSeconds(10), MaximumNotificationCount.UnlimitedNotifications());
                cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 10, 10);
                cfg.DisplayOptions.Width = 350;
            });

            //SOFTWARE VERSION
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            SoftwareVersionText.Text = "Versão: " + version;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            if (UpdateTime == 0)
                UpdateTime = 3;

            _task.Tick += new EventHandler(OnServiceUpdate);
            _task.Interval = TimeSpan.FromSeconds(UpdateTime);

            if (AutoStartService)
            {
                //Initialize WMI Session
                SessionConnect();
                _task.Start();
                IsLoading = true;
            }

            //Search Collection
            ServiceItemView.Filter = new Predicate<object>(o => Filter(o as Services));

            //Set Theme
            Toggle.IsChecked = Settings.Default.Theme == MaterialDesignThemes.Wpf.BaseTheme.Dark;

            //Hiden Window
            if (MinimizedWindow)
                this.Visibility = Visibility.Hidden;

        }


        #region Settings Properties
        public ObservableCollection<Services> ServiceList
        {
            get { return _serviceList; }
            set { _serviceList = value; OnPropertyChanged(); }
        }

        public ICollectionView ServiceItemView
        {
            get { return CollectionViewSource.GetDefaultView(ServiceList); }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool AutoStartService
        {
            get { return _autoStartService; }
            set { _autoStartService = value; OnPropertyChanged(); }
        }

        public bool EnableNotification
        {
            get { return _enableNotification; }
            set { _enableNotification = value; OnPropertyChanged(); }
        }

        public bool MinimizedWindow
        {
            get { return _minimizedWindow; }
            set { _minimizedWindow = value; OnPropertyChanged(); }
        }

        public int UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; OnPropertyChanged(); }
        }

        public string Server
        {
            get { return _server; }
            set { _server = value; OnPropertyChanged(); }
        }

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public string ServiceFilter
        {
            get { return _serviceFilter; }
            set { _serviceFilter = value; OnPropertyChanged(); }
        }

        public Services SelectedService
        {
            get { return _selectedService; }
            set { _selectedService = value; OnPropertyChanged(); }
        }

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnPropertyChanged();
                ServiceItemView.Refresh();
            }
        }

        #endregion

        #region Theme
        private void Toggle_Checked(object sender, RoutedEventArgs e) => SetTheme();

        private void Toggle_Unchecked(object sender, RoutedEventArgs e) => SetTheme();
        private void SetTheme()
        {
            if (Toggle.IsChecked == true)
            {
                Settings.Default.Theme = MaterialDesignThemes.Wpf.BaseTheme.Dark;
            }
            else
            {
                Settings.Default.Theme = MaterialDesignThemes.Wpf.BaseTheme.Light;
            }

            Settings.Default.Save();
            ((App)Application.Current).SetTheme(Settings.Default.Theme);
        }
        #endregion

        #region Config
        private void LoadConfig()
        {
            try
            {
                List<Config> config = SqliteDataAccess.GetConfig();
                foreach (var item in config)
                {
                    Server = item.Server;
                    Domain = item.Domain;
                    Username = item.Username;
                    PasswordTextBox.Password = item.Password;
                    ServiceFilter = item.Query;
                    AutoStartService = bool.Parse(item.AutoStart);
                    EnableNotification = bool.Parse(item.Notification);
                    MinimizedWindow = bool.Parse(item.Minimized);
                    UpdateTime = item.Duration;
                }
            }
            catch (Exception ex)
            {
                snackMessageQueue.Enqueue($"Erro ao ler as configurações. {ex}");
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D63031"));
                log.Error(ex.Message);
            }


        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Config config = new Config
                {
                    Server = this.Server,
                    Domain = this.Domain,
                    Username = this.Username,
                    Password = PasswordTextBox.Password,
                    Query = this.ServiceFilter,
                    AutoStart = this.AutoStartService.ToString(),
                    Notification = this.EnableNotification.ToString(),
                    Minimized = this.MinimizedWindow.ToString(),
                    Duration = this.UpdateTime
                };

                SqliteDataAccess.SaveConfig(config);

                snackMessageQueue.Enqueue("Configuração salva com sucesso!");
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01D275"));

                if (IsConnected)
                {
                    //Reiniciar Serviço
                    _task.Stop();
                    ServiceList.Clear();
                    _task.Start();
                }
                
            }
            catch (Exception ex)
            {
                snackMessageQueue.Enqueue($"Erro ao tentar salvar as configurações. {ex}");
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D63031"));
                log.Error(ex.Message);
            }
        }
        #endregion

        #region WMI Service
        private void OnServiceUpdate(object sender, EventArgs e)
        {

            Console.WriteLine("Atualizando lista de serviços WMI");

            try
            {
                string query;
                if (ServiceFilter != null)
                    query = $"{_serviceQuery} WHERE DisplayName LIKE '%{ServiceFilter}%'";
                else
                    query = _serviceQuery;

                AllServices = Session.QueryInstances(@"root\cimv2", "WQL", query);

                
                if (ServiceList.Count() > 0)
                {
                    //Loop through all services
                    foreach (CimInstance oneService in AllServices)
                    {

                        Services s = CimInstanceConvert(oneService);
                        Services service = ServiceList.FirstOrDefault(i => i.Name == s.Name);

                        if (service.State != s.State)
                        {
                            service.Update(s);
                            string subtitle;
                            PackIconKind icon;

                            switch (s.State)
                            {
                                case "Running":
                                    subtitle = "Serviço foi inicializado";
                                    icon = PackIconKind.Notifications;
                                    break;
                                case "Start Pending":
                                    subtitle = "Serviço esta sendo inicializado";
                                    icon = PackIconKind.InfoCircle;
                                    break;
                                case "Stopped":
                                    subtitle = "Serviço foi parado";
                                    icon = PackIconKind.Dangerous;
                                    break;
                                case "Stop Pending":
                                    subtitle = "Serviço esta sendo parado";
                                    icon = PackIconKind.Warning;
                                    break;
                                default:
                                    subtitle = "Estado não encontrado";
                                    icon = PackIconKind.SackPercent;
                                    break;
                            }

                            if (_enableNotification)
                                _notifier.ShowNotification(s.DisplayName, subtitle, icon, s.State, null, declineAction: n => CloseNotification(n));

                        }

                    }
                }
                else
                {
                    //Loop through all services
                    foreach (CimInstance oneService in AllServices)
                    {
                        Services s = CimInstanceConvert(oneService);
                        ServiceList.Add(s);
                    }
                }

                IsLoading = false;
                IsConnected = true;

            }
            catch (Exception ex)
            {
                IsConnected = false;
                IsLoading = false;
                snackMessageQueue.Enqueue(ex.Message);
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D20101"));
                _task.Stop();
                //ServiceList.Clear();
                log.Error(ex.Message);
            }

        }

        private Services CimInstanceConvert(CimInstance oneService)
        {

            return new Services
            {
                
                //STRINGS
                Caption = (oneService.CimInstanceProperties["Caption"].Value != null ? oneService.CimInstanceProperties["Caption"].Value.ToString() : ""),
                CreationClassName = (oneService.CimInstanceProperties["CreationClassName"].Value != null ? oneService.CimInstanceProperties["CreationClassName"].Value.ToString() : ""),
                Description = (oneService.CimInstanceProperties["Description"].Value != null ? oneService.CimInstanceProperties["Description"].Value.ToString() : ""),
                DisplayName = (oneService.CimInstanceProperties["DisplayName"].Value != null ? oneService.CimInstanceProperties["DisplayName"].Value.ToString() : ""),
                ErrorControl = (oneService.CimInstanceProperties["ErrorControl"].Value != null ? oneService.CimInstanceProperties["ErrorControl"].Value.ToString() : ""),
                Name = (oneService.CimInstanceProperties["Name"].Value != null ? oneService.CimInstanceProperties["Name"].Value.ToString() : ""),
                PathName = (oneService.CimInstanceProperties["PathName"].Value != null ? oneService.CimInstanceProperties["PathName"].Value.ToString() : ""),
                ServiceType = (oneService.CimInstanceProperties["ServiceType"].Value != null ? oneService.CimInstanceProperties["ServiceType"].Value.ToString() : ""),
                StartMode = (oneService.CimInstanceProperties["StartMode"].Value != null ? oneService.CimInstanceProperties["StartMode"].Value.ToString() : ""),
                StartName = (oneService.CimInstanceProperties["StartName"].Value != null ? oneService.CimInstanceProperties["StartName"].Value.ToString() : ""),
                State = (oneService.CimInstanceProperties["State"].Value != null ? oneService.CimInstanceProperties["State"].Value.ToString() : ""),
                Status = (oneService.CimInstanceProperties["Status"].Value != null ? oneService.CimInstanceProperties["Status"].Value.ToString() : ""),
                SystemCreationClassName = (oneService.CimInstanceProperties["SystemCreationClassName"].Value != null ? oneService.CimInstanceProperties["SystemCreationClassName"].Value.ToString() : ""),
                SystemName = (oneService.CimInstanceProperties["SystemName"].Value != null ? oneService.CimInstanceProperties["SystemName"].Value.ToString() : ""),

                //INT
                TagId = (oneService.CimInstanceProperties["TagId"] != null ? int.Parse(oneService.CimInstanceProperties["TagId"].Value.ToString()) : 0),
                WaitHint = (oneService.CimInstanceProperties["WaitHint"] != null ? int.Parse(oneService.CimInstanceProperties["WaitHint"].Value.ToString()) : 0),
                CheckPoint = (oneService.CimInstanceProperties["CheckPoint"] != null ? int.Parse(oneService.CimInstanceProperties["CheckPoint"].Value.ToString()) : 0),
                ProcessId = (oneService.CimInstanceProperties["ProcessId"] != null ? int.Parse(oneService.CimInstanceProperties["ProcessId"].Value.ToString()) : 0),
                ServiceSpecificExitCode = (oneService.CimInstanceProperties["ServiceSpecificExitCode"] != null ? int.Parse(oneService.CimInstanceProperties["ServiceSpecificExitCode"].Value.ToString()) : 0),
                ExitCode = (oneService.CimInstanceProperties["ExitCode"] != null ? int.Parse(oneService.CimInstanceProperties["ExitCode"].Value.ToString()) : 0),

                //BOOLEAN
                Started = (oneService.CimInstanceProperties["Started"] != null ? bool.Parse(oneService.CimInstanceProperties["Started"].Value.ToString()) : false),
                DesktopInteract = (oneService.CimInstanceProperties["DesktopInteract"] != null ? bool.Parse(oneService.CimInstanceProperties["DesktopInteract"].Value.ToString()) : false),
                AcceptPause = (oneService.CimInstanceProperties["AcceptPause"] != null ? bool.Parse(oneService.CimInstanceProperties["AcceptPause"].Value.ToString()) : false),
                AcceptStop = (oneService.CimInstanceProperties["AcceptPause"] != null ? bool.Parse(oneService.CimInstanceProperties["AcceptStop"].Value.ToString()) : false),
                DelayedAutoStart = (oneService.CimInstanceProperties["DelayedAutoStart"] != null ? bool.Parse(oneService.CimInstanceProperties["DelayedAutoStart"].Value.ToString()) : false),

                //DATETIME
                //InstallDate = DateTime.Parse(oneService.CimInstanceProperties["InstallDate"].Value.ToString()),

            };
        }

        private void SessionConnect()
        {
            try
            {
                SecureString securepassword = new SecureString();
                foreach (char c in PasswordTextBox.Password)
                    securepassword.AppendChar(c);

                // create Credentials
                CimCredential Credentials = new CimCredential(PasswordAuthenticationMechanism.Default, Domain, Username, securepassword);

                // create SessionOptions using Credentials
                WSManSessionOptions SessionOptions = new WSManSessionOptions();
                SessionOptions.AddDestinationCredentials(Credentials);

                // create Session using computer, SessionOptions
                Session = CimSession.Create(Server, SessionOptions);
            }
            catch (Exception ex)
            {
                snackMessageQueue.Enqueue(ex);
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D63031"));
                log.Error(ex.Message);
            }
        }

        #endregion

        #region Action Buttons
        private async void StartService_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Services s = ((Button)sender).Tag as Services;
                string title = $"Deseja iniciar o serviço {s.DisplayName}?";
                var dialog = new AlertDialog(title, "Iniciar");
                var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "MainDialogHost");

                if (result)
                {
                    CimInstance service = AllServices.OfType<CimInstance>().Where<CimInstance>(x => x.CimInstanceProperties["Name"].Value.ToString() == s.Name).FirstOrDefault();
                    Session.InvokeMethod(service, "StartService", null);
                    snackMessageQueue.Enqueue($"Comando enviado para parar o serviço {s.DisplayName}");
                    SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01D275"));
                }

            }
            catch (Exception ex)
            {
                snackMessageQueue.Enqueue($"Erro ao tentar parar o serviço. {ex}");
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D63031"));
                log.Error(ex.Message);

            }



        }

        private async void StopService_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Services s = ((Button)sender).Tag as Services;

                string title = $"Deseja parar o serviço {s.DisplayName}?";
                var dialog = new AlertDialog(title, "Parar");
                var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "MainDialogHost");

                if (result)
                {
                    CimInstance service = AllServices.OfType<CimInstance>().Where<CimInstance>(x => x.CimInstanceProperties["Name"].Value.ToString() == s.Name).FirstOrDefault();
                    Session.InvokeMethod(service, "StopService", null);
                    snackMessageQueue.Enqueue($"Comando enviado para parar o serviço {s.DisplayName}");
                    SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01D275"));
                }

            }
            catch (Exception ex)
            {
                snackMessageQueue.Enqueue($"Erro ao tentar parar o serviço. {ex}");
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D63031"));
                log.Error(ex.Message);

            }

        }

        private async void DeleteProcess_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Services s = ((Button)sender).Tag as Services;
                string title = $"Deseja interromper o processo {s.ProcessId}?";
                var dialog = new AlertDialog(title, "Parar");
                var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "MainDialogHost");

                if (result)
                {
                    string _processQuery = $"SELECT * FROM Win32_Process WHERE ProcessId = '{s.ProcessId}'";

                    IEnumerable<CimInstance> allProcess = Session.QueryInstances(@"root\cimv2", "WQL", _processQuery);
                    foreach (CimInstance process in allProcess)
                    {
                        Session.InvokeMethod(process, "Terminate", null);
                    }

                    snackMessageQueue.Enqueue($"Comando enviado para terminar o processo do serviço {s.DisplayName}");
                    SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01D275"));
                }

            }
            catch (Exception ex)
            {
                snackMessageQueue.Enqueue($"Erro ao tentar terminar o processo. {ex}");
                SnackbarNotify.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D63031"));
                log.Error(ex.Message);

            }
        }

        private void ConnectService_Click(object sender, RoutedEventArgs e)
        {

            if (IsConnected)
            {
                _task.Stop();
                IsConnected = false;
                ServiceList.Clear();
            }
            else
            {
                SessionConnect();
                _task.Start();
                IsConnected = true;
                IsLoading = true;
            }

        }

        private void CloseNotification(ServiceNotification n)
        {
            n.Close();
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {

            Button b = (Button)sender;
            int id = Convert.ToInt32(b.Tag);
            resetMenuColors();

            switch (b.Tag)
            {
                case "0":
                    menu0Icon.SetResourceReference(PackIcon.ForegroundProperty, "PrimaryHueLightBrush");
                    //menu0Text.SetResourceReference(TextBlock.ForegroundProperty, "Primary");
                    TabControllPage.SelectedIndex = 0;
                    break;
                case "1":
                    menu1Icon.SetResourceReference(PackIcon.ForegroundProperty, "PrimaryHueLightBrush");
                    //menu1Text.SetResourceReference(TextBlock.ForegroundProperty, "Primary");
                    TabControllPage.SelectedIndex = 1;
                    break;
                case "2":
                    menu2Icon.SetResourceReference(PackIcon.ForegroundProperty, "PrimaryHueLightBrush");
                    //menu2Text.SetResourceReference(TextBlock.ForegroundProperty, "Primary");
                    TabControllPage.SelectedIndex = 2;
                    break;
                //case "3":
                //    menu3Icon.SetResourceReference(PackIcon.ForegroundProperty, "Primary");
                //    menu3Text.SetResourceReference(TextBlock.ForegroundProperty, "Primary");
                //    TabControllPage.SelectedIndex = 3;
                //    break;
                //case "4":
                //    menu4Icon.SetResourceReference(PackIcon.ForegroundProperty, "Primary");
                //    menu4Text.SetResourceReference(TextBlock.ForegroundProperty, "Primary");
                //    break;
                default:
                    break;
            }

            GridCursor.Margin = new Thickness(0, (40 * id), 0, 0);
        }

        private void resetMenuColors()
        {
            menu0Icon.SetResourceReference(PackIcon.ForegroundProperty, "Secondary");
            menu1Icon.SetResourceReference(PackIcon.ForegroundProperty, "Secondary");
            menu2Icon.SetResourceReference(PackIcon.ForegroundProperty, "Secondary");
            //menu3Icon.SetResourceReference(PackIcon.ForegroundProperty, "Secondary");
            //menu4Icon.SetResourceReference(PackIcon.ForegroundProperty, "Secondary");

            //menu0Text.SetResourceReference(TextBlock.ForegroundProperty, "Secondary");
            //menu1Text.SetResourceReference(TextBlock.ForegroundProperty, "Secondary");
            //menu2Text.SetResourceReference(TextBlock.ForegroundProperty, "Secondary");
            //menu3Text.SetResourceReference(TextBlock.ForegroundProperty, "Secondary");
            //menu4Text.SetResourceReference(TextBlock.ForegroundProperty, "Secondary");
        }

        private void ServiceInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Services s = ((Button)sender).Tag as Services;
                SelectedService = s;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        #endregion

        #region Filter
        private bool Filter(Services servie)
        {
            return Search == null || servie.DisplayName.CaseInsensitiveContains(Search, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Window Action

        private void MoveWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (System.InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void HideWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void MinimizeProgram_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeProgram_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        #endregion

        #region URI LINKS

        private void URILinkedin_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/lucianowork/");
        }

        private void URIGitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/luciano-work");
        }
        private void URICoffe_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.buymeacoffee.com/wefixit");
        }
        #endregion


    }

    public static class Extensions
    {
        public static bool CaseInsensitiveContains(this string text, string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }

}
