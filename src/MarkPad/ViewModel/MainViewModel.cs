using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MarkPad.Core;
using MarkPad.Sources.LocalFiles;

namespace MarkPad.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LocalSource _source = new LocalSource();
        private ObservableCollection<Document> _documents;
        private Document _selectedDocument;

        public ObservableCollection<Document> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                RaisePropertyChanged(() => Documents);
            }
        }

        public Document SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                RaisePropertyChanged(() => SelectedDocument);
            }
        }

        public ICommand OpenCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public MainViewModel()
        {
            Documents = new ObservableCollection<Document>();
            OpenCommand = new RelayCommand(() => Open());
            NewCommand = new RelayCommand(New);
            SaveCommand = new RelayCommand(() => _source.Save(SelectedDocument));
            CloseCommand = new RelayCommand<Document>((d) =>
                {
                    if (d.IsModified)
                        _source.Save(d);

                    Documents.Remove(d);
                });
            Load();

        }

        private void New()
        {
            _source.Restore();
            var doc = new LocalDocument(string.Empty);
            Documents.Add(doc);
            SelectedDocument = doc;
        }

        private async Task Load()
        {
          
            if (Documents.Count == 0)
                Documents.Add(new LocalDocument("") { Name = "New Doc" });
               
            SelectedDocument = Documents[0];
        }

        private async Task Open()
        {
            var files = await _source.Open();
            foreach (var f in files)
            {
                Documents.Add(f);
                SelectedDocument = f;
            }
        }
    }
}