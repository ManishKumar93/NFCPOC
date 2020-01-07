using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace NFCPOC.ViewModels
{
    public class NFCPageViewModel : BaseViewModel
    {
        public ICommand RefreshCommand { get; set; }

        public bool IsSupported { get => CrossNFC.IsSupported; }
        public bool IsAvailable { get => CrossNFC.Current.IsAvailable; }
        public bool IsEnabled { get => CrossNFC.Current.IsEnabled; }

        public NFCPageViewModel(INavigation navigation)
        {
            Navigation = navigation;

            RefreshCommand = new Command(OnRefresh);
        }

        private void OnRefresh()
        {
            OnPropertyChanged(nameof(IsSupported));
            OnPropertyChanged(nameof(IsAvailable));
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
}
