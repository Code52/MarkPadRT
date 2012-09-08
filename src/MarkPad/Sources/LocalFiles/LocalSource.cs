using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkPad.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace MarkPad.Sources.LocalFiles
{
    public class LocalSource : ISource
    {
        readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public async Task<bool> Login()
        {
            return true;
        }

        public async Task<IEnumerable<Document>> Open()
        {
            var docs = new List<Document>();
            var filepicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.List,
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    FileTypeFilter = { ".md", ".mdown", ".markdown", ".mkd" }
                };

            var files = await filepicker.PickMultipleFilesAsync();
            foreach (var file in files)
            {
                var d = new LocalDocument(file);
                await d.Load();
                docs.Add(d);

                string token = StorageApplicationPermissions.FutureAccessList.Add(file, file.Name);
                _localSettings.Values[file.Name] = token;
            }

            return docs;
        }

        public async Task<IEnumerable<Document>> Restore()
        {
            var docs = new List<Document>();
            foreach (var f in _localSettings.Values)
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)f.Value);
                var d = new LocalDocument(file);
                await d.Load();
                docs.Add(d);
            }
            return docs;
        }

        public async Task Save(Document document)
        {
            var doc = (LocalDocument)document;
            if (!doc.IsModified)
                return;

            StorageFile file;
            if (doc.File == null)
            {
                var filepicker = new FileSavePicker
                    {
                        DefaultFileExtension = ".md",
                        CommitButtonText = "Save",
                        SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    };
                filepicker.FileTypeChoices.Add("Markdown", new List<string> { ".md", ".mdown", ".markdown", ".mkd" });
                file = await filepicker.PickSaveFileAsync();
                doc.File = file;

                string token = StorageApplicationPermissions.FutureAccessList.Add(file, file.Name);
                _localSettings.Values[file.Name] = token;
            }
            else
                file = doc.File;

            if (file != null)
            {
                doc.Name = file.Name;
                await FileIO.WriteTextAsync(file, doc.Text);
                doc.OriginalText = doc.Text.Trim('\r', '\n');
            }
        }

        public void Close(Document document)
        {
            try
            {
                var doc = (LocalDocument)document;
                var token = _localSettings.Values[doc.File.Name];
                StorageApplicationPermissions.FutureAccessList.Remove((string)token);
                _localSettings.Values.Remove(doc.File.Name);
            }
            catch (Exception o_0)
            {

            }
        }
    }
}