using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Samsung.Android.Sdk.Healthdata;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Com.Samsung.Android.Sdk.Healthdata.HealthDataResolver;
using static Com.Samsung.Android.Sdk.Healthdata.HealthDataStore;

namespace TwoHearts
{
    public class ECGReporter
    {
        private readonly HealthDataStore store;
        private const long FiveMinutesInMillis = 5 * 60 * 1000L; 

        public ECGReporter(HealthDataStore store)
        {
            this.store = store;
        }

        public void Start()
        {
            HealthDataObserver.AddObserver(store, HealthConstants.Electrocardiogram.HealthDataType, new ECGObserver(ReadLastFiveMinutesECG));
        }

        private void ReadLastFiveMinutesECG()
        {
            var resolver = new HealthDataResolver(store, null);

            // Set time range from start time of today to the current time
            var startTime = DateTime.Now.Date.Ticks;
            var endTime = startTime + FiveMinutesInMillis;

            ReadRequestBuilder requestBuilder = new ReadRequestBuilder()
                .SetDataType(HealthConstants.Electrocardiogram.HealthDataType)
                .SetProperties(new[] { HealthConstants.Electrocardiogram.Data })
                .SetLocalTimeRange(HealthConstants.Electrocardiogram.StartTime, HealthConstants.Electrocardiogram.TimeOffset,
                    startTime, endTime);

            IReadRequest request = requestBuilder.Build();

            try
            {
                resolver.Read(request).SetResultListener(new ECGResultHolderResultListener());
            }
            catch (Exception)
            {
                // Getting step count fails.
            }
        }
    }

    public class ECGResultHolderResultListener : Java.Lang.Object, IDisposable, IJavaPeerable, IHealthResultHolderResultListener
    {
        public void OnResult(Java.Lang.Object resultObject)
        {
            if (resultObject is ReadResult result)
            {
                string data = "";

                try
                {
                    var iterator = result.Iterator();
                    while (iterator.HasNext)
                    {
                        // var data = (HealthData)iterator.Next();
                        data = HealthConstants.Electrocardiogram.Data;
                    }
                }
                finally
                {
                    result.Close();
                }

                System.Console.Write(data);
            }
        }
    }


    public class ECGObserver : HealthDataObserver
    {
        private readonly Action readLastFiveMinutesECG;

        private ECGObserver(Handler p0)
            : base(p0)
        {
        }

        public ECGObserver(Action readLastFiveMinutesECG)
            : this((Handler)null)
        {
            this.readLastFiveMinutesECG = readLastFiveMinutesECG;
        }

        public override void OnChange(string dataTypeName)
        {
            readLastFiveMinutesECG();
        }
    }
}