using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NFCPOC.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NFCPage : ContentPage
    {
        public ViewModels.NFCPageViewModel ViewModel { get; set; }

        private bool _eventsSubscribed = false;

        public NFCPage()
        {
            InitializeComponent();
            ViewModel = new ViewModels.NFCPageViewModel(Navigation) { Title = Helpers.ConstantsHelper.APP_NAME };
            this.BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_eventsSubscribed)
            {
                SubscribeEvents();

                CrossNFC.Current.StartListening();
            }
        }

        Task ShowAlert(string message, string title = null) => DisplayAlert(string.IsNullOrWhiteSpace(title) ? Helpers.ConstantsHelper.APP_NAME : title, message, "Cancel");

        void SubscribeEvents()
        {
            CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
            CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;

            if (Device.RuntimePlatform == Device.iOS)
                CrossNFC.Current.OniOSReadingSessionCancelled += Current_OniOSReadingSessionCancelled;

            _eventsSubscribed = true;
        }

        private void Current_OniOSReadingSessionCancelled(object sender, EventArgs e)
        {

        }

        private async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            if (!CrossNFC.Current.IsWritingTagSupported)
            {
                await ShowAlert("Writing tag is not supported on this device");
                return;
            }

            try
            {
                NFCNdefRecord record = new NFCNdefRecord
                {
                    TypeFormat = NFCNdefTypeFormat.Uri,
                    Payload = NFCUtils.EncodeToByteArray(Helpers.ConstantsHelper.APP_NAME)
                };

                if (!format && record == null)
                    throw new Exception("Record can't be null.");

                tagInfo.Records = new[] { record };

                if (format)
                    CrossNFC.Current.ClearMessage(tagInfo);
            }
            catch (System.Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }

        string GetMessage(NFCNdefRecord record)
        {
            var message = $"Message: {record.Message}";
            message += Environment.NewLine;
            message += $"RawMessage: {Encoding.UTF8.GetString(record.Payload)}";
            message += Environment.NewLine;
            message += $"Type: {record.TypeFormat.ToString()}";

            if (!string.IsNullOrWhiteSpace(record.MimeType))
            {
                message += Environment.NewLine;
                message += $"MimeType: {record.MimeType}";
            }

            return message;
        }

        private async void Current_OnMessagePublished(ITagInfo tagInfo)
        {
            try
            {
                CrossNFC.Current.StopPublishing();
                if (tagInfo.IsEmpty)
                    await ShowAlert("Formatting tag successfully");
                else
                    await ShowAlert("Writing tag successfully");
            }
            catch (System.Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }

        private async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            try
            {
                if (tagInfo == null)
                {
                    await ShowAlert("No tag found");
                    return;
                }

                // Customized serial number
                var identifier = tagInfo.Identifier;
                var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");
                var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

                if (!tagInfo.IsSupported)
                {
                    await ShowAlert("Unsupported tag (app)", title);
                }
                else if (tagInfo.IsEmpty)
                {
                    await ShowAlert("Empty tag", title);
                }
                else
                {
                    var first = tagInfo.Records[0];
                    await ShowAlert(GetMessage(first), title);
                }
            }
            catch (Exception ex)
            {
                await ShowAlert(ex.Message);
            }
            finally
            {
                Current_OnTagDiscovered(tagInfo, true);
                CrossNFC.Current.StartListening();
            }
        }
    }
}