
namespace Packer.Core
{
	internal sealed class HeuristicContext
	{
		public byte[] Buffer { get; }
		public int Read { get; }
		public string Extension { get; }
		public TextDetectionOptions Options { get; }
		public ContextFlags Flags { get; set; }

		public HeuristicContext(byte[] buffer, int read, string ext, TextDetectionOptions options)
		{
			Buffer = buffer;
			Read = read;
			Extension = ext ?? string.Empty;
			Options = options;
		}
	}
}
