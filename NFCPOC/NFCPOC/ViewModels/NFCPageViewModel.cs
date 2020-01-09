using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NFCPOC.ViewModels
{
    public class NFCPageViewModel : BaseViewModel
    {
        public ICommand OpenSettingsCommand { get; set; }

        public string WriteMessage
        {
            get => Global.WriteMessage;
            set
            {
                if (Global.WriteMessage != value)
                {
                    Global.WriteMessage = value;
                    OnPropertyChanged(nameof(WriteMessage));
                }
            }
        }

        public string LastMessageReceived
        {
            get
            {
                if (string.IsNullOrEmpty(Global.ReadMessage))
                {
                    return "";
                }
                else
                {
                    return Global.ReadMessage + Environment.NewLine + " at : " + Global.ReadOn.ToLongTimeString();
                }
            }
        }

        public bool IsNFCEnabled
        {
            get
            {
                if (CrossNFC.Current == null)
                {
                    return false;
                }
                else
                {
                    return (CrossNFC.IsSupported && CrossNFC.Current.IsAvailable && CrossNFC.Current.IsEnabled);
                }
            }
        }

        public NFCPageViewModel(INavigation navigation)
        {
            Navigation = navigation;

            OpenSettingsCommand = new Command(OnOpenSettings);
        }

        private void OnOpenSettings()
        {
            DependencyService.Get<NFCPOC.Interface.IOpenSettingsInterface>().OpenNFCSettings();
        }

        public void OnRefresh()
        {
            OnPropertyChanged(nameof(WriteMessage));
            OnPropertyChanged(nameof(LastMessageReceived));
            OnPropertyChanged(nameof(IsNFCEnabled));
        }
    }
}
