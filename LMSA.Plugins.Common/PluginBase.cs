using System;

namespace lenovo.mbg.service.framework.services;

/// <summary>
/// Abstract base class for all LMSA plugins.
/// Provides default implementations for the IPlugin lifecycle methods.
/// </summary>
public abstract class PluginBase : IPlugin
{
    public static ILanguage? LangHelper;

    public abstract object CreateControl(IMessageBox iMessage);

    public virtual object? GetService(Type serviceType)
    {
        if (serviceType.IsAssignableFrom(GetType()))
        {
            return this;
        }
        return null;
    }

    public virtual void Dispose()
    {
    }

    public virtual bool CanClose()
    {
        return true;
    }

    public virtual bool IsExecuteWork()
    {
        return false;
    }

    public virtual void OnSelected(string val)
    {
    }

    public virtual void OnSelecting(string val)
    {
    }

    public abstract void Init();

    public virtual void OnInit(object data)
    {
    }

    public virtual bool IsNonBusinessPage()
    {
        return true;
    }
}
