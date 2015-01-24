using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CoPilot.Desktop.Data
{
    public class NetworkScan : INotifyPropertyChanged
    {
        public const String KEY_ADRESS = "LastAdress";

        #region PRIVATE

        private ApplicationDataContainer setting = ApplicationData.Current.LocalSettings;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is Network
        /// </summary>
        private Boolean isNetwork = false;
        public Boolean IsNetwork
        {
            get
            {
                return isNetwork;
            }
            set
            {
                if (isNetwork == value)
                {
                    return;
                }
                isNetwork = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Last Adress
        /// </summary>
        public String LastAdress
        {
            get
            {
                return this.getKey(KEY_ADRESS);
            }
            set
            {
                var adr = this.LastAdress;
                if (adr == value)
                {
                    return;
                }
                //set
                this.setKey(KEY_ADRESS, value);
                RaisePropertyChanged();
            }
        }

        #endregion

        public NetworkScan()
        {
            //scaner prepare
            this.prepareScaner();
            //status change
            NetworkInformation.NetworkStatusChanged += statusChange;
        }

        /// <summary>
        /// Prepare scaner
        /// </summary>
        private void prepareScaner()
        {
            //resolve profile
            this.IsNetwork = this.getIsNetwork();
        }

        /// <summary>
        /// Get key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private String getKey(string key)
        {
            return setting.Values.ContainsKey(key) ? setting.Values[key] as String : null;
        }

        /// <summary>
        /// Set key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void setKey(string key, string value)
        {
            if (setting.Values.ContainsKey(key))
            {
                setting.Values[key] = value;
            }
            else
            {
                setting.Values.Add(key, value);
            }
        }

        /// <summary>
        /// Get is network
        /// </summary>
        /// <returns></returns>
        private Boolean getIsNetwork()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null && profile.IsWlanConnectionProfile)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Network staus change
        /// </summary>
        /// <param name="sender"></param>
        private void statusChange(object sender)
        {
            this.prepareScaner();
        }

        #region PROPERTY CHANGE

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// On property change
        /// </summary>
        /// <param name="name"></param>
        public async void OnPropertyChanged(string name)
        {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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
