using System;
using System.Collections.Generic;
using MarkPad.Interfaces;

namespace MarkPad.Sources.LocalFiles
{
    public class LocalFileSource : ISource
    {
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
