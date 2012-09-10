using System.IO;
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

        //Yes, this is really fugly, but issues with async stuff in the ctor
        public async Task Load()
        {
            var t = await File.ReadAllTextAsync();
            OriginalText = t;
            Text = t;
        }

        public LocalDocument()
            : base("", "New Document")
        {

        }
        public LocalDocument(string originalText, string fileName = "New Document")
            : base(originalText, fileName)
        {
        }

        public override string Id { get { return Path.Combine(File.Path, File.Name); } }
    }
}
