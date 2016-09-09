using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using eLublin.Web.Models.Db;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace eLublin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage
    {
        public StartPage()
        {
            InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }
        private static void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame == null) return;
            if (!frame.CanGoBack) return;
            frame.GoBack();
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {         
            NameBlock.Text = StaticData.FirstName + " " + StaticData.LastName;
            LevelBlock.Text = "Poziom " + StaticData.Level;
            LevelBar.Value = StaticData.Expvalu;
        }  

        private async void StartPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Map.ZoomLevel = 16;
            Map.Center = TempData.Position.Coordinate.Point;
            BasicGeoposition pos = new BasicGeoposition();
            MapIcon mapIcon2 = new MapIcon();
            pos.Latitude = TempData.Position.Coordinate.Latitude;
            pos.Longitude = TempData.Position.Coordinate.Longitude;
            mapIcon2.Location = new Geopoint(pos);
            mapIcon2.NormalizedAnchorPoint = new Point(0, 0);
            mapIcon2.Title = "Twoja lokalizacja";
            mapIcon2.Visible = true;
            mapIcon2.ZIndex = int.MaxValue;

            Map.MapElements.Add(mapIcon2);
            try
            {
                var clnt = Service.GetAuthorizedClient();
                var rponse = await clnt.GetAsync(Service.ServiceUrl + "api/Reports");
                var rult = await rponse.Content.ReadAsStringAsync();
                var oj = JsonConvert.DeserializeObject<List<Report>>(rult);

                foreach (var var in oj)
                {
                    //BasicGeoposition position = new BasicGeoposition();
                    //MapIcon icon = new MapIcon();
                    //position.Latitude = var.
                    //mapIcon2.Location = new Geopoint(pos);
                    //mapIcon2.NormalizedAnchorPoint = new Point(0, 0);
                    //mapIcon2.Title = "Twoja lokalizacja";
                    //mapIcon2.Visible = true;
                    //mapIcon2.ZIndex = int.MaxValue;

                }
                
                
                ////listaProjektow.ItemsSource = el.Where(x => x.type == 4);
                //listaOgloszen.ItemsSource = el.Where(x => x.type == 3);
            }
            catch (Exception exc)
            {
                var dlg = new MessageDialog(exc.Message);
                dlg.ShowAsync();
            }

            var client = Service.GetAuthorizedClient();
            var response = await client.GetAsync(Service.ServiceUrl + "api/Notifications");
            var result = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<List<Notification>>(result);
            foreach (var t in obj)
            {
                t.tytul = t.tytul.Trim();
            }
            PivotItem1.DataContext = obj;
        }

        private void AddIcon_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ReportEvent));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(typeof(StartPage));
            frame.BackStack.RemoveAt(frame.BackStack.Count - 1);
        }

        private void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
