using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkPad.Core;

namespace MarkPad.Sources.LocalFiles
{
    public class LocalFileSource : ISource
    {
        public async Task<bool> Login()
        {
            return true;
        }

        public IFile GetFile()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFile> GetFiles()
        {
            throw new NotImplementedException();
        }

        public void SaveFile(IFile file)
        {
            
        }

        public void SaveFiles(IEnumerable<IFile> files)
        {
            
        }
    }
}
