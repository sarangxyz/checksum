using System.ComponentModel;

public class BusyStatusTracker : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isBusy = false;
    public bool IsBusy
    {
        get
        {
            return _isBusy;
        }
        set
        {
            _isBusy = value;
            OnPropertyChanged("IsBusy");
        }
    }

    public BusyStatusTracker()
    {
        _isBusy = false;
    }

    protected void OnPropertyChanged(string name)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}