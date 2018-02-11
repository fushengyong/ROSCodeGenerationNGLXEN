namespace Rosbridge.Client.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _connectedToRosBridge;
        private string _connectButtonText;
        private string _statusTextBoxText;

        public bool ConnectedToRosBridge
        {
            get
            {
                return _connectedToRosBridge;
            }

            set
            {
                _connectedToRosBridge = value;
                OPC();
            }
        }

        public string ConnectButtonText
        {
            get
            {
                return _connectButtonText;
            }

            set
            {
                _connectButtonText = value;
                OPC();
            }
        }

        public string StatusTextBoxText
        {
            get
            {
                return _statusTextBoxText;
            }

            set
            {
                _statusTextBoxText = value;
                OPC();
            }
        }

        public string RosbridgeUri { get; set; }
        public string TurtlesimPublishTopicName { get; set; }
        public string TurtlesimSubscribeTopicName { get; set; }
    }
}
