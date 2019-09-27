using System;


namespace Estimotes {

    public class Beacon : IBeacon {

        readonly EstimoteSdk.Recognition.Packets.Beacon beacon;

        public Beacon(EstimoteSdk.Recognition.Packets.Beacon beacon) {
            this.beacon = beacon;
            this.Proximity = beacon.GetProximity();
        }
        public string Uuid => this.beacon.ProximityUUID.ToString();
        public ushort Major => (ushort)this.beacon.Major;
        public ushort Minor => (ushort)this.beacon.Minor;
		public Proximity Proximity { get; internal set; }
		internal DateTime LastPing { get; set; }
    }
}