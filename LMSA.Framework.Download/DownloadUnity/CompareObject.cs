using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public class CompareObject<T> : IEqualityComparer<T>
{
	public bool Equals(T x, T y)
	{
		return ((Expression<Func<T, T, bool>>)((T val, T val2) => (object)val == (object)val2)).Compile()(x, y);
	}

	public int GetHashCode(T obj)
	{
		return obj?.GetHashCode() ?? 0;
	}
}
