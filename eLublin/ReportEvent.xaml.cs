using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using eLublin.Web.Models.Api;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace eLublin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportEvent
    {
        public ReportEvent()
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
        
        private StorageFile streampub { get; set; }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NameBlock.Text = StaticData.FirstName + " "+ StaticData.LastName;
            LevelBlock.Text = "Poziom " + StaticData.Level;
            LevelBar.Value = StaticData.Expvalu;
        }

        private readonly CoreApplicationView _view = CoreApplication.GetCurrentView();

        private void AddPhoto_OnClick(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            _view.Activated += ViewActivated;
        }

        private async void ViewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            var args = args1 as FileOpenPickerContinuationEventArgs;

            if (args == null) return;
            if (args.Files.Count == 0) return;

            _view.Activated -= ViewActivated;
            var storageFileWp = args.Files[0];
            var stream = await storageFileWp.OpenAsync(FileAccessMode.Read);
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream);
            Img.Source = bitmapImage;
            streampub = args.Files[0];
            
        }
        private async void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            var post = new CommitReport
            {
                tekst = TextBox.Text,
                image = await ImageToBase64(streampub),
                lat = TempData.Position.Coordinate.Latitude.ToString(),
                lng = TempData.Position.Coordinate.Longitude.ToString()
            };
            var client = Service.GetAuthorizedClient();
            var uri = new Uri(Service.ServiceUrl + "api/Reports");
            var serial = JsonConvert.SerializeObject(post);
            var stringContent=new StringContent(serial, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(uri, stringContent);
            var requestContent = request.Content;
            var result = await requestContent.ReadAsStringAsync();
            //var await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));
            Frame.Navigate(typeof (StartPage));
        }
        public async Task<byte[]> ImageToBase64(StorageFile MyImageFile)
        {
            var ms = await MyImageFile.OpenStreamForReadAsync();
            var imageBytes = new byte[(int)ms.Length];
            ms.Read(imageBytes, 0, (int)ms.Length);
            //return Convert.ToBase64String(imageBytes);
            return imageBytes;
        }

    }
   
}