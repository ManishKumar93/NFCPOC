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
        public ICommand RefreshCommand { get; set; }


        public bool IsEventsSubscribed { get; set; }


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

        public void SubscribeEvents()
        {
            CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
            CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;

            if (Device.RuntimePlatform == Device.iOS)
                CrossNFC.Current.OniOSReadingSessionCancelled += Current_OniOSReadingSessionCancelled;

            CrossNFC.Current.StartListening();

            IsEventsSubscribed = true;
        }

        private void Current_OniOSReadingSessionCancelled(object sender, EventArgs e)
        {

        }

        private async Task DisplayTag(ITagInfo tagInfo, string title)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                if (tagInfo == null)
                {
                    await ShowAlert("TagInfo is null!", "Error");
                    return;
                }

                if (!string.IsNullOrEmpty(tagInfo.SerialNumber))
                {
                    sb.AppendLine("SerialNumber : " + tagInfo.SerialNumber);
                }

                sb.AppendLine("IsWritable : " + tagInfo.IsWritable);

                sb.AppendLine("IsEmpty : " + tagInfo.IsEmpty);

                sb.AppendLine("IsSupported : " + tagInfo.IsSupported);

                if (tagInfo.Records == null)
                {
                    sb.AppendLine("Records : null");
                }
                else
                {
                    sb.AppendLine("Records Count : " + tagInfo.Records.Length);
                    if (tagInfo.Records.Length > 0)
                    {
                        int i = 1;
                        foreach (var record in tagInfo.Records)
                        {
                            if (record != null)
                            {
                                sb.AppendLine("Record [" + i.ToString() + "] Message : " + NFCUtils.GetMessage(record));
                            }
                        }
                    }
                }

                await ShowAlert(sb.ToString(), "Tag Discovered");
            }
            catch (Exception ex)
            {
                await ShowAlert(ex.Message);
            }
            finally
            {
                CrossNFC.Current.StartListening();
            }
        }

        private async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            await DisplayTag(tagInfo, "Tag Discovered");
        }

        private async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            await DisplayTag(tagInfo, "Message Received");
        }

        private async void Current_OnMessagePublished(ITagInfo tagInfo)
        {
            try
            {
                await DisplayTag(tagInfo, "Message Published");
            }
            catch (System.Exception ex)
            {
                await ShowAlert(ex.Message);
            }
            finally
            {
                CrossNFC.Current.StopPublishing();
                CrossNFC.Current.StartListening();
            }
        }

        public async Task ShowAlert(string message, string title = null)
        {
            await App.Current.MainPage.DisplayAlert(string.IsNullOrWhiteSpace(title) ? Helpers.ConstantsHelper.APP_NAME : title, message, "Ok");
        }
    }
}
