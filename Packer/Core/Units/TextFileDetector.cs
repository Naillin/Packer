using Packer.Core.Interfaces;
using Packer.Core.Models;
using Packer.Heuristics;
using System.Text;

namespace Packer.Core
{
	internal abstract class TextFileDetector : ITextFileDetector
	{
		protected readonly List<IHeuristic> _heuristics = new()
		{
			new ExtensionPriorHeuristic(),
			new BomHeuristic(),
			new Utf16PatternHeuristic(),
			new NullByteHeuristic(),
			new ControlCharHeuristic(),
			new Utf8DecodeHeuristic(),
			new PrintableRatioHeuristic(),
			new LineBreakHeuristic(),
			new LanguageHintsHeuristic(),
		};

		public abstract TextDetectionResult Detect(string path, TextDetectionOptions? options = null);

		protected static Encoding GuessFallbackEncoding(HeuristicContext ctx)
		{
			if (ctx.Flags.HasFlag(ContextFlags.UTF16LELike)) return Encoding.Unicode;       // UTF-16LE
			if (ctx.Flags.HasFlag(ContextFlags.UTF16BELike)) return Encoding.BigEndianUnicode; // UTF-16BE

			// Попробуем UTF-8 без BOM — самый безопасный дефолт для современных файлов
			try
			{
				var utf8Strict = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
				_ = utf8Strict.GetString(ctx.Buffer, 0, ctx.Read);
				return Encoding.UTF8;
			}
			catch { /* ignore */ }

			// На Windows для "ANSI" разумно отдать текущую однобайтовую OEM/ACP
			return Encoding.Default;
		}
	}
}
