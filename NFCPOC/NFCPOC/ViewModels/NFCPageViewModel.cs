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
        public string WriteMessage
        {
            get => Global.WriteMessage;
            set
            {
                Global.WriteMessage = value;
                OnPropertyChanged(nameof(WriteMessage));
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
                    return "Last Message Received : " + Global.ReadMessage + Environment.NewLine + " on : " + Global.ReadOn.ToShortTimeString();
                }
            }
        }

        public NFCPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }

        public void OnRefresh()
        {
            OnPropertyChanged(nameof(WriteMessage));
            OnPropertyChanged(nameof(LastMessageReceived));
        }
    }
}
