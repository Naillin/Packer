using Microsoft.Extensions.Logging;
using Packer.Core;
using Packer.Core.Models;
using System.Text;

namespace Packer.Workers
{
	internal class TextFileDetectorWorker : TextFileDetector
	{
		private readonly ILogger<TextFileDetectorWorker> _logger;

		public TextFileDetectorWorker(ILogger<TextFileDetectorWorker> logger) => _logger = logger;

		public override TextDetectionResult Detect(string path, TextDetectionOptions? options = null)
		{
			options ??= new TextDetectionOptions();

			string tempFile = Path.GetTempFileName();
			byte[] buffer;
			int read;
			try
			{
				// Создаём копию файла, чтобы не зависеть от блокировок
				File.Copy(path, tempFile, overwrite: true);

				using var fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				var sampleSize = (int)Math.Min(options.SampleSize, fs.Length);
				buffer = new byte[sampleSize];
				read = fs.Read(buffer, 0, sampleSize);
			}
			catch (Exception ex)
			{
				_logger.LogWarning($"Не удалось прочитать файл {path}: {ex.Message}");
				return new TextDetectionResult
				{
					IsText = false,
					Confidence = 0.0,
					DetectedEncoding = null,
					Reason = "Файл заблокирован или недоступен."
				};
			}
			finally
			{
				// Удаляем временный файл
				try { File.Delete(tempFile); } catch { /* игнорируем ошибки удаления */ }
			}

			var ext = Path.GetExtension(path) ?? string.Empty;
			var ctx = new HeuristicContext(buffer, read, ext, options);

			double score = 0.5; // базовый нейтральный приор
			var notes = new List<string>();
			Encoding? pickedEncoding = null;

			_logger.LogInformation($"Запуск эвристических методов.");
			foreach (var h in _heuristics)
			{
				var r = h.Apply(ctx);
				if (r != null)
				{
					score += r.ScoreDelta;
					notes.Add(r.Note);
					if (r.SuggestedEncoding != null) pickedEncoding = r.SuggestedEncoding;
				}
			}

			score = Math.Clamp(score, 0.0, 1.0);
			bool isText = score >= options.MinConfidence;

			// если кодировка ещё не определена — попробуем разумно догадаться
			pickedEncoding ??= GuessFallbackEncoding(ctx);

			// пустой файл считаем текстом
			if (read == 0)
			{
				isText = true;
				score = Math.Max(score, 0.75);
				notes.Add("Пустой файл.");
			}

			return new TextDetectionResult
			{
				IsText = isText,
				Confidence = score,
				DetectedEncoding = pickedEncoding,
				Reason = string.Join(" ", notes)
			};
		}
	}
}
