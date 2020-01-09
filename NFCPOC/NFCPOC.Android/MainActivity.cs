using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Nfc;
using System.Text;

namespace NFCPOC.Droid
{
    [Activity(Label = Helpers.ConstantsHelper.APP_NAME, Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance)]
    [IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = Helpers.ConstantsHelper.DataMimeType)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, NfcAdapter.ICreateNdefMessageCallback, NfcAdapter.IOnNdefPushCompleteCallback
    {
        NfcAdapter mNfcAdapter;
        private const int MESSAGE_SENT = 1;

        public MainActivity()
        {
            mHandler = new MyHandler(HandlerHandleMessage);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            if (mNfcAdapter == null)
            {

            }
            else
            {
                // Register callback to set NDEF message
                mNfcAdapter.SetNdefPushMessageCallback(this, this);
                // Register callback to listen for message-sent success
                mNfcAdapter.SetOnNdefPushCompleteCallback(this, this);
            }

            // Plugin NFC: Initialization
            Plugin.NFC.CrossNFC.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Forms.DependencyService.Register<NFCPOC.Interface.IOpenSettingsInterface, NFCPOC.Droid.Interface.DroidOpenSettingsInterface>();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnNdefPushComplete(NfcEvent e)
        {
            try
            {
                // A handler is needed to send messages to the activity when this
                // callback occurs, because it happens from a binder thread
                mHandler.ObtainMessage(MESSAGE_SENT).SendToTarget();
            }
            catch (Exception exc)
            {
                Helpers.LoggerHelper.LogException(exc);
            }
        }

        public NdefMessage CreateNdefMessage(NfcEvent e)
        {
            try
            {
                DateTime time = DateTime.Now;
                var text = (Global.WriteMessage);

                if (string.IsNullOrEmpty(text))
                {
                    text = "Blank Message";
                }

                NdefMessage msg = new NdefMessage(
                new NdefRecord[] { CreateMimeRecord (
                 Helpers.ConstantsHelper.DataMimeType, Encoding.UTF8.GetBytes (text))
                });
                return msg;
            }
            catch (Exception exc)
            {
                Helpers.LoggerHelper.LogException(exc);
                return null;
            }
        }

        class MyHandler : Handler
        {
            public MyHandler(Action<Message> handler)
            {
                this.handle_message = handler;
            }

            Action<Message> handle_message;
            public override void HandleMessage(Message msg)
            {
                handle_message(msg);
            }
        }

        private readonly Handler mHandler;

        protected void HandlerHandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case MESSAGE_SENT:
                    Toast.MakeText(this.ApplicationContext, "Message sent!", ToastLength.Long).Show();
                    break;
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            // Check to see that the Activity started due to an Android Beam
            if (NfcAdapter.ActionNdefDiscovered == Intent.Action)
            {
                ProcessIntent(Intent);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            // onResume gets called after this to handle the intent
            Intent = intent;

            // Plugin NFC: Tag Discovery Interception
            Plugin.NFC.CrossNFC.OnNewIntent(intent);
        }

        void ProcessIntent(Intent intent)
        {
            try
            {
                IParcelable[] rawMsgs = intent.GetParcelableArrayExtra(
                    NfcAdapter.ExtraNdefMessages);
                // only one message sent during the beam
                NdefMessage msg = (NdefMessage)rawMsgs[0];
                // record 0 contains the MIME type, record 1 is the AAR, if present
                Global.ReadMessage = Encoding.UTF8.GetString(msg.GetRecords()[0].GetPayload());
            }
            catch (Exception exc)
            {
                Helpers.LoggerHelper.LogException(exc);
            }
        }

        public NdefRecord CreateMimeRecord(String mimeType, byte[] payload)
        {
            try
            {
                byte[] mimeBytes = Encoding.UTF8.GetBytes(mimeType);
                NdefRecord mimeRecord = new NdefRecord(
                    NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], payload);
                return mimeRecord;
            }
            catch (Exception exc)
            {
                Helpers.LoggerHelper.LogException(exc);
                return null;
            }
        }
    }
}