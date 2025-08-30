using Microsoft.Extensions.Logging;
using Packer.Core;
using Packer.Core.Interfaces;

namespace Packer.Workers
{
	internal class DirectoryWalkerWorker : DirectoryWalker
	{
		private ITextFileDetector _textFileDetector;
		private ITextProcessor _textProcessor;
		private readonly LoggerManager _loggerDirectoryWalkerWorker;
		private int _depth = -1;

		public DirectoryWalkerWorker(ITextFileDetector textFileDetector, ITextProcessor textProcessor, ILogger<DirectoryWalkerWorker> logger, string path = "", int depth = -1) : base(path)
		{
			_textFileDetector = textFileDetector;
			_textProcessor = textProcessor;
			_loggerDirectoryWalkerWorker = new LoggerManager(logger, "DirectoryWalkerWorker");
			_depth = depth;
		}
		
		public override void WalkOnDirectory()
		{
			TraverseDirectory(_directoryInfo);
			_textProcessor.Process();
		}

		private void TraverseDirectory(DirectoryInfo directoryInfo, int currentDepth = 0)
		{
			// Если превышена максимальная глубина, выходим
			if (_depth >= 0 && currentDepth > _depth)
				return;

			foreach (FileInfo file in directoryInfo.GetFiles())
			{
				if (IsIgnored(file))
				{
					_loggerDirectoryWalkerWorker.LogInformation($"Игнорирование файла: {file.FullName}");
					continue;
				}

				_loggerDirectoryWalkerWorker.LogInformation($"Файл: {file.FullName}");
				var statusFile = _textFileDetector.Detect(file.FullName);

				if (statusFile.IsText)
				{
					string data = "Пусто или не получилось прочесть содержимое!";
					try
					{
						data = File.ReadAllText(file.FullName);
					}
					catch (Exception ex)
					{
						_loggerDirectoryWalkerWorker.LogError($"Ошибка: {ex.Message}");
					}
					finally
					{
						_textProcessor.Add(file.FullName, data);
					}
				}
				else
				{
					_loggerDirectoryWalkerWorker.LogInformation($"Внимание: {statusFile.Reason}");
				}
			}

			foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
			{
				if (IsIgnored(directory))
				{
					_loggerDirectoryWalkerWorker.LogInformation($"Игнорирование папки: {directory.FullName}");
					continue;
				}

				_loggerDirectoryWalkerWorker.LogInformation($"Папка: {directory.FullName} (Глубина: {currentDepth})");
				TraverseDirectory(directory, currentDepth + 1);
			}
		}
	}
}
