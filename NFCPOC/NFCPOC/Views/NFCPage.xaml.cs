using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NFCPOC.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NFCPage : ContentPage
    {

        public ViewModels.NFCPageViewModel ViewModel { get; set; }

        public NFCPage()
        {
            InitializeComponent();
            ViewModel = new ViewModels.NFCPageViewModel(Navigation) { Title = Helpers.ConstantsHelper.APP_NAME };
            this.BindingContext = ViewModel;

            if (!Global.IsSubscribed)
            {
                MessagingCenter.Subscribe<string>(this, "ReadMessage", (message) =>
                {
                    ViewModel.OnRefresh();
                    Xamarin.Essentials.Vibration.Vibrate();
                    //App.Current.MainPage.DisplayAlert("Message Received", message, "Ok");
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.OnRefresh();
        }
    }
}