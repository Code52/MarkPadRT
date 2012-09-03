using System;
using System.Threading.Tasks;
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
    }
}
