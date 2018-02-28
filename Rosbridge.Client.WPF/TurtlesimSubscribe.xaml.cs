using Newtonsoft.Json;
using Rosbridge.Client.Common;
using Rosbridge.Client.Common.Interfaces;
using Rosbridge.Client.V2_0;
using Rosbridge.Client.WPF.ViewModels;
using RosMessages.geometry_msgs;
using System.Windows;

namespace Rosbridge.Client.WPF
{
    /// <summary>
    /// Interaction logic for TurtlesimSubscribe.xaml
    /// </summary>
    public partial class TurtlesimSubscribe : Window
    {
        private TurtlesimSubscribeViewModel _viewModel;
        private IRosSubscriber<Twist> _subscriber;

        public TurtlesimSubscribe(string topic, IMessageDispatcher messageDispatcher)
        {
            InitializeComponent();
            _viewModel = new TurtlesimSubscribeViewModel(topic);
            _subscriber = new Subscriber<Twist>(topic, messageDispatcher, new RosMessageTypeAttributeHelper());
            _subscriber.RosMessageReceived += _subscriber_RosMessageReceived;
            _viewModel.Type = _subscriber.Type;
            Loaded += TurtlesimSubscribe_Loaded;
            Closing += TurtlesimSubscribe_Closing;
        }

        private void TurtlesimSubscribe_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_subscriber != null)
            {
                _subscriber.UnsubscribeAsync();
                _subscriber = null;
            }
        }

        private void _subscriber_RosMessageReceived(object sender, Rosbridge.Client.Common.EventArguments.RosMessageReceivedEventArgs<Twist> args)
        {
            if (args != null)
            {
                _viewModel.CurrentMessage = JsonConvert.SerializeObject(args.RosMessage);
            }
        }

        private void TurtlesimSubscribe_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;
            _subscriber.SubscribeAsync();
        }
    }
}
