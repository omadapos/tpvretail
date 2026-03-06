using OposScale_CCO;
using OposScanner_CCO;
using System;

namespace OmadaPOS.Services;

public class ZebraScannerService
{
    private OPOSScannerClass? scanner;
    private OPOSScaleClass? scale;

    public event Action<string>? OnBarcodeDataReceived;
    public event Action<string, double>? OnWeightUpdated;

    public ZebraScannerService()
    {
        scanner = new OPOSScannerClass();
        scale = new OPOSScaleClass();
    }

    public string Initialize()
    {
        var sResult = "";

        scanner?.Open("ZEBRA_SCANNER");
        scanner?.ClaimDevice(1000);

        scale?.Open("ZEBRA_SCALE");
        scale?.ClaimDevice(1000);

        if (scanner.Claimed)
        {
            scanner.DataEvent += Scanner_DataEvent;
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
        }
        else
        {
            sResult = "Failed to connect to any scanner.";
        }

        if (scale.CapStatusUpdate)
        {
            scale.StatusNotify = (int)OPOSScaleConstants.SCAL_SN_ENABLED;
            if (scale.ResultCode == (int)OPOSConstants.OPOS_SUCCESS)
            {
                scale.StatusUpdateEvent += StatusUpdateEvent;
                scale.DeviceEnabled = true;
                if (scale.DeviceEnabled)
                {
                    scale.DataEventEnabled = true;
                    scale.DataEventEnabled = false;
                }
                else
                {
                    sResult = "Failed to enable the scale. Error code: " + scale.ResultCode;
                }
            }
        }
        else
        {
            sResult = "Failed to connect to any scale.";
        }

        return sResult;
    }

    private void Scanner_DataEvent(int Status)
    {
        scanner.DataEventEnabled = true;
        OnBarcodeDataReceived?.Invoke(scanner.ScanDataLabel);
    }

    private void StatusUpdateEvent(int value)
    {
        //int status = (int)scale.ResultCode;
        string weightStatus = string.Empty;

        switch (value)
        {
            case (int)OPOSScaleConstants.SCAL_SUE_STABLE_WEIGHT:
                weightStatus = WeightFormat(scale.ScaleLiveWeight);
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNSTABLE:
                weightStatus = ".......................";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_ZERO:
                weightStatus = WeightFormat(scale.ScaleLiveWeight);
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_OVERWEIGHT:
                weightStatus = "Weight limit exceeded.";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_NOT_READY:
                weightStatus = "Scale not ready.";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNDER_ZERO:
                weightStatus = "Scale under zero weight.";
                break;

            default:
                weightStatus = $"Unknown status [{value}]";
                break;
        }

        OnWeightUpdated?.Invoke(weightStatus, scale.ScaleLiveWeight);
    }

    private string WeightFormat(int weight)
    {
        string units = UnitAbbreviation(scale.WeightUnits);
        return string.IsNullOrEmpty(units) ? "Unknown weight unit" : $"{0.001 * weight:0.000} {units}";
    }

    private string UnitAbbreviation(int units)
    {
        switch(units)
        {
            case (int)OPOSScaleConstants.SCAL_WU_GRAM:
                return "gr.";
            case (int)OPOSScaleConstants.SCAL_WU_KILOGRAM:
                return "Kg.";
            case (int)OPOSScaleConstants.SCAL_WU_OUNCE:
                return "oz.";
            case (int)OPOSScaleConstants.SCAL_WU_POUND:
                return "lb.";
            default:
                return string.Empty;
        }
    }

    public void Close()
    {
        scanner.DataEvent -= Scanner_DataEvent;
        scanner.DataEventEnabled = false;
        scanner.ReleaseDevice();
        scanner.Close();

        scale.StatusUpdateEvent -= StatusUpdateEvent;
        scale.DataEventEnabled = false;
        scale.ReleaseDevice();
        scale.Close();
    }
}