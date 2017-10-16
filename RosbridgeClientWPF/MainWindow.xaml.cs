namespace RosbridgeClientWPF
{
    using RosbridgeClientCommon;
    using RosbridgeClientCommon.Interfaces;
    using RosbridgeClientV2_0;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private IMessageDispatcher _messageDispatcher;
        private List<Window> _windowList;

        private const string ConnectButtonString = "Connect";
        private const string DisconnectButtonString = "Disconnect";
        private const string StatusTextBoxTextConnected = "Connected";
        private const string StatusTextBoxTextNotConnected = "Not Connected";
        private const string StatusTextBoxTextNotValidURI = "The given URI is not valid!";
        private const string StatusTextBoxTextEmptyURI = "Please fill out the Rosbridge URI field!";


        public MainWindow()
        {
            InitializeComponent();
            _windowList = new List<Window>();
            _viewModel = new MainWindowViewModel();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing; ;
        }

        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var window in _windowList)
            {
                window.Close();
            }

            if (_messageDispatcher != null)
            {
                await _messageDispatcher.StopAsync();
                _messageDispatcher.Dispose();
                _messageDispatcher = null;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;
            _viewModel.ConnectButtonText = ConnectButtonString;
            _viewModel.StatusTextBoxText = StatusTextBoxTextNotConnected;
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConnectedToRosBridge)
            {
                foreach (var window in _windowList)
                {
                    window.Close();
                }

                await _messageDispatcher.StopAsync();
                _messageDispatcher.Dispose();
                _messageDispatcher = null;

                _viewModel.StatusTextBoxText = StatusTextBoxTextNotConnected;
            }
            else
            {
                if (!string.IsNullOrEmpty(_viewModel.RosbridgeUri))
                {
                    Uri rosbridgeUri;
                    bool result = Uri.TryCreate(_viewModel.RosbridgeUri, UriKind.Absolute, out rosbridgeUri);
                    if (result == true)
                    {
                        try
                        {
                            _messageDispatcher = new MessageDispatcher(new Socket(rosbridgeUri), new MessageSerializer());
                            await _messageDispatcher.StartAsync();

                            if (_messageDispatcher.CurrentState == RosbridgeClientCommon.Enums.States.Started)
                            {
                                _viewModel.ConnectButtonText = DisconnectButtonString;
                                _viewModel.ConnectedToRosBridge = true;
                                _viewModel.StatusTextBoxText = StatusTextBoxTextConnected;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            _messageDispatcher = null;
                        }

                        return;
                    }
                    else
                    {
                        _viewModel.StatusTextBoxText = StatusTextBoxTextNotValidURI;
                    }
                }
                else
                {
                    _viewModel.StatusTextBoxText = StatusTextBoxTextEmptyURI;
                }
            }

            _viewModel.ConnectButtonText = ConnectButtonString;
            _viewModel.ConnectedToRosBridge = false;
        }

        private void TurtlesimPublish_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConnectedToRosBridge && !string.IsNullOrEmpty(_viewModel.TurtlesimPublishTopicName))
            {
                TurtlesimPublish publishWindow = new TurtlesimPublish(_viewModel.TurtlesimPublishTopicName, _messageDispatcher);
                publishWindow.Show();

                _windowList.Add(publishWindow);
            }
        }

        private void TurtlesimSubscribe_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConnectedToRosBridge)
            {
                TurtlesimSubscribe subscribeWindow = new TurtlesimSubscribe(_viewModel.TurtlesimSubscribeTopicName, _messageDispatcher);
                subscribeWindow.Show();

                _windowList.Add(subscribeWindow);
            }
        }
    }
}
