using System;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.services.Model;

/// <summary>
/// Represents a single business data event with device context.
/// </summary>
public class BusinessData
{
    public string appType { get; set; } = string.Empty;
    public string connectType { get; set; } = string.Empty;
    public string androidVersion { get; set; } = string.Empty;
    public string modelName { get; set; } = string.Empty;
    public string useCaseStep { get; set; } = string.Empty;
    public long cycleTime { get; set; }
    public BusinessStatus status { get; set; }
    public string clientDate { get; set; } = string.Empty;
    public object? extraData { get; set; }

    public BusinessData()
    {
    }

    public BusinessData(BusinessType subBusiness, DeviceEx? device, BusinessStatus status = BusinessStatus.CLICK)
    {
        cycleTime = 0L;
        useCaseStep = subBusiness.ToString();
        this.status = status;
        appType = device?.ConnectedAppType ?? string.Empty;
        connectType = device?.ConnectType.ToString() ?? string.Empty;
        androidVersion = device?.Property?.AndroidVersion ?? string.Empty;
        modelName = device?.Property?.ModelName ?? string.Empty;
        clientDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public BusinessData Update(long useTime, BusinessStatus status, object? extraData)
    {
        return Update(useTime, status, null, extraData);
    }

    public BusinessData Update(long useTime, BusinessStatus status, string? modelName, object? extraData)
    {
        cycleTime = useTime / 1000;
        this.status = status;
        if (!string.IsNullOrEmpty(modelName))
        {
            this.modelName = modelName;
        }
        this.extraData = extraData;
        return this;
    }

    public static BusinessData Clone(BusinessData data)
    {
        return new BusinessData
        {
            appType = data.appType,
            connectType = data.connectType,
            androidVersion = data.androidVersion,
            useCaseStep = data.useCaseStep,
            status = data.status,
            clientDate = data.clientDate,
            extraData = data.extraData,
            modelName = data.modelName,
            cycleTime = data.cycleTime
        };
    }
}
