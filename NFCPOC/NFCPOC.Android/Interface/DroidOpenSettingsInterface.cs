using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NFCPOC.Droid.Interface
{
    public class DroidOpenSettingsInterface : NFCPOC.Interface.IOpenSettingsInterface
    {
        public void OpenNFCSettings()
        {
            try
            {
                Android.App.Application.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionNfcSettings));
            }
            catch (Exception exc)
            {
                Helpers.LoggerHelper.LogException(exc);
            }
        }
    }
}