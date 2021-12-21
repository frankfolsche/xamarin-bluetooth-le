﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Connectivity;
using Windows.Devices.Bluetooth;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;

namespace Plugin.BLE.UWP
{
    public class Device : DeviceBase
    {
        private readonly ObservableBluetoothLEDevice _nativeDevice;
        public override object NativeDevice => _nativeDevice;

        public Device(Adapter adapter, BluetoothLEDevice nativeDevice, int rssi, Guid id, IReadOnlyList<AdvertisementRecord> advertisementRecords = null) : base(adapter)
        {
            _nativeDevice = new ObservableBluetoothLEDevice(nativeDevice.DeviceInformation);
            Rssi = rssi;
            Id = id;
            Name = nativeDevice.Name;
            AdvertisementRecords = advertisementRecords;
        }

        internal void Update(short btAdvRawSignalStrengthInDBm, IReadOnlyList<AdvertisementRecord> advertisementData)
        {
            this.Rssi = btAdvRawSignalStrengthInDBm;
            this.AdvertisementRecords = advertisementData;
        }

        public override Task<bool> UpdateRssiAsync()
        {
            //No current method to update the Rssi of a device
            //In future implementations, maybe listen for device's advertisements

            Trace.Message("Request RSSI not supported in UWP");

            return Task.FromResult(true);
        }

        protected override async Task<IReadOnlyList<IService>> GetServicesNativeAsync()
        {
            var result = await _nativeDevice.BluetoothLEDevice.GetGattServicesAsync(BleImplementation.CacheModeGetServices);
            result.ThrowIfError();

            return result.Services?
                .Select(nativeService => new Service(nativeService, this))
                .Cast<IService>()
                .ToList();
        }

        protected override DeviceState GetState()
        {
            if (_nativeDevice.IsConnected)
            {
                return DeviceState.Connected;
            }

            return _nativeDevice.IsPaired ? DeviceState.Limited : DeviceState.Disconnected;
        }

        protected override Task<int> RequestMtuNativeAsync(int requestValue)
        {
            Trace.Message("Request MTU not supported in UWP");
            return Task.FromResult(-1);
        }

        protected override bool UpdateConnectionIntervalNative(ConnectionInterval interval)
        {
            Trace.Message("Update Connection Interval not supported in UWP");
            return false;
        }
    }
}
