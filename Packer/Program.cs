using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Packer.Core.Interfaces;
using Packer.Workers;

namespace Packer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Парсим аргументы
			(string path, int depth) = parseArgs(args);

			// Создаем хост с DI контейнером
			var host = CreateHostBuilder(path, depth).Build();

			// Получаем сервис и запускаем
			var walker = host.Services.GetRequiredService<IDirectoryWalker>();
			walker.WalkOnDirectory();

			Console.ReadKey();
		}

		private static IHostBuilder CreateHostBuilder(string path = "", int depth = -1) => Host.CreateDefaultBuilder()
		.ConfigureServices((context, services) =>
		{
			// Регистрируем сервисы
			services.AddSingleton<ITextFileDetector, TextFileDetectorWorker>();
			services.AddSingleton<ITextProcessor, TextProcessorWorker>();

			services.AddSingleton<IDirectoryWalker>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var detector = provider.GetRequiredService<ITextFileDetector>();
				var processor = provider.GetRequiredService<ITextProcessor>();
				var logger = loggerFactory.CreateLogger<DirectoryWalkerWorker>();

				return new DirectoryWalkerWorker(detector, processor, logger, path, depth);
			});
		})
		.ConfigureLogging((context, logging) =>
		{
			logging.ClearProviders();

			if (OperatingSystem.IsLinux())
			{
				logging.AddSystemdConsole().SetMinimumLevel(LogLevel.Debug);
			}
			else
			{
				logging.AddConsole().SetMinimumLevel(LogLevel.Debug);
			}
		});


		private static (string, int) parseArgs(string[] args)
		{
			// Парсим аргументы
			string path = string.Empty;
			int depth = -1;
			var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var arg in args)
			{
				var parts = arg.Split('=', 2);
				if (parts.Length == 2)
				{
					dict[parts[0]] = parts[1];
				}
			}
			if (dict.TryGetValue("path", out var pathValue))
				path = pathValue;
			if (dict.TryGetValue("depth", out var depthValue) && int.TryParse(depthValue, out int d))
				depth = d;

			return (path, depth);
		}
	}
}
