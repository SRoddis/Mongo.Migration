using Microsoft.Extensions.Logging;

public class ApplicationLogging
{
	private static ILoggerFactory _factory = null;

	public static void ConfigureLogger(ILoggerFactory factory)
	{
		factory.AddFile("logs/migration/log-migration.log");
	}

	/// <summary>
	/// Cria uma instancia do logFactory
	/// </summary>
	/// <param name="equipment">Objeto com os dados do equipamento</param>
	/// <returns>Instancia da logger Factory</returns>
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
	/// Retorna uma instancia do ILogger configurada para ser usada nas migrações
	/// </summary>
	/// <returns>Instancia do ILogger</returns>
	public static ILogger CreateLogger() => LoggerFactory.CreateLogger("logger");
}