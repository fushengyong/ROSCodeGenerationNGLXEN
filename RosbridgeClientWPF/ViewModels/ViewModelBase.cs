namespace RosbridgeClientWPF.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OPC([CallerMemberName] string callerMember = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMember));
        }
    }
}
