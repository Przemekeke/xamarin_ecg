using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Com.Samsung.Android.Sdk.Healthdata;
using Android.Text;
using static Com.Samsung.Android.Sdk.Healthdata.HealthPermissionManager;
using System.Collections.Generic;
using Android.Runtime;
using TwoHearts;
using Java.Interop;

namespace TwoHearts
{

    [Activity(Label = "@string/app_name", MainLauncher = true)]
    // [Activity(Label = "WearTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : WearableActivity
    {
        
        private static MainActivity Instance = null;
        private HealthDataStore Store;
        private readonly HealthConnectionErrorResult ConnError;
        private HashSet<PermissionKey> KeySet;

        public ConnectionListener ConnectionListener { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            Instance = this;
            KeySet = new HashSet<PermissionKey>
            {
                new PermissionKey(HealthConstants.Electrocardiogram.HealthDataType, PermissionType.Read)
            };

            Button button = FindViewById<Button>(Resource.Id.click_button);
            button.Click += StartMeassurement;
            SetAmbientEnabled();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Store.DisconnectService();
        }

        private void StartMeassurement(object sender, EventArgs e)
        {
            TextView status = FindViewById<TextView>(Resource.Id.status);
            TextView result = FindViewById<TextView>(Resource.Id.status);

            status.SetTextColor(Android.Graphics.Color.Green);
            status.Text = string.Format("Meassurement started");

            var ecg = HealthConstants.Electrocardiogram.Data;
            result.Text = string.Format("ECG length = {0}", ecg);

            var connectionListener = new ConnectionListener();
            Store = new HealthDataStore(this, connectionListener);
            connectionListener.Store = Store; // This is the important line
            Store.ConnectService();
        }
    }

    public class ConnectionListener : Java.Lang.Object, IDisposable, IJavaPeerable, HealthDataStore.IConnectionListener
    {
        internal HealthDataStore Store { get;  set; }

        public void OnConnected()
        {
            //Log(APP_TAG, "Health data service is connected.");
            HealthPermissionManager pmsManager = new HealthPermissionManager(Store);
            var ecgReporter = new ECGReporter(Store);
            ecgReporter.Start();

            try
            {
                // Check whether the permissions that this application needs are acquired
                // Request the permission for reading step counts if it is not acquired

                // Get the current step count and display it if data permission is required
                // ...
            }
            catch (Exception e)
            {
                System.Console.WriteLine(string.Format("Exception Cought {0}\n Permission setting fails.", e.Message));
            }
        }

        public void OnConnectionFailed(HealthConnectionErrorResult p0)
        {
            // Health data service is not available.
        }
        public void OnDisconnected()
        {
            Store.DisconnectService();
        }
    }
}



