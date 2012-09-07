using System.Threading.Tasks;
using MarkPad.Core;
using MarkPad.Extensions;
using Windows.Storage;

namespace MarkPad.Sources.LocalFiles
{
    public class LocalDocument : Document
    {
        public StorageFile File { get; set; }

        public LocalDocument(StorageFile file)
        {
            Name = file.Name;
            File = file;
        }

        public async Task Load()
        {
            var t =await File.ReadAllTextAsync();

            OriginalText = t;
            Text = t;
        }

        public LocalDocument(string originalText, string fileName = "New Document")
            : base(originalText, fileName)
        {
        }
    }
}
