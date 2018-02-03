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
        private const string MESSAGE_BOX_ERROR_TITLE = "ERROR";
        private const string CONNECT_BUTTON_STRING = "Connect";
        private const string DISCONNECT_BUTTON_STRING = "Disconnect";
        private const string STATUS_TEXT_BOX_TEXT_CONNECTED = "Connected";
        private const string STATUS_TEXT_BOX_TEXT_NOT_CONNECTED = "Not Connected";
        private const string STATUS_TEXT_BOX_TEXT_NOT_VALID_URI = "The given URI is not valid!";
        private const string STATUS_TEXT_BOX_TEXT_EMPTY_URI_FIELD = "Please fill out the Rosbridge URI field!";

        private MainWindowViewModel _viewModel;
        private IMessageDispatcher _messageDispatcher;
        private List<Window> _windowList;

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
            _viewModel.ConnectButtonText = CONNECT_BUTTON_STRING;
            _viewModel.StatusTextBoxText = STATUS_TEXT_BOX_TEXT_NOT_CONNECTED;
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

                _viewModel.StatusTextBoxText = STATUS_TEXT_BOX_TEXT_NOT_CONNECTED;
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
                                _viewModel.ConnectButtonText = DISCONNECT_BUTTON_STRING;
                                _viewModel.ConnectedToRosBridge = true;
                                _viewModel.StatusTextBoxText = STATUS_TEXT_BOX_TEXT_CONNECTED;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, MESSAGE_BOX_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                            _messageDispatcher = null;
                        }

                        return;
                    }
                    else
                    {
                        _viewModel.StatusTextBoxText = STATUS_TEXT_BOX_TEXT_NOT_VALID_URI;
                    }
                }
                else
                {
                    _viewModel.StatusTextBoxText = STATUS_TEXT_BOX_TEXT_EMPTY_URI_FIELD;
                }
            }

            _viewModel.ConnectButtonText = CONNECT_BUTTON_STRING;
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
