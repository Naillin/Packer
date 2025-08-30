using Microsoft.Extensions.Logging;
using Packer.Core.Interfaces;
using Packer.Workers;

namespace Packer
{
	internal class Program
	{
		private static LoggerManager _loggerProgram;
		private static string path = string.Empty;
		private static int depth = -1;

		static void Main(string[] args)
		{
			ILoggerFactory loggerFactory;
			if (OperatingSystem.IsLinux()) 
			{
				loggerFactory = LoggerFactory.Create(builder =>
				{
					builder.AddSystemdConsole().SetMinimumLevel(LogLevel.Debug);
				});
			}
			else
			{
				loggerFactory = LoggerFactory.Create(builder =>
				{
					builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
				});
			}
				
			_loggerProgram = new LoggerManager(loggerFactory.CreateLogger<Program>(), "TextFileDetectorWorker");
			_loggerProgram.LogInformation("Starting.");

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

			ITextFileDetector textFileDetector = new TextFileDetectorWorker(loggerFactory.CreateLogger<TextFileDetectorWorker>());
			ITextProcessor textProcessor = new TextProcessorWorker(loggerFactory.CreateLogger<TextProcessorWorker>());
			
			IDirectoryWalker directoryWalker = new DirectoryWalkerWorker(textFileDetector, textProcessor, loggerFactory.CreateLogger<DirectoryWalkerWorker>(), path, depth);
			directoryWalker.WalkOnDirectory();
			_loggerProgram.LogInformation("Successfully completed.");

			Console.ReadKey();
		}
	}
}
