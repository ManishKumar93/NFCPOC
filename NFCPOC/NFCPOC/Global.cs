using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace NFCPOC
{
    public class Global
    {
        private static string _writeMessage = "";
        public static string WriteMessage
        {
            get => _writeMessage;
            set
            {
                _writeMessage = value;
            }
        }

        private static string _readMessage = "";
        public static string ReadMessage
        {
            get => _readMessage;
            set
            {
                _readMessage = value;
                ReadOn = DateTime.Now;
                MessagingCenter.Send(ReadMessage, "ReadMessage");
            }
        }

        public static DateTime ReadOn { get; set; } = DateTime.MinValue;

        public static bool IsSubscribed { get; set; } = false;
    }
}
