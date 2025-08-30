using System.Text;

namespace Packer.Core.Models
{
	internal sealed class TextDetectionResult
	{
		public bool IsText { get; init; }
		public double Confidence { get; init; }      // [0..1]
		public Encoding? DetectedEncoding { get; init; }
		public string Reason { get; init; } = string.Empty;
	}
}
