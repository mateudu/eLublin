using System;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace eLublin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstPage
    {
        public FirstPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            TempData.Position = await TempData.Lokalizuj();
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (await Service.LoginAsync(EmailBox.Text, PasswordBox.Password))
            {
                var userinfo = await Service.GetUserInfoAsync();
                StaticData.FirstName = userinfo.firstName;
                StaticData.LastName = userinfo.lastName;
                if (userinfo.exp == null) return;
                StaticData.Level = Convert.ToInt32(Math.Floor(Math.Sqrt(userinfo.exp.Value)));
                var abovelevel = userinfo.exp - StaticData.Level*StaticData.Level;
                abovelevel = abovelevel*100;
                var delta = (StaticData.Level + 1)*(StaticData.Level + 1) - StaticData.Level*StaticData.Level;
                StaticData.Expvalu = Convert.ToDouble(abovelevel/delta);
                Frame.Navigate(typeof (StartPage));
            }
            else
            {
                PasswordBox.Password = "";
                PasswordBox.PlaceholderText = "Wpisz ponownie hasło";
                var dlg = new MessageDialog("Błędne hasło");
                await dlg.ShowAsync();
            }
        }

        private void EmailBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) PasswordBox.Focus(FocusState.Keyboard);
        }

        private void PasswordBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) LoginButton_OnClick(this, e);
        }
    }
}