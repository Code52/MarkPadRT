using System.Collections.Generic;

namespace MarkPad.Interfaces
{
    public interface ISource
    {
        IFile GetFile();
        IEnumerable<IFile> GetFiles();
        void SaveFile(IFile file);
        void SaveFiles(IEnumerable<IFile> files);
    }
}
