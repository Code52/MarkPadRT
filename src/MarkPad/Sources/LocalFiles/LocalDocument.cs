using System;
using System.IO;
using System.Threading.Tasks;
using MarkPad.Core;
using MarkPad.Extensions;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace MarkPad.Sources.LocalFiles
{
    public class LocalDocument : Document
    {
        public StorageFile File { get; set; }
        public string Token { get; set; }
        public LocalDocument(StorageFile file)
        {
            Name = file.Name;
            File = file;
        }

        //Yes, this is really fugly, but issues with async stuff in the ctor
        public async Task Load()
        {
            var t = await File.LoadAndRemoveBOM();
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

        public static async Task<LocalDocument> FromPath(string path)
        {
            try
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(path, AccessCacheOptions.None);
                var d = new LocalDocument(file);
                await d.Load();
                return d;
            }catch(Exception o_0)
            {
                return null;
            }
        }


        public override string Id { get { return Token; } }
    }
}
