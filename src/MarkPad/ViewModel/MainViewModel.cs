using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MarkPad.Extensions;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MarkPad.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
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
            Documents = new ObservableCollection<DocumentViewModel>
                {
                    new DocumentViewModel {Text = "hi", Name = "test file 1"},
                    new DocumentViewModel {Text = "world", Name = "test file 2"}
                };

            OpenCommand = new RelayCommand(() => Open());
        }

        private async Task Open()
        {
            var filepicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { ".md", ".mdown", ".markdown" }
            };

            var files = await filepicker.PickMultipleFilesAsync();
            foreach (StorageFile f in files)
            {
                var text = await f.ReadAllTextAsync();
                Documents.Add(new DocumentViewModel { Name = f.Name, Text = text });
            }
        }
    }
}