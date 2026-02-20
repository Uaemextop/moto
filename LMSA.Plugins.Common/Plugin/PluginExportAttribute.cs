using System;

namespace lenovo.mbg.service.framework.services;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginExportAttribute : Attribute
{
    public Type ContractType { get; private set; }

    public string PluginId { get; private set; }

    public PluginExportAttribute(Type contractType, string pluginId)
    {
        ContractType = contractType;
        PluginId = pluginId;
    }
}
