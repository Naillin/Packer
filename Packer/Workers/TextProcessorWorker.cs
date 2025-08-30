using Microsoft.Extensions.Logging;
using Packer.Core;
using System.Reflection;

namespace Packer.Workers
{
	internal class TextProcessorWorker : TextProcessor
	{
		private string _pathToFile = "pack.txt";
		private string _exePath = Assembly.GetExecutingAssembly().Location;
		private string _baseInfo = string.Empty;
		private readonly LoggerManager _loggerTextProcessorWorker;

		public TextProcessorWorker(ILogger<TextProcessorWorker> logger)
		{
			if(File.Exists(_pathToFile))
				File.Delete(_pathToFile);
			//File.Create(_pathToFile);

			_baseInfo = $"Location: {_exePath}\r\n" +
						$"Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n" +
						Environment.NewLine;
			File.WriteAllText(_pathToFile, _baseInfo);

			_loggerTextProcessorWorker = new LoggerManager(logger, "TextProcessorWorker");
		}

		public override void Process()
		{
			if (File.Exists(_pathToFile))
			{
				_loggerTextProcessorWorker.LogInformation("Запись данных в файл.");
				using (StreamWriter writer = new StreamWriter(_pathToFile))
				{
					foreach (var item in _textData)
					{
						writer.WriteLine(item.Key);
						writer.WriteLine(item.Value);
						writer.Write(Environment.NewLine);
					}
				}
				_loggerTextProcessorWorker.LogInformation("Данные записаны.");
			}
		}
	}
}
