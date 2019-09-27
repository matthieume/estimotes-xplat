using System;
using EstimoteSdk;
using EstimoteSdk.Observation.Utils;

namespace Estimotes {

    public static class Extensions {

        public static Proximity GetProximity(this EstimoteSdk.Recognition.Packets.Beacon beacon) {
            var prox = EstimoteSdk.Observation.Region.RegionUtils.ComputeProximity(beacon);
            return prox.FromNative();
        }


        public static Proximity FromNative(this EstimoteSdk.Observation.Utils.Proximity prox) {
            if (prox == EstimoteSdk.Observation.Utils.Proximity.Far)
				return Proximity.Far;

            if (prox == EstimoteSdk.Observation.Utils.Proximity.Immediate)
			    return Proximity.Immediate;

            if (prox == EstimoteSdk.Observation.Utils.Proximity.Near)
				return Proximity.Near;

            return Proximity.Unknown;
        }
    }
}