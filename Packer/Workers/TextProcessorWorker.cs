using Microsoft.Extensions.Logging;
using Packer.Core;
using System.Reflection;

namespace Packer.Workers
{
	internal class TextProcessorWorker : TextProcessor
	{
		private string _pathToFile = "pack.txt";
		private string _exePath = System.AppContext.BaseDirectory;
		private string _baseInfo = string.Empty;
		private readonly ILogger<TextProcessorWorker> _logger;

		public TextProcessorWorker(ILogger<TextProcessorWorker> logger)
		{
			if(File.Exists(_pathToFile))
				File.Delete(_pathToFile);
			//File.Create(_pathToFile);

			_baseInfo = $"Location: {_exePath}\r\n" +
						$"Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n" +
						Environment.NewLine;
			File.WriteAllText(_pathToFile, _baseInfo);

			_logger = logger;
		}

		public override void Process()
		{
			if (File.Exists(_pathToFile))
			{
				_logger.LogInformation("Запись данных в файл.");
				using (StreamWriter writer = new StreamWriter(_pathToFile))
				{
					foreach (var item in _textData)
					{
						writer.WriteLine(item.Key);
						writer.WriteLine(item.Value);
						writer.Write(Environment.NewLine);
					}
				}
				_logger.LogInformation("Данные записаны.");
			}
		}
	}
}
