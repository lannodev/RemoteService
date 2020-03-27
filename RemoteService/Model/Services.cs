using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteService.Model
{
    public class Services : INotifyPropertyChanged
    {
        private string _name { get; set; }
        private string _displayName { get; set; }
        private bool _started { get; set; }
        private string _state { get; set; }
        private int _processId { get; set; }
        private bool _acceptPause { get; set; }
        private bool _acceptStop { get; set; }
        private string _caption { get; set; }
        private int _checkPoint { get; set; }
        private string _creationClassName { get; set; }
        private bool _delayedAutoStart { get; set; }
        private string _description { get; set; }
        private bool _desktopInteract { get; set; }
        private string _errorControl { get; set; }
        private int _exitCode { get; set; }
        private DateTime _installDate { get; set; }
        private string _pathName { get; set; }
        private int _serviceSpecificExitCode { get; set; }
        private string _serviceType { get; set; }
        private string _startMode { get; set; }
        private string _startName { get; set; }
        private string _status { get; set; }
        private string _systemCreationClassName { get; set; }
        private string _systemName { get; set; }
        private int _tagId { get; set; }
        private int _waitHint { get; set; }


        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }
        public bool Started
        {
            get { return _started; }
            set
            {
                if (_started != value)
                {
                    _started = value;
                    OnPropertyChanged("Started");
                }
            }
        }
        public string State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }
        public int ProcessId
        {
            get { return _processId; }
            set
            {
                if (_processId != value)
                {
                    _processId = value;
                    OnPropertyChanged("ProcessId");
                }
            }
        }
        public bool AcceptPause
        {
            get { return _acceptPause; }
            set
            {
                if (_acceptPause != value)
                {
                    _acceptPause = value;
                    OnPropertyChanged("AcceptPause");
                }
            }
        }
        public bool AcceptStop
        {
            get { return _acceptStop; }
            set
            {
                if (_acceptStop != value)
                {
                    _acceptStop = value;
                    OnPropertyChanged("AcceptStop");
                }
            }
        }
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    OnPropertyChanged("Caption");
                }
            }
        }
        public int CheckPoint
        {
            get { return _checkPoint; }
            set
            {
                if (_checkPoint != value)
                {
                    _checkPoint = value;
                    OnPropertyChanged("CheckPoint");
                }
            }
        }
        public string CreationClassName
        {
            get { return _creationClassName; }
            set
            {
                if (_creationClassName != value)
                {
                    _creationClassName = value;
                    OnPropertyChanged("CreationClassName");
                }
            }
        }
        public bool DelayedAutoStart
        {
            get { return _delayedAutoStart; }
            set
            {
                if (_delayedAutoStart != value)
                {
                    _delayedAutoStart = value;
                    OnPropertyChanged("DelayedAutoStart");
                }
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        public bool DesktopInteract
        {
            get { return _desktopInteract; }
            set
            {
                if (_desktopInteract != value)
                {
                    _desktopInteract = value;
                    OnPropertyChanged("DesktopInteract");
                }
            }
        }
        public string ErrorControl
        {
            get { return _errorControl; }
            set
            {
                if (_errorControl != value)
                {
                    _errorControl = value;
                    OnPropertyChanged("ErrorControl");
                }
            }
        }
        public int ExitCode
        {
            get { return _exitCode; }
            set
            {
                if (_exitCode != value)
                {
                    _exitCode = value;
                    OnPropertyChanged("ExitCode");
                }
            }
        }
        public DateTime InstallDate
        {
            get { return _installDate; }
            set
            {
                if (_installDate != value)
                {
                    _installDate = value;
                    OnPropertyChanged("InstallDate");
                }
            }
        }
        public string PathName
        {
            get { return _pathName; }
            set
            {
                if (_pathName != value)
                {
                    _pathName = value;
                    OnPropertyChanged("PathName");
                }
            }
        }
        public int ServiceSpecificExitCode
        {
            get { return _serviceSpecificExitCode; }
            set
            {
                if (_serviceSpecificExitCode != value)
                {
                    _serviceSpecificExitCode = value;
                    OnPropertyChanged("ServiceSpecificExitCode");
                }
            }
        }
        public string ServiceType
        {
            get { return _serviceType; }
            set
            {
                if (_serviceType != value)
                {
                    _serviceType = value;
                    OnPropertyChanged("ServiceType");
                }
            }
        }
        public string StartMode
        {
            get { return _startMode; }
            set
            {
                if (_startMode != value)
                {
                    _startMode = value;
                    OnPropertyChanged("StartMode");
                }
            }
        }
        public string StartName
        {
            get { return _startName; }
            set
            {
                if (_startName != value)
                {
                    _startName = value;
                    OnPropertyChanged("StartName");
                }
            }
        }
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        public string SystemCreationClassName
        {
            get { return _systemCreationClassName; }
            set
            {
                if (_systemCreationClassName != value)
                {
                    _systemCreationClassName = value;
                    OnPropertyChanged("SystemCreationClassName");
                }
            }
        }
        public string SystemName
        {
            get { return _systemName; }
            set
            {
                if (_systemName != value)
                {
                    _systemName = value;
                    OnPropertyChanged("SystemName");
                }
            }
        }
        public int TagId
        {
            get { return _tagId; }
            set
            {
                if (_tagId != value)
                {
                    _tagId = value;
                    OnPropertyChanged("TagId");
                }
            }
        }
        public int WaitHint
        {
            get { return _waitHint; }
            set
            {
                if (_waitHint != value)
                {
                    _waitHint = value;
                    OnPropertyChanged("WaitHint");
                }
            }
        }

        public void Update(Services s)
        {
            Name = s.Name;
            DisplayName = s.DisplayName;
            Started = s.Started;
            State = s.State;
            ProcessId = s.ProcessId;
            AcceptPause = s.AcceptPause;
            AcceptStop = s.AcceptStop;
            Caption = s.Caption;
            CheckPoint = s.CheckPoint;
            CreationClassName = s.CreationClassName;
            DelayedAutoStart = s.DelayedAutoStart;
            Description = s.Description;
            DesktopInteract = s.DesktopInteract;
            ErrorControl = s.ErrorControl;
            ExitCode = s.ExitCode;
            InstallDate = s.InstallDate;
            PathName = s.PathName;
            ServiceType = s.ServiceType;
            StartMode = s.StartMode;
            StartName = s.StartName;
            Status = s.Status;
            SystemCreationClassName = s.SystemCreationClassName;
            SystemName = s.SystemName;
            TagId = s.TagId;
            WaitHint = s.WaitHint;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
