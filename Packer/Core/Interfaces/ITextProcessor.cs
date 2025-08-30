
namespace Packer.Core.Interfaces
{
	internal interface ITextProcessor
	{
		void Add(string path, string data);

		void Delete(string path);

		void Process();
	}
}
