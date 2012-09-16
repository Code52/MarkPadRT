using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MarkPad.Extensions
{
    public static class StorageFileExtensions
    {
        async public static Task WriteAllTextAsync(this StorageFile storageFile, string content)
        {
            var inputStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
            var writeStream = inputStream.GetOutputStreamAt(0);
            var writer = new DataWriter(writeStream);
            writer.WriteString(content);
            await writer.StoreAsync();
            await writeStream.FlushAsync();
        }

        async public static Task<string> ReadAllTextAsync(this StorageFile storageFile)
        {
            var inputStream = await storageFile.OpenAsync(FileAccessMode.Read);
            var readStream = inputStream.GetInputStreamAt(0);
            var reader = new DataReader(readStream);
            uint fileLength = await reader.LoadAsync((uint)inputStream.Size);
            string content = reader.ReadString(fileLength);
            return content;
        }

        private readonly static string UTF8_BYTE_ORDER_MARK = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble(), 0, Encoding.UTF8.GetPreamble().Length);
        public static async Task<string> LoadAndRemoveBOM(this StorageFile storageFile)
        {
            IRandomAccessStream readStream = await storageFile.OpenAsync(FileAccessMode.Read);
            IInputStream inputSteam = readStream.GetInputStreamAt(0);
            DataReader dataReader = new DataReader(inputSteam);
            uint numBytesLoaded = await dataReader.LoadAsync((uint)readStream.Size);
            string output = dataReader.ReadString(numBytesLoaded);
            return output.Trim(UTF8_BYTE_ORDER_MARK.ToCharArray());
        }
    }
}
