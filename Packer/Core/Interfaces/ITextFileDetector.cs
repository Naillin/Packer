using Packer.Core.Models;

namespace Packer.Core.Interfaces
{
	internal interface ITextFileDetector
	{
		TextDetectionResult Detect(string path, TextDetectionOptions? options = null);
	}
}
