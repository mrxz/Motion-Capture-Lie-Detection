using System;
using System.Collections.Concurrent;

namespace Motion_lie_detection
{
	/**
	 * The different log levels for the logger.
	 */
	public enum LogLevel
	{
		ERROR = 100,
		WARN = 200,
		INFO = 300,
		DEBUG = 400,
		FINE = 1000
	}

	public class Logger
	{

		/**
		 * The format to use when formatting log levels.
		 * [{timestamp}] {loglevel} {logger name}: {message}
		 */
		public static readonly String format = "[{0}] {1} {2}: {3}";
		/**
		 * The default log level to use for new loggers.
		 */
		public static readonly int defaultLogLevel = (int)LogLevel.INFO;
		/**
		 * Dictionary containing the mapping between the name of the logger and the instance.
		 * This allows multiple classes (or instances) to use loggers with the same name.
		 */
		public static ConcurrentDictionary<String, Logger> loggers = new ConcurrentDictionary<String, Logger> ();


		/**
		 * Name of the logger.
		 */
		public readonly String name;
		/**
		 * Log level of the logger, any message with a higher loglevel will be discarded.
		 */
		public int logLevel;

		private Logger (String name, int logLevel)
		{
			this.name = name;
			this.logLevel = logLevel;
		}

		public void log(LogLevel level, String message) {
			if ((int)level > logLevel)
				return;

			Console.Out.WriteLine (format, DateTime.UtcNow.ToString("HH:mm:ss.fff"), level, name, message);
		}

		public void error(String message) {
			log (LogLevel.ERROR, message);
		}

		public void warn(String message) {
			log (LogLevel.WARN, message);
		}

		public void info(String message) {
			log (LogLevel.INFO, message);
		}

		public void debug(String message) {
			log (LogLevel.DEBUG, message);
		}

		public void fine(String message) {
			log (LogLevel.FINE, message);
		}

		public static Logger getInstance(String name) {
			return loggers.GetOrAdd (name, new Logger (name, defaultLogLevel));
		}
	}
}

