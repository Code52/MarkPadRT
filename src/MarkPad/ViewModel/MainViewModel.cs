using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Callisto.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MarkPad.Core;
using MarkPad.Messages;
using MarkPad.Sources.LocalFiles;
using MarkPad.Views;
using MarkdownDeep;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.StartScreen;

namespace MarkPad.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const string Html = @"<html><head><style>body {{ background : #eaeaea; font-family: '{0}', sans-serif; font-size: {1}px; }}</style></head><body>{2}</body></html>";
        private readonly Markdown _markdown = new Markdown();
        private readonly LocalSource _source = new LocalSource();
        private ObservableCollection<Document> _documents;
        private SettingsFlyout _flyout;
        private Document _selectedDocument;

        public MainViewModel(SettingsViewModel settings)
        {
            Settings = settings;
            Distraction = true;
            Documents = new ObservableCollection<Document>();
            OpenCommand = new RelayCommand(() => Open());
            NewCommand = new RelayCommand(New);
            SaveCommand = new RelayCommand(() => _source.Save(SelectedDocument));
            CloseCommand = new RelayCommand<Document>(async d =>
                {
                    if (d.IsModified && await ShouldSave())
                        _source.Save(d);

                    _source.Close(d);
                    Documents.Remove(d);

                    if (Documents.Count == 0)
                        New();
                });
            PinCommand = new RelayCommand(() => PinDocument(SelectedDocument));
            Load();

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareRequested;
            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
        }

        public SettingsViewModel Settings { get; set; }

        public ObservableCollection<Document> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                RaisePropertyChanged(() => Documents);
            }
        }

        public bool Distraction { get; set; }

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
        public ICommand PinCommand { get; set; }

        public string Transform()
        {
            return string.Format(Html, Settings.SelectedFont, Settings.FontSize, _markdown.Transform(SelectedDocument.Text));
        }

        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var cmd = new SettingsCommand("settings", "Settings", x =>
                {
                    Messenger.Default.Send(new HideWebviewMessage());
                    _flyout = new SettingsFlyout
                        {
                            HeaderText = "Settings",
                            Content = new Settings(),
                            IsOpen = true,
                        };

                    _flyout.Closed += (s, e) => Messenger.Default.Send(new ShowWebViewMessage());
                });

            args.Request.ApplicationCommands.Add(cmd);
            args.Request.ApplicationCommands.Add(new SettingsCommand("help", "Markdown Help", x => ShowHelp()));
        }

        private async Task ShowHelp()
        {
            var doc = new LocalDocument(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Help.md")));
            await doc.Load();
            Open(doc);
        }

        private void ShareRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (SelectedDocument == null)
                return;

            DataRequest request = args.Request;
            request.Data.Properties.Title = SelectedDocument.Name;
            request.Data.SetText(SelectedDocument.Text);
            request.Data.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(_markdown.Transform(SelectedDocument.Text)));
        }

        private async Task<bool> ShouldSave()
        {
            var tcs = new TaskCompletionSource<bool>();

            var d = new MessageDialog("Do you want to save this file before closing?");
            d.Commands.Add(new UICommand("Yes", command => tcs.SetResult(true)));
            d.Commands.Add(new UICommand("No", command => tcs.SetResult(false)));
            d.DefaultCommandIndex = 0;
            d.CancelCommandIndex = 1;
            await d.ShowAsync();

            return await tcs.Task;
        }

        private void New()
        {
            var doc = new LocalDocument();
            Documents.Add(doc);
            SelectedDocument = doc;
        }

        private async Task Load()
        {
            foreach (Document d in await _source.Restore())
            {
                Documents.Add(d);
            }

            if (Documents.Count == 0)
                Documents.Add(new LocalDocument());

            SelectedDocument = Documents[0];
        }

        private async Task Open()
        {
            var files = await _source.Open();
            foreach (var f in files)
                Open(f);
        }

        public async Task Open(Document d)
        {
            if (Documents.Any(x => x.Name == d.Name && x.Id == d.Id))
                return;

            Documents.Add(d);
            SelectedDocument = d;
        }

        public async Task PinDocument(Document d)
        {
            string tileActivationArguments = d.Id;

            try
            {
                var secondaryTile = new SecondaryTile("AppSecondaryTile", d.Name, d.Name, tileActivationArguments, TileOptions.ShowNameOnLogo,  new Uri("ms-appx:///assets/Logo.png"));
                secondaryTile.RequestCreateAsync();
            }
            catch (Exception o_0)
            {

            }
        }
    }
}