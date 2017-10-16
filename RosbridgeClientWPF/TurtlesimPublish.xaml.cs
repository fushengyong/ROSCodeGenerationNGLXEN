namespace RosbridgeClientWPF
{
    using RosbridgeClientCommon.Interfaces;
    using RosbridgeClientV2_0;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for TurtlesimPublish.xaml
    /// </summary>
    public partial class TurtlesimPublish : Window
    {
        private const double MAX_STEP = 5;
        private const double MIN_STEP = 0.1;
        private TurtlesimPublishViewModel _viewModel;
        private IRosPublisher<Twist> _publisher;

        public TurtlesimPublish(string topic, IMessageDispatcher messageDispatcher)
        {
            InitializeComponent();
            Loaded += TurtlesimPublish_Loaded;
            Closing += TurtlesimPublish_Closing; ;
            _publisher = new Publisher<Twist>(topic, messageDispatcher);
            _viewModel = new TurtlesimPublishViewModel(topic, MIN_STEP, MAX_STEP);
        }

        private async void TurtlesimPublish_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;
            await _publisher.AdvertiseAsync();
        }

        private async void TurtlesimPublish_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await _publisher.UnadvertiseAsync();
            _publisher = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null && sender is Button)
            {
                Button senderButton = sender as Button;
                switch (senderButton.Tag.ToString())
                {
                    case "1":
                        Forward();
                        break;
                    case "2":
                        TurnLeft();
                        break;
                    case "3":
                        Backward();
                        break;
                    case "4":
                        TurnRight();
                        break;
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (null != e)
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Up:
                        Forward();
                        break;
                    case System.Windows.Input.Key.Down:
                        Backward();
                        break;
                    case System.Windows.Input.Key.Left:
                        TurnLeft();
                        break;
                    case System.Windows.Input.Key.Right:
                        TurnRight();
                        break;
                    case System.Windows.Input.Key.PageUp:
                        if (_viewModel.Step < MAX_STEP)
                        {
                            _viewModel.Step += 0.1;
                        }
                        else
                        {
                            _viewModel.Step = MAX_STEP;
                        }
                        break;
                    case System.Windows.Input.Key.PageDown:
                        if (_viewModel.Step > MIN_STEP)
                        {
                            _viewModel.Step -= 0.1;
                        }
                        else
                        {
                            _viewModel.Step = MIN_STEP;
                        }
                        break;
                }
            }
        }

        private void Forward()
        {
            _publisher.PublishAsync(new Twist() { linear = new Vector3() { x = _viewModel.Step, y = 0.0, z = 0.0 }, angular = new Vector3() });
        }

        private void Backward()
        {
            _publisher.PublishAsync(new Twist() { linear = new Vector3() { x = _viewModel.Step * -1, y = 0.0, z = 0.0 }, angular = new Vector3() });
        }

        private void TurnLeft()
        {
            _publisher.PublishAsync(new Twist() { linear = new Vector3(), angular = new Vector3() { x = 0.0, y = 0.0, z = _viewModel.Step } });
        }
        private void TurnRight()
        {
            _publisher.PublishAsync(new Twist() { linear = new Vector3(), angular = new Vector3() { x = 0.0, y = 0.0, z = _viewModel.Step * -1 } });
        }
    }
}
