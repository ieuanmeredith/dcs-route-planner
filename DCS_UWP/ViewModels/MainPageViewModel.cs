using AirFrameDrivers.Drivers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Globalization.NumberFormatting;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace DCS_UWP.ViewModels
{
    public class MainPageViewModel 
    {
        private bool Connected = false;
        private AirFrameFactory AirFrameFactory;

        public WayPointFormModel FormModel;

        public bool ConnectButtonEnabled { get { return !this.Connected; } }

        public IEnumerable<string> Airframes { get; set; }

        public IEnumerable<string> Locations { get; set; }
        public ObservableCollection<string> POINames { get; set; }
        public IEnumerable<POI> POIs { get; set; }

        public string SelectedAirframe { get; set; }

        public DecimalFormatter LatLongFormatter { get {
                DecimalFormatter formatter = new DecimalFormatter();
                formatter.IntegerDigits = 1;
                formatter.FractionDigits = 4;
                return formatter;
            }
        }
        public DecimalFormatter AltFormatter { get {
                DecimalFormatter formatter = new DecimalFormatter();
                formatter.IntegerDigits = 1;
                formatter.FractionDigits = 0;
                return formatter;
            } 
        }

        public ObservableCollection<LocationDetails> Waypoints { get; set; }

        public MainPageViewModel()
        {
            this.Locations = new List<string> { "Caucasus", "Persian Gulf", "Syria" };

            this.POINames = new ObservableCollection<string>();

            this.Waypoints = new ObservableCollection<LocationDetails>();

            this.FormModel = new WayPointFormModel();

            this.AirFrameFactory = new AirFrameFactory();
            this.Airframes = AirFrameFactory.GetAvailableAirFrames();
        }

        public async void OnLocationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<POI> poiArray;

            switch (e.AddedItems[0])
            {
                case "Caucasus":
                    poiArray = await this.ReadPoiFromFile("Caucasus.json");
                    break;
                case "Persian Gulf":
                    poiArray = await this.ReadPoiFromFile("Persian Gulf.json");
                    break;
                case "Syria":
                    poiArray = await this.ReadPoiFromFile("Syria.json");
                    break;
                default:
                    poiArray = new List<POI>();
                    break;
            }

            this.POIs = poiArray;

            this.POINames.Clear();

            foreach (var x in poiArray)
            {
                this.POINames.Add(x.Location[0].Name);
            }
        }

        public void OnPOISelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count() == 0)
            {
                return;
            }

            var poi = this.POIs.Where(x => x.Location[0].Name == e.AddedItems[0].ToString()).FirstOrDefault();

            this.FormModel.Lat = poi.LocationDetails.Lat;
            this.FormModel.Long = poi.LocationDetails.Lon;
            this.FormModel.Altitude = poi.LocationDetails.Altitude;
        }

        public void OnAirframeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count() == 0)
            {
                return;
            }

            this.SelectedAirframe = e.AddedItems[0].ToString();
        }

        public void AddWaypoint(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Waypoints.Add(new LocationDetails {
                Lat = this.FormModel.Lat,
                Lon = this.FormModel.Long,
                Altitude = this.FormModel.Altitude
            });

            this.FormModel.RemoveWaypointEnabled = true;
        }

        public void RemoveLastWaypoint()
        {
            this.Waypoints.RemoveAt(this.Waypoints.Count - 1);

            if (this.Waypoints.Count == 0)
            {
                this.FormModel.RemoveWaypointEnabled = false;
            }
        }

        public void PushWaypoints()
        {
            if (SelectedAirframe == null)
            {
                CreateErrorDialog("Select an airframe.");
                return;
            }

            if (Waypoints.Count == 0)
            {
                CreateErrorDialog("Enter at least 1 waypoint.");
                return;
            }

            try
            {
                var airFrame = this.AirFrameFactory.GetAirFrameDriver(SelectedAirframe);
                airFrame.SendWaypointsToDcs(this.Waypoints);
            }
            catch
            {
                CreateErrorDialog("Failed to connect to DCS. Ensure DCSFlightpanels DCS-BIOS is installed and DCS is running.");
            }
        }

        private async Task<POI[]> ReadPoiFromFile(string fileName)
        {
            StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\" + fileName);
            string text = await FileIO.ReadTextAsync(file);

            var poiArray = JsonConvert.DeserializeObject<POI[]>(text, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return poiArray;
        }

        private async void CreateErrorDialog(string message)
        {
            var dialog = new MessageDialog(message, "Oops");
            dialog.Commands.Add(new UICommand("Close", delegate (IUICommand command) { }));

            var result = await dialog.ShowAsync();
        }
    }
}
