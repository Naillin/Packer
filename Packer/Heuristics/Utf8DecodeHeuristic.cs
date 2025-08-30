using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;
using System.Text;

namespace Packer.Heuristics
{
	internal sealed class Utf8DecodeHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			// Если уже определили UTF-16 — пропускаем
			if ((ctx.Flags & (ContextFlags.UTF16LELike | ContextFlags.UTF16BELike)) != 0) return null;

			try
			{
				var utf8Strict = new UTF8Encoding(false, true);
				_ = utf8Strict.GetString(ctx.Buffer, 0, ctx.Read);
				return new HeuristicResult(+0.25, "Строгое декодирование UTF-8 прошло успешно.", Encoding.UTF8);
			}
			catch
			{
				return new HeuristicResult(-0.20, "Строгое декодирование UTF-8 не прошло — вероятно бинарный или иная кодировка.");
			}
		}
	}
}
