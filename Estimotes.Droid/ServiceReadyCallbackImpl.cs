using System;
using EstimoteSdk;
using EstimoteSdk.Service;

namespace Estimotes {

    public class ServiceReadyCallbackImpl : Java.Lang.Object, BeaconManager.IServiceReadyCallback {
        readonly Action callback;


        public ServiceReadyCallbackImpl(Action callback) {
            this.callback = callback;
        }


        public void OnServiceReady() {
            this.callback();
        }
    }
}