namespace Rosbridge.Client.WPF.ViewModels
{
    public class TurtlesimPublishViewModel : ViewModelBase
    {
        private double _step;

        public double Step
        {
            get
            {
                return _step;
            }

            set
            {
                _step = value;
                OPC();
            }
        }

        public string TopicToPublish { get; private set; }
        public double MinStep { get; set; }
        public double MaxStep { get; set; }

        public TurtlesimPublishViewModel(string topic, double minStep, double maxStep)
        {
            TopicToPublish = topic;
            MinStep = minStep;
            MaxStep = maxStep;
        }
    }
}
