using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;

namespace api_gateway.services
{
	public interface ILogService
	{
		public void Log(string endpoint, object request);
	}
	public class LogService : ILogService
	{
		private readonly ILogger<LogService> _logService;
		private static object _lock = new object();

		public LogService(ILogger<LogService> logService) { _logService = logService; }


		public void Log(string endpoint, object request)
		{
			lock (_lock)
			{
				string fullFilePath = Path.Combine(".", DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
				var n = Environment.NewLine;
				string exc = "";
				exc = n + endpoint + ": " + request + n;
				File.AppendAllText(fullFilePath, LogLevel.Information + ": " + DateTime.Now.ToString() + " " + n + exc);
			}
		}
	}
}
