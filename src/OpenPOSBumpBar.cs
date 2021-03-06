﻿
namespace POS.AltCCOInterop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.PointOfService;
    using POS.Devices;

    [ServiceObject(DeviceType.BumpBar, "OpenPOS BumpBar", "OPOS BumpBar Alternative CCO Interop", 1, 14)]

    public class OpenPOSBumpBar : BumpBar, ILegacyControlObject, IDisposable
    {
        private POS.Devices.OPOSBumpBar _cco = null;
        private const string _oposDeviceClass = "BumpBar";
        private string _oposDeviceName = "";
        private int _binaryConversion = 0;

        #region Event handler management variable
        public override event DataEventHandler DataEvent;
        public override event DirectIOEventHandler DirectIOEvent;
        public override event DeviceErrorEventHandler ErrorEvent;
        public override event StatusUpdateEventHandler StatusUpdateEvent;
        public override event OutputCompleteEventHandler OutputCompleteEvent;
        #endregion

        #region Constructor, Destructor
        public OpenPOSBumpBar()
        {
            _cco = null;
            _oposDeviceName = "";
            _binaryConversion = 0;
        }

        ~OpenPOSBumpBar()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: Discard the managed state (managed object).
                }

                if (_cco != null)
                {
                    _cco.DataEvent -= (_IOPOSBumpBarEvents_DataEventEventHandler)_cco_DataEvent;
                    _cco.DirectIOEvent -= (_IOPOSBumpBarEvents_DirectIOEventEventHandler)_cco_DirectIOEvent;
                    _cco.ErrorEvent -= (_IOPOSBumpBarEvents_ErrorEventEventHandler)_cco_ErrorEvent;
                    _cco.OutputCompleteEvent -= (_IOPOSBumpBarEvents_OutputCompleteEventEventHandler)_cco_OutputCompleteEvent;
                    _cco.StatusUpdateEvent -= (_IOPOSBumpBarEvents_StatusUpdateEventEventHandler)_cco_StatusUpdateEvent;
                    _cco = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

        #region Utility subroutine

        /// <summary>
        /// Check the processing result value of OPOS and generate a PosControlException exception if it is an error.
        /// </summary>
        /// <param name="value">OPOS method return value or ResultCode property value</param>
        private void VerifyResult(int value)
        {
            if (value != (int)ErrorCode.Success)
            {
                ErrorCode eValue = (ErrorCode)InteropEnum<ErrorCode>.ToEnumFromInteger(value);
                throw new Microsoft.PointOfService.PosControlException((_oposDeviceClass + ":" + _oposDeviceName), eValue, _cco.ResultCodeExtended);
            }
        }

        #endregion

        #region Process of relaying OPOS event and generating POS for.NET event

        private void _cco_DataEvent(int Status)
        {
            if (this.DataEvent != null)
            {
                DataEvent(this, new DataEventArgs(Status));
            }
        }

        private void _cco_DirectIOEvent(int EventNumber, ref int pData, ref string pString)
        {
            if (this.DirectIOEvent != null)
            {
                DirectIOEventArgs eDE = new DirectIOEventArgs(EventNumber, pData, pString);
                DirectIOEvent(this, eDE);
                pData = eDE.Data;
                pString = Convert.ToString(eDE.Object);
            }
        }

        private void _cco_ErrorEvent(int ResultCode, int ResultCodeExtended, int ErrorLocus, ref int pErrorResponse)
        {
            if (this.ErrorEvent != null)
            {
                ErrorCode eCode = (ErrorCode)InteropEnum<ErrorCode>.ToEnumFromInteger(ResultCode);
                ErrorLocus eLocus = (ErrorLocus)InteropEnum<ErrorLocus>.ToEnumFromInteger(ErrorLocus);
                ErrorResponse eResponse = (ErrorResponse)InteropEnum<ErrorResponse>.ToEnumFromInteger(pErrorResponse);
                DeviceErrorEventArgs eEE = new DeviceErrorEventArgs(eCode, ResultCodeExtended, eLocus, eResponse);
                ErrorEvent(this, eEE);
                pErrorResponse = (int)eEE.ErrorResponse;
            }
        }

        private void _cco_OutputCompleteEvent(int OutputID)
        {
            if (this.OutputCompleteEvent != null)
            {
                OutputCompleteEvent(this, new OutputCompleteEventArgs(OutputID));
            }
        }

        private void _cco_StatusUpdateEvent(int Data)
        {
            if (this.StatusUpdateEvent != null)
            {
                StatusUpdateEvent(this, new StatusUpdateEventArgs(Data));
            }
        }

        #endregion

        #region ILegacyControlObject member

        public BinaryConversion BinaryConversion
        {
            get
            {
                _binaryConversion = _cco.BinaryConversion;
                return (BinaryConversion)InteropEnum<BinaryConversion>.ToEnumFromInteger(_binaryConversion);
            }
            set
            {
                _cco.BinaryConversion = (int)value;
                _binaryConversion = _cco.BinaryConversion;
                VerifyResult(_cco.ResultCode);
            }
        }

        public string ControlObjectDescription
        {
            get { return _cco.ControlObjectDescription; }
        }

        public Version ControlObjectVersion
        {
            get { return InteropCommon.ToVersion(_cco.ControlObjectVersion); }
        }

        #endregion

        #region Device common properties

        public override bool CapCompareFirmwareVersion
        {
            get { return _cco.CapCompareFirmwareVersion; }
        }

        public override PowerReporting CapPowerReporting
        {
            get { return (PowerReporting)InteropEnum<PowerReporting>.ToEnumFromInteger(_cco.CapPowerReporting); }
        }

        public override bool CapStatisticsReporting
        {
            get { return _cco.CapStatisticsReporting; }
        }

        public override bool CapUpdateFirmware
        {
            get { return _cco.CapUpdateFirmware; }
        }

        public override bool CapUpdateStatistics
        {
            get { return _cco.CapUpdateStatistics; }
        }

        public override string CheckHealthText
        {
            get { return _cco.CheckHealthText; }
        }

        public override bool Claimed
        {
            get { return _cco.Claimed; }
        }

        public override int DataCount
        {
            get { return _cco.DataCount; }
        }

        public override bool DataEventEnabled
        {
            get
            {
                return _cco.DataEventEnabled;
            }
            set
            {
                _cco.DataEventEnabled = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override bool DeviceEnabled
        {
            get
            {
                return _cco.DeviceEnabled;
            }
            set
            {
                _cco.DeviceEnabled = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override bool FreezeEvents
        {
            get
            {
                return _cco.FreezeEvents;
            }
            set
            {
                _cco.FreezeEvents = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override int OutputId
        {
            get { return _cco.OutputID; }
        }

        public override PowerNotification PowerNotify
        {
            get
            {
                return (PowerNotification)InteropEnum<PowerNotification>.ToEnumFromInteger(_cco.PowerNotify);
            }
            set
            {
                _cco.PowerNotify = (int)value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override PowerState PowerState
        {
            get { return (PowerState)InteropEnum<PowerState>.ToEnumFromInteger(_cco.PowerState); }
        }

        public override ControlState State
        {
            get { return (ControlState)InteropEnum<ControlState>.ToEnumFromInteger(_cco.State); }
        }

        public override string ServiceObjectDescription
        {
            get { return _cco.ServiceObjectDescription; }
        }

        public override Version ServiceObjectVersion
        {
            get { return InteropCommon.ToVersion(_cco.ControlObjectVersion); }
        }

        public override string DeviceDescription
        {
            get { return _cco.DeviceDescription; }
        }

        public override string DeviceName
        {
            get { return _cco.DeviceName; }
        }

        #endregion

        #region Device common method

        public override void Open()
        {
            if (string.IsNullOrWhiteSpace(_oposDeviceName))
            {
                try
                {
                    _oposDeviceName = GetConfigurationProperty("OposDeviceName");
                    _oposDeviceName.Trim();
                }
                catch
                {
                    _oposDeviceName = "";
                }
            }

            if (string.IsNullOrWhiteSpace(_oposDeviceName))
            {
                string strMessage = "OposDeviceName is not configured on " + DevicePath + ".";
                throw new Microsoft.PointOfService.PosControlException(strMessage, ErrorCode.NoExist);
            }

            if (_cco == null)
            {
                try
                {
                    // CCO object CreateInstance
                    _cco = new POS.Devices.OPOSBumpBar();

                    // Register event handler
                    _cco.DataEvent += new _IOPOSBumpBarEvents_DataEventEventHandler(_cco_DataEvent);
                    _cco.DirectIOEvent += new _IOPOSBumpBarEvents_DirectIOEventEventHandler(_cco_DirectIOEvent);
                    _cco.ErrorEvent += new _IOPOSBumpBarEvents_ErrorEventEventHandler(_cco_ErrorEvent);
                    _cco.OutputCompleteEvent += new _IOPOSBumpBarEvents_OutputCompleteEventEventHandler(_cco_OutputCompleteEvent);
                    _cco.StatusUpdateEvent += new _IOPOSBumpBarEvents_StatusUpdateEventEventHandler(_cco_StatusUpdateEvent);
                }
                catch
                {
                    string strMessage = "Can not create Common ControlObject on " + DevicePath + ".";
                    throw new Microsoft.PointOfService.PosControlException(strMessage, ErrorCode.Failure);
                }
            }

            VerifyResult(_cco.Open(_oposDeviceName));
        }

        public override void Close()
        {
            VerifyResult(_cco.Close());

            _cco.DataEvent -= (_IOPOSBumpBarEvents_DataEventEventHandler)_cco_DataEvent;
            _cco.DirectIOEvent -= (_IOPOSBumpBarEvents_DirectIOEventEventHandler)_cco_DirectIOEvent;
            _cco.ErrorEvent -= (_IOPOSBumpBarEvents_ErrorEventEventHandler)_cco_ErrorEvent;
            _cco.OutputCompleteEvent -= (_IOPOSBumpBarEvents_OutputCompleteEventEventHandler)_cco_OutputCompleteEvent;
            _cco.StatusUpdateEvent -= (_IOPOSBumpBarEvents_StatusUpdateEventEventHandler)_cco_StatusUpdateEvent;
            _cco = null;
        }

        public override void Claim(int timeout)
        {
            VerifyResult(_cco.ClaimDevice(timeout));
        }

        public override void Release()
        {
            VerifyResult(_cco.ReleaseDevice());
        }

        public override string CheckHealth(HealthCheckLevel level)
        {
            VerifyResult(_cco.CheckHealth((int)level));
            return _cco.CheckHealthText;
        }

        public override void ClearInput()
        {
            VerifyResult(_cco.ClearInput());
        }

        public override void ClearOutput()
        {
            VerifyResult(_cco.ClearOutput());
        }

        public override DirectIOData DirectIO(int command, int data, object obj)
        {
            var intValue = data;
            var stringValue = Convert.ToString(obj);
            VerifyResult(_cco.DirectIO(command, ref intValue, ref stringValue));
            return new DirectIOData(intValue, stringValue);
        }

        public override CompareFirmwareResult CompareFirmwareVersion(string firmwareFileName)
        {
            int result;
            VerifyResult(_cco.CompareFirmwareVersion(firmwareFileName, out result));
            return (CompareFirmwareResult)InteropEnum<CompareFirmwareResult>.ToEnumFromInteger(result);
        }

        public override void UpdateFirmware(string firmwareFileName)
        {
            VerifyResult(_cco.UpdateFirmware(firmwareFileName));
        }

        public override void ResetStatistic(string statistic)
        {
            VerifyResult(_cco.ResetStatistics(statistic));
        }

        public override void ResetStatistics(string[] statistics)
        {
            VerifyResult(_cco.ResetStatistics(string.Join(",", statistics)));
        }

        public override void ResetStatistics(StatisticCategories statistics)
        {
            VerifyResult(_cco.ResetStatistics(Enum.GetName(typeof(StatisticCategories), statistics)));
        }

        public override void ResetStatistics()
        {
            VerifyResult(_cco.ResetStatistics(""));
        }

        public override string RetrieveStatistic(string statistic)
        {
            var result = statistic;
            VerifyResult(_cco.RetrieveStatistics(ref result));
            return result;
        }

        public override string RetrieveStatistics(string[] statistics)
        {
            var result = string.Join(",", statistics);
            VerifyResult(_cco.RetrieveStatistics(ref result));
            return result;
        }

        public override string RetrieveStatistics(StatisticCategories statistics)
        {
            var result = Enum.GetName(typeof(StatisticCategories), statistics);
            VerifyResult(_cco.RetrieveStatistics(ref result));
            return result;
        }

        public override string RetrieveStatistics()
        {
            var result = "";
            VerifyResult(_cco.RetrieveStatistics(ref result));
            return result;
        }

        public override void UpdateStatistic(string name, object value)
        {
            VerifyResult(_cco.UpdateStatistics(name + "=" + value));
        }

        public override void UpdateStatistics(StatisticCategories statistics, object value)
        {
            VerifyResult(_cco.UpdateStatistics(Enum.GetName(typeof(StatisticCategories), statistics) + "=" + value));
        }

        public override void UpdateStatistics(Statistic[] statistics)
        {
            VerifyResult(_cco.UpdateStatistics(InteropCommon.ToStatisticsString(statistics)));
        }

        #endregion

        #region OPOSBumpBar  Specific Properties

        public override bool AsyncMode
        {
            get
            {
                return _cco.AsyncMode;
            }
            set
            {
                _cco.AsyncMode = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override int AutoToneDuration
        {
            get
            {
                return _cco.AutoToneDuration;
            }
            set
            {
                _cco.AutoToneDuration = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override int AutoToneFrequency
        {
            get
            {
                return _cco.AutoToneFrequency;
            }
            set
            {
                _cco.AutoToneFrequency = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override int BumpBarDataCount
        {
            get { return _cco.BumpBarDataCount; }
        }

        public override bool CapTone
        {
            get { return _cco.CapTone; }
        }

        public override DeviceUnits CurrentUnitId
        {
            get
            {
                return (DeviceUnits)InteropEnum<DeviceUnits>.ToEnumFromInteger(_cco.CurrentUnitID);
            }
            set
            {
                _cco.CurrentUnitID = (int)value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override string ErrorString
        {
            get { return _cco.ErrorString; }
        }

        public override DeviceUnits ErrorUnits
        {
            get { return (DeviceUnits)InteropEnum<DeviceUnits>.ToEnumFromInteger(_cco.ErrorUnits); }

            //get { return (DeviceUnits)cco.ErrorUnits; }
        }

        public override string EventString
        {
            get { return _cco.EventString; }
        }

        public override DeviceUnits EventUnitId
        {
            get { return (DeviceUnits)InteropEnum<DeviceUnits>.ToEnumFromInteger(_cco.EventUnitID); }
        }

        public override DeviceUnits EventUnits
        {
            get { return (DeviceUnits)InteropEnum<DeviceUnits>.ToEnumFromInteger(_cco.EventUnits); }

            //get { return (DeviceUnits)cco.EventUnits; }
        }

        public override int Keys
        {
            get { return _cco.Keys; }
        }

        public override int Timeout
        {
            get
            {
                return _cco.Timeout;
            }
            set
            {
                _cco.Timeout = value;
                VerifyResult(_cco.ResultCode);
            }
        }

        public override DeviceUnits UnitsOnline
        {
            get { return (DeviceUnits)InteropEnum<DeviceUnits>.ToEnumFromInteger(_cco.UnitsOnline); }

            //get { return (DeviceUnits)cco.UnitsOnline; }
        }

        #endregion

        #region OPOSBumpBar  Specific Methodss

        public override void BumpBarSound(DeviceUnits units, int frequency, int duration, int numberOfCycles, int interSoundWait)
        {
            VerifyResult(_cco.BumpBarSound((int)units, frequency, duration, numberOfCycles, interSoundWait));
        }

        public override void SetKeyTranslation(DeviceUnits units, int scanCode, int logicalKey)
        {
            VerifyResult(_cco.SetKeyTranslation((int)units, scanCode, logicalKey));
        }

        #endregion
    }
}
