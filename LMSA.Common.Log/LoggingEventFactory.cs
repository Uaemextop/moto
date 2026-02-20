using System;
using System.Threading;
using log4net.Core;

namespace lenovo.mbg.service.common.log;

public class LoggingEventFactory : ILoggingEventFactory
{
	public LoggingEvent CreateEncryptedLoggingEvent(LoggingEvent loggingEvent, string encryptedLoggingMessage, string encryptedExceptionMessage = null)
	{
		if (loggingEvent == null)
		{
			throw new ArgumentNullException("source");
		}
		LoggingEventData loggingEventData = loggingEvent.GetLoggingEventData();
		loggingEventData.Message = encryptedLoggingMessage;
		if (!string.IsNullOrWhiteSpace(encryptedExceptionMessage))
		{
			loggingEventData.ExceptionString = encryptedExceptionMessage;
		}
		return new LoggingEvent(loggingEventData);
	}

	public LoggingEvent CreateErrorEvent(string ErrorMessage)
	{
		return new LoggingEvent(new LoggingEventData
		{
			Domain = "Software Fix",
			ExceptionString = ErrorMessage,
			Level = Level.Error,
			LoggerName = "lenovo.mbg.service.common.log.LogAesEncrypt",
			Message = "lenovo.mbg.service.common.log.LogAesEncrypt",
			ThreadName = Thread.CurrentThread.Name,
			TimeStampUtc = DateTime.Now.ToUniversalTime(),
			UserName = Environment.UserName
		});
	}
}
