using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MarkPad.Extensions;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace MarkPad.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private ObservableCollection<DocumentViewModel> _documents;
        private DocumentViewModel _selectedDocument;

        public ObservableCollection<DocumentViewModel> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                RaisePropertyChanged(() => Documents);
            }
        }

        public DocumentViewModel SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                RaisePropertyChanged(() => SelectedDocument);
            }
        }

        public ICommand OpenCommand { get; set; }

        public MainViewModel()
        {
            Documents = new ObservableCollection<DocumentViewModel>();
            OpenCommand = new RelayCommand(() => Open());
            Load();

        }

        private async Task Load()
        {
            foreach (var f in _localSettings.Values)
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)f.Value);
                var text = await file.ReadAllTextAsync();
                Documents.Add(new DocumentViewModel { Name = file.Name, Text = text });
            }
        }

        public async Task Open(StorageFile f)
        {
            var text = await f.ReadAllTextAsync();
            Documents.Add(new DocumentViewModel { Name = f.Name, Text = text });
            string token = StorageApplicationPermissions.FutureAccessList.Add(f, f.Name);
            _localSettings.Values[f.Name] = token;
        }

        private async Task Open()
        {
            var filepicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { ".md", ".mdown", ".markdown" }
            };

            var files = await filepicker.PickMultipleFilesAsync();
            foreach (var file in files)
                Open(file);
        }
    }
}