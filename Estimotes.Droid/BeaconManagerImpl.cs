using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using EstimoteSdk;
using Android.Content;
using Android.Util;
using EstimoteSdk.Service;

namespace Estimotes
{

    public class BeaconManagerImpl : AbstractBeaconManagerImpl
    {
        const string DEBUG_TAG = "acrbeacons";
        readonly BeaconManager beaconManager;
        bool isConnected;


        public BeaconManagerImpl()
        {
            this.beaconManager = new BeaconManager(Application.Context);

            this.beaconManager.BeaconEnteredRegion += (sender, args) =>
            {
                Log.Debug(DEBUG_TAG, "EnteredRegion Event");
                var region = this.FromNative(args.Region);
                this.OnRegionStatusChanged(region, true);
            };
            this.beaconManager.BeaconExitedRegion += (sender, args) =>
            {
                Log.Debug(DEBUG_TAG, "ExitedRegion Event");
                var region = this.FromNative(args.Region);
                this.OnRegionStatusChanged(region, false);
            };
            this.beaconManager.BeaconRanging += (sender, args) =>
            {
                Log.Debug(DEBUG_TAG, "Ranging Event");
                var beacons = args.Beacons.Select(x => new Beacon(x)).ToList();
                if (beacons.Count > 0)
                    this.OnRanged(beacons);
            };
        }


        public void SetBackgroundScanPeriod(TimeSpan scanPeriod, TimeSpan waitTime)
        {
            this.beaconManager.SetBackgroundScanPeriod((long)scanPeriod.TotalMilliseconds, (long)waitTime.TotalMilliseconds);
        }


        public void SetForegroundScanPeriod(TimeSpan scanPeriod, TimeSpan waitTime)
        {
            this.beaconManager.SetForegroundScanPeriod((long)scanPeriod.TotalMilliseconds, (long)waitTime.TotalMilliseconds);
        }


        public override async Task<BeaconInitStatus> Initialize(bool backgroundMonitoring)
        {
            if (this.isConnected)
                return BeaconInitStatus.Success;

            //if (!this.beaconManager.HasBluetooth)
            //    return BeaconInitStatus.BluetoothMissing;

            //if (!this.beaconManager.IsBluetoothEnabled)
            //    return BeaconInitStatus.BluetoothOff;

            //if (!this.beaconManager.CheckPermissionsAndService())
            //return BeaconInitStatus.PermissionDenied;

            await this.Connect();

            // restore monitored beacons
            foreach (var region in this.MonitoringRegions)
                this.StartMonitoringNative(region);

            return BeaconInitStatus.Success;
        }



        protected virtual async Task Connect()
        {
            if (this.isConnected)
                return;

            var tcs = new TaskCompletionSource<object>();
            Application.Context.StartService(new Intent(Application.Context, typeof(EstimoteSdk.Service.BeaconService)));
            var ready = new ServiceReadyCallbackImpl(() =>
            {
                Log.Debug(DEBUG_TAG, "successfully connected to estimote service");
                this.isConnected = true;
                tcs.TrySetResult(null);
            });
            this.beaconManager.Connect(ready);
            await tcs.Task;
        }


        protected override void StartMonitoringNative(BeaconRegion region)
        {
            Log.Debug(DEBUG_TAG, "StartMonitoringNative");
            var native = this.ToNative(region);
            this.beaconManager.StartMonitoring(native);
        }


        protected override void StopMonitoringNative(BeaconRegion region)
        {
            var native = this.ToNative(region);
            this.beaconManager.StopMonitoring(native.Identifier);
        }


        protected override void StartRangingNative(BeaconRegion region)
        {
            var native = this.ToNative(region);
            this.beaconManager.StartRanging(native);
        }


        protected override void StopRangingNative(BeaconRegion region)
        {
            var native = this.ToNative(region);
            this.beaconManager.StopRanging(native);
        }


        protected virtual BeaconRegion FromNative(EstimoteSdk.Observation.Region.Beacon.BeaconRegion native)
        {
            return new BeaconRegion(
                native.Identifier,
                native.ProximityUUID.ToString(),
                (ushort?)native.Major,
                (ushort?)native.Minor
            );
        }

        protected virtual EstimoteSdk.Observation.Region.Beacon.BeaconRegion ToNative(BeaconRegion region)
        {
            EstimoteSdk.Observation.Region.Beacon.BeaconRegion native = null;

            if (region.Major > 0 && region.Minor > 0)
                native = new EstimoteSdk.Observation.Region.Beacon.BeaconRegion(region.Identifier, region.Uuid, region.Major.Value, region.Minor.Value);

            else if (region.Major > 0)
                native = new EstimoteSdk.Observation.Region.Beacon.BeaconRegion(region.Identifier, region.Uuid, region.Major.Value);

            else
                native = new EstimoteSdk.Observation.Region.Beacon.BeaconRegion(region.Identifier, region.Uuid);

            return native;
        }
    }
}