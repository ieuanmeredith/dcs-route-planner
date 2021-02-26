using System.ComponentModel;
using System.Runtime.CompilerServices;

public class WayPointFormModel : INotifyPropertyChanged
{
    private double lat;
    private double @long;
    private double alt;
    private bool removeWaypointEnabled = false;

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public double Lat
    {
        get { return this.lat; }
        set
        {
            this.lat = value;
            this.OnPropertyChanged();
        }
    }

    public double Long
    {
        get { return this.@long; }
        set
        {
            this.@long = value;
            this.OnPropertyChanged();
        }
    }
    public double Altitude
    {
        get { return this.alt; }
        set
        {
            this.alt = value;
            this.OnPropertyChanged();
        }
    }

    public bool RemoveWaypointEnabled
    {
        get { return this.removeWaypointEnabled; }
        set
        {
            this.removeWaypointEnabled = value;
            this.OnPropertyChanged();
        }
    }

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
