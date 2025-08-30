using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;
using System.Text;

namespace Packer.Heuristics
{
	internal sealed class LanguageHintsHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			// Лёгкие маркеры языков/разметки для .sql/.xml/.cpp/.cs и др.
			var sampleLen = Math.Min(ctx.Read, 4096);
			string asciiPreview = SafeAsciiPreview(ctx.Buffer, sampleLen);

			int bump = 0;
			void Hit(string what) { bump++; }

			if (asciiPreview.Contains("<?xml") || asciiPreview.Contains("<xml")) Hit("xml");
			if (asciiPreview.Contains("#include") || asciiPreview.Contains("//") || asciiPreview.Contains("/*")) Hit("c/cpp");
			if (asciiPreview.Contains("using ") || asciiPreview.Contains("namespace ")) Hit("csharp/cpp");
			if (asciiPreview.Contains("CREATE TABLE") || asciiPreview.Contains("INSERT INTO") || asciiPreview.Contains("SELECT ")) Hit("sql");
			if (asciiPreview.Contains("{") && asciiPreview.Contains("}")) Hit("curly");

			if (bump >= 2) return new HeuristicResult(+0.10, "Найдены характерные маркеры языков/разметки.");
			if (bump == 1) return new HeuristicResult(+0.05, "Обнаружен маркер текста/кода.");
			return null;

			static string SafeAsciiPreview(byte[] buf, int len)
			{
				var sb = new StringBuilder(len);
				for (int i = 0; i < len; i++)
				{
					byte b = buf[i];
					if (b == 0x09 || b == 0x0A || b == 0x0D || (b >= 0x20 && b <= 0x7E))
						sb.Append((char)b);
					else
						sb.Append(' ');
				}
				return sb.ToString();
			}
		}
	}
}
