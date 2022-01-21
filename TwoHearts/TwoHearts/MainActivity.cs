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

namespace TwoHearts
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    // [Activity(Label = "WearTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : WearableActivity
    {
        
        private static MainActivity Instance = null;
        private HealthDataStore Store;
        private HealthConnectionErrorResult ConnError;
        private HashSet<PermissionKey> KeySet;

        public HealthDataStore.IConnectionListener ConnectionListener { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            Instance = this;
            KeySet = new HashSet<PermissionKey>();
            KeySet.Add(new PermissionKey(HealthConstants.Electrocardiogram.HealthDataType, PermissionType.Read));
            // Create a HealthDataStore instance and set its listener
            Store = new HealthDataStore(this, ConnectionListener);
            // Request the connection to the health data store
            Store.ConnectService();

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
            result.Text = string.Format("ECG length = {0}", ecg.Length);
        }
    }
}



