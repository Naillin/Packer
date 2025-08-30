using System.Text;

namespace Packer.Core.Models
{
	internal sealed class HeuristicResult
	{
		public double ScoreDelta { get; }
		public string Note { get; }
		public Encoding? SuggestedEncoding { get; }

		public HeuristicResult(double delta, string note, Encoding? enc = null)
		{
			ScoreDelta = delta;
			Note = note;
			SuggestedEncoding = enc;
		}
	}
}
