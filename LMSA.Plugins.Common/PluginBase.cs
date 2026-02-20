using System.Windows;

namespace lenovo.mbg.service.framework.services;

public abstract class PluginBase : IPlugin
{
	public abstract void Init();

	public abstract FrameworkElement CreateControl(IMessageBox iMsg);

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

	public virtual void OnInit(object data)
	{
	}

	public virtual bool IsNonBusinessPage()
	{
		return false;
	}
}
