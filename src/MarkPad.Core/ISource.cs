using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkPad.Core
{
    public interface ISource
    {
        Task<bool> Login();
        IFile GetFile();
        IEnumerable<IFile> GetFiles();
        void SaveFile(IFile file);
        void SaveFiles(IEnumerable<IFile> files);
    }
}
