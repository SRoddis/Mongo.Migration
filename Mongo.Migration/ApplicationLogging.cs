using Microsoft.Extensions.Logging;

public class ApplicationLogging
{
	private static ILoggerFactory _factory = null;

	public static void ConfigureLogger(ILoggerFactory factory)
	{
		factory.AddFile("logs/migration/log-migration.log");
	}

	/// <summary>
	/// Log Factory instance
	/// </summary>
	public static ILoggerFactory LoggerFactory
	{
		get
		{
			if (_factory == null)
			{
				_factory = new LoggerFactory();
				ConfigureLogger(_factory);
			}
			return _factory;
		}
	}

	/// <summary>
	/// Get a ILogger instance to be used in migrations 
	/// </summary>
	/// <returns>ILogger instance</returns>
	public static ILogger CreateLogger() => LoggerFactory.CreateLogger("logger");
}