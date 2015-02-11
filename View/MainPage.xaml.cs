using CoPilot.Desktop.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CoPilot.Desktop.View
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region PRIVATE

        private NetworkScan scanner;
        private ResourceLoader strings;
        private String highlight;
        private String lastSearch = "";

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is loaded
        /// </summary>
        private Boolean isLoaded = false;
        public Boolean IsLoaded
        {
            get
            {
                return isLoaded && !isNavigation;
            }
            set
            {
                isLoaded = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is input
        /// </summary>
        private Boolean isInput = false;
        public Boolean IsInput
        {
            get
            {
                return isInput && !isNavigation;
            }
            set
            {
                isInput = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// Is internet
        /// </summary>
        private Boolean isInternet = false;
        public Boolean IsInternet
        {
            get
            {
                return isInternet;
            }
            set
            {
                isInternet = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is navigating
        /// </summary>
        private Boolean isNavigation = false;
        public Boolean IsNavigation
        {
            get
            {
                return isNavigation;
            }
            set
            {
                isNavigation = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is search
        /// </summary>
        private Boolean isSearch = false;
        public Boolean IsSearch
        {
            get
            {
                return isSearch;
            }
            set
            {
                isSearch = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// LastAdress
        /// </summary>
        public String LastAdress
        {
            get
            {
                return this.scanner.LastAdress;
            }
        }

        /// <summary>
        /// WebView
        /// </summary>
        public WebView WebView
        {
            get
            {
                return this.Browser;
            }
        }

        #endregion

        #region ENUM

        public enum SettingCommandType
        {
            Support,
            Feedback,
            PrivatePolicy,
            Facebook,
            Twitter,
            Blog
        }

        #endregion


        /// <summary>
        /// Main page
        /// </summary>
        public MainPage()
        {
            //init
            this.InitializeComponent();
            //scanner, browser
            this.createBrowser();
            this.createNetworkScan();
            this.createDataTransfer();
            this.createSearchFlyout();
            this.createSettingPage();
            //dark theme
            this.RequestedTheme = ElementTheme.Dark;
            //size change
            this.SizeChanged += MainPage_SizeChanged;
            //strings
            this.strings = new ResourceLoader();
            //context
            this.DataContext = this;
        }

        /// <summary>
        /// Size change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 700) 
            { 
                var x = VisualStateManager.GoToState(this, "MinimalLayout", true); 
            }  
            else 
            { 
                VisualStateManager.GoToState(this, "DefaultLayout", true); 
            } 
        }

        /// <summary>
        /// Create settings page
        /// </summary>
        private void createSettingPage()
        {
            var settings = SettingsPane.GetForCurrentView();
            settings.CommandsRequested += (SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args) =>
            {
                UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);

                SettingsCommand blog = new SettingsCommand(SettingCommandType.Blog, this.strings.GetString("Settings/Blog"), handler);
                args.Request.ApplicationCommands.Add(blog);

                SettingsCommand facebook = new SettingsCommand(SettingCommandType.Facebook, this.strings.GetString("Settings/Facebook"), handler);
                args.Request.ApplicationCommands.Add(facebook);

                SettingsCommand twitter = new SettingsCommand(SettingCommandType.Twitter, this.strings.GetString("Settings/Twitter"), handler);
                args.Request.ApplicationCommands.Add(twitter);

                SettingsCommand support = new SettingsCommand(SettingCommandType.Support, this.strings.GetString("Settings/Support"), handler);
                args.Request.ApplicationCommands.Add(support);

                SettingsCommand feedback = new SettingsCommand(SettingCommandType.Feedback, this.strings.GetString("Settings/Feedback"), handler);
                args.Request.ApplicationCommands.Add(feedback);

                SettingsCommand pp = new SettingsCommand(SettingCommandType.PrivatePolicy, this.strings.GetString("Settings/PrivatePolicy"), handler);
                args.Request.ApplicationCommands.Add(pp);
            };
        }

        /// <summary>
        /// Invoker
        /// </summary>
        /// <param name="command"></param>
        private async void onSettingsCommand(IUICommand command)
        {
            SettingCommandType type = (SettingCommandType)command.Id;

            switch (type)
            {
                case SettingCommandType.Blog:
                    await Launcher.LaunchUriAsync(new Uri("http://carcopilot.blogspot.cz/"));
                    break;
                case SettingCommandType.Feedback:
                    await Launcher.LaunchUriAsync(new Uri("mailto:stanislav.hacker@live.com?Subject=CoPilot Desktop Feedback"));
                    break;
                case SettingCommandType.PrivatePolicy:
                    await Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/notes/co-pilot/privacy-policy/1490299477853259"));
                    break;
                case SettingCommandType.Support:
                    await Launcher.LaunchUriAsync(new Uri("mailto:stanislav.hacker@live.com?Subject=CoPilot Support"));
                    break;
                case SettingCommandType.Facebook:
                    await Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/carcopilot"));
                    break;
                case SettingCommandType.Twitter:
                    await Launcher.LaunchUriAsync(new Uri("https://twitter.com/carcopilot"));
                    break;
            }
        }

        /// <summary>
        /// Create search fylout
        /// </summary>
        private void createSearchFlyout()
        {
            this.SearchFlyout.Closed += delegate
            {
                this.searchOnPage("");
            };
            this.SearchFlyout.Opened += delegate
            {
                this.searchOnPage(lastSearch);
            };
        }

        /// <summary>
        /// Create data transfer
        /// </summary>
        private void createDataTransfer()
        {
            var transfer = DataTransferManager.GetForCurrentView();
            transfer.DataRequested += async (DataTransferManager sender, DataRequestedEventArgs args) =>
            {
                //We are going to use an async API to talk to the webview, so get a deferral for the results 
                DataRequestDeferral deferral = args.Request.GetDeferral(); 
                //data
                var data = await this.Browser.CaptureSelectedContentToDataPackageAsync();
                var request = args.Request; 
                //no selected info
                if (data != null && data.GetView().AvailableFormats.Count > 0)
                {
                    data.Properties.Title = strings.GetString("Share_Selection/Title");
                    data.Properties.Description = strings.GetString("Share_Selection/Description");
                    request.Data = data;
                }
                else
                {
                    InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
                    await this.Browser.CapturePreviewToStreamAsync(ms);
                    request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(ms));
                    request.Data.Properties.Title = strings.GetString("Share_Screen/Title");
                    request.Data.Properties.Description = strings.GetString("Share_Screen/Description");
                }
                //complete
                deferral.Complete(); 
            };
        }

        /// <summary>
        /// Create browser
        /// </summary>
        private void createBrowser()
        {
            this.Browser.NavigationCompleted += async (WebView sender, WebViewNavigationCompletedEventArgs e) =>
            {
                //await
                await Task.Delay(1000);
                //clear
                this.IsNavigation = false;
                //found
                this.IsInput = this.scanner.IsNetwork && !e.IsSuccess;
                //loaded
                this.IsLoaded = e.IsSuccess;
                //internet
                this.IsInternet = this.IsLoaded || this.scanner.IsNetwork;
                //appbar is is input
                if (this.IsInput) 
                {
                    this.BottomAppBar.IsOpen = true;
                }
            };
        }

        /// <summary>
        /// Create network scanner
        /// </summary>
        private void createNetworkScan()
        {
            //scanner
            this.scanner = new NetworkScan();
            //events
            this.scanner.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                var s = sender as NetworkScan;
                //found
                this.IsInput = s.IsNetwork && s.LastAdress == null;
                this.IsInternet = s.IsNetwork;
                this.BottomAppBar.IsOpen = this.IsInput;
                //loading
                this.loadeFromUrl();
            };

            //scan
            this.scanner.Scan();
        }

        /// <summary>
        /// Load from url
        /// </summary>
        private void loadeFromUrl()
        {
            if (!this.isInput && !this.isNavigation)
            {
                //get url
                var url = "http://" + this.scanner.LastAdress + "/copilot/";
                //valid url
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    //set
                    this.IsNavigation = true;
                    this.IsInternet = true;
                    //navigate
                    this.Browser.Navigate(new Uri(url));
                }
                else
                {
                    this.IsInput = true;
                    this.IsLoaded = false;
                    this.BottomAppBar.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Set url
        /// </summary>
        /// <param name="b"></param>
        private void setUrl(String text)
        {
            this.scanner.LastAdress = text;
        }

        #region ADRESS INPUT

        /// <summary>
        /// Adress keydown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdressInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                TextBox b = sender as TextBox;
                setUrl(b.Text);
            }
        }

        /// <summary>
        /// Lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdressInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox b = sender as TextBox;
            setUrl(b.Text);
        }

        #endregion

        #region REFRESH

        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resfresh_Click(object sender, RoutedEventArgs e)
        {
            this.Browser.Refresh();
        }

        #endregion

        #region BACK

        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Browser.GoBack();
        }

        #endregion

        #region HOME

        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            UriBuilder b = new UriBuilder(this.Browser.Source);
            b.Fragment = "";
            this.Browser.Source = b.Uri;
        }

        #endregion

        #region LINK

        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkYes_Click(object sender, RoutedEventArgs e)
        {
            this.IsInput = true;
            this.IsLoaded = false;
            this.scanner.LastAdress = "";
            RaisePropertyChanged("LastAdress");
            this.LinkFlyout.Hide();
        }

        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkNo_Click(object sender, RoutedEventArgs e)
        {
            this.LinkFlyout.Hide();
        }

        #endregion

        #region SEARCH

        /// <summary>
        /// Search change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Search_QueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs args)
        {
            this.lastSearch = args.QueryText;
            this.searchOnPage(args.QueryText);
        }


        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Previous_Click(object sender, RoutedEventArgs e)
        {
            //prevous
            await this.Browser.InvokeScriptAsync("eval", new string[] { "Previous();" });
        }

        /// <summary>
        /// Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            //next
            await this.Browser.InvokeScriptAsync("eval", new string[] { "Next();" });
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="what"></param>
        private async void searchOnPage(String what)
        {
            Boolean success = true;
            try
            {
                //search
                await this.Browser.InvokeScriptAsync("eval", new string[] { "HiglightNow('" + what + "');" });
                //search
                this.IsSearch = !String.IsNullOrEmpty(what);
            }
            catch (Exception e)
            {
                success = false;
            }
            //inject script and search again
            if (success == false)
            {
                await this.Browser.InvokeScriptAsync("eval", new string[] { highlight });
                this.searchOnPage(what);
            }
        }

        #endregion

        #region OVVERIDE

        /// <summary>
        /// On navihgate to
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //load function
            StorageFile highlightFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///highlight.js"));
            this.highlight = await FileIO.ReadTextAsync(highlightFile); 
            //base
            base.OnNavigatedTo(e);
        }

        #endregion

        #region PROPERTY CHANGE

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// On property change
        /// </summary>
        /// <param name="name"></param>
        public async void OnPropertyChanged(string name)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            });
        }

        /// <summary>
        /// Raise proeprty change
        /// </summary>
        /// <param name="caller"></param>
        public async void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(caller));
                }
            });
        }

        #endregion
    }
}
