namespace Rosbridge.Client.WPF.ViewModels
{
    public class TurtlesimSubscribeViewModel : ViewModelBase
    {
        private string _type;
        private string _currentMessage;

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
                OPC();
            }
        }

        public string CurrentMessage
        {
            get
            {
                return _currentMessage;
            }

            set
            {
                _currentMessage = value;
                OPC();
            }
        }

        public string Topic { get; set; }

        public TurtlesimSubscribeViewModel(string topic)
        {
            Topic = topic;
        }
    }
}
