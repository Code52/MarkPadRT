using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;

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
            args.Request.ApplicationCommands.Add(new SettingsCommand("help", "Markdown Help", x =>
                {
                    Messenger.Default.Send(new HideWebviewMessage());
                    _flyout = new SettingsFlyout
                        {
                            HeaderText = "Markdown Syntax Help",
                            Content = new MarkdownSyntax(),
                            IsOpen = true,
                        };

                    _flyout.Closed += (s, e) => Messenger.Default.Send(new ShowWebViewMessage());
                }));
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
            d.Commands.Add(new UICommand("Close", command => tcs.SetResult(false)));
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
            IEnumerable<Document> files = await _source.Open();
            foreach (Document f in files)
            {
                Documents.Add(f);
                SelectedDocument = f;
            }
        }
    }
}