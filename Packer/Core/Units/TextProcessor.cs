using Packer.Core.Interfaces;

namespace Packer.Core
{
	internal abstract class TextProcessor : ITextProcessor
	{
		public TextProcessor() { }

		protected Dictionary<string, string> _textData = new Dictionary<string, string>();

		public void Add(string path, string data)
		{
			_textData.Add(path, data);
		}

		public void Delete(string path)
		{
			_textData.Remove(path);
		}

		public abstract void Process();
	}
}
