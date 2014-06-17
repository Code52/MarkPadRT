using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Callisto.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HtmlAgilityPack;
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
		readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always);

		private const string Html = @"<html><head><style>body {{ background : #eaeaea; font-family: '{0}', sans-serif; font-size: {1}px; }}</style></head><body>{2}</body></html>";
		private readonly Markdown _markdown = new Markdown();
		private readonly LocalSource _source = new LocalSource();
		private ObservableCollection<Document> _documents;
		private SettingsFlyout _flyout;
		private Document _selectedDocument;

		public MainViewModel(SettingsViewModel settings)
		{
			Settings = settings;
			Documents = new ObservableCollection<Document>();
			OpenCommand = new RelayCommand(() => Open());
			NewCommand = new RelayCommand(New);
			SaveCommand = new RelayCommand(() => _source.Save(SelectedDocument));
			CloseCommand = new RelayCommand(async () =>
				{
					if (SelectedDocument == null)
						return;

					var d = SelectedDocument;
					if (d.IsModified && await ShouldSave())
						_source.Save(d);

					_source.Close(d);
					Documents.Remove(d);

					if (Documents.Count == 0)
						New();
					else
						SelectedDocument = Documents.Last();
				});
			PinCommand = new RelayCommand<Document>(document => PinDocument(document), CanExecute);
			Load();

			DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
			dataTransferManager.DataRequested += ShareRequested;
			SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
		}

		private bool CanExecute(Document d)
		{
			if (d == null)
				return false;

			return ((LocalDocument)SelectedDocument).File != null;
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

		public bool Distraction { get { return Settings.Distraction; } set { Settings.Distraction = value; } }

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

		/// <summary>
		/// Transform given Markdown text into HTML.
		/// </summary>
		/// <returns></returns>
		public string Transform()
		{
			return Transform(false);
		}

		/// <summary>
		/// Transform given Markdown text into HTML.
		/// </summary>
		/// <param name="openHttpAsExternal">True, if open IE.</param>
		/// <returns></returns>
		public string Transform(bool openHttpAsExternal)
		{
			var result = string.Format(Html, Settings.SelectedFont, Settings.FontSize, _markdown.Transform(SelectedDocument.Text));
			return !openHttpAsExternal ? result : injectScriptNotifyToHttpLink(result);
		}

		/// <summary>
		/// Replace http url link to ScriptNotify container in order to open IE in preview webview.
		/// </summary>
		/// <param name="content">A html content</param>
		/// <returns></returns>
		private static string injectScriptNotifyToHttpLink(string content)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(content);
			var headNode = doc.DocumentNode.FirstChild.FirstChild; // head tag
			if (headNode.Name.Equals("head"))
				Debug.WriteLine("head");

			// insert javascript function
			HtmlNode scriptElement = doc.CreateElement("script");
			scriptElement.Attributes.Add("type", "text/javascript");
			const string scriptContent = @"function gotoLink(args){ window.external.notify('url:' + args); return false;}";
			scriptElement.AppendChild(doc.CreateComment(scriptContent));
			headNode.AppendChild(scriptElement);

			// replace all a-tag. http://stackoverflow.com/a/23966320/361100
			foreach (HtmlNode node in doc.DocumentNode.FirstChild.Element("body").Descendants("a"))
			{
				if (node.Name.Equals("a"))
				{
					HtmlAttribute att = node.Attributes.FirstOrDefault(l => l.Name.Equals("href"));
					if (att != null)
					{
						node.Attributes.Add("onClick", String.Format("gotoLink('{0}');", att.Value));
						att.Value = "#";
					}
				}
			}
			return doc.DocumentNode.OuterHtml;
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
			args.Request.ApplicationCommands.Add(new SettingsCommand("about", "About", x =>
				{
					Messenger.Default.Send(new HideWebviewMessage());
					_flyout = new SettingsFlyout
					{
						HeaderText = "About",
						Content = new AboutView(),
						IsOpen = true,
					};

					_flyout.Closed += (s, e) => Messenger.Default.Send(new ShowWebViewMessage());
				}));
			args.Request.ApplicationCommands.Add(new SettingsCommand("privacy", "Privacy", x =>
			{
				Messenger.Default.Send(new HideWebviewMessage());
				_flyout = new SettingsFlyout
				{
					HeaderText = "Privacy",
					Content = new PrivacyView(),
					IsOpen = true,
				};

				_flyout.Closed += (s, e) => Messenger.Default.Send(new ShowWebViewMessage());
			}));
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
			if (!_localSettings.Values.ContainsKey("HasRun"))
			{
				_localSettings.Values.Add("HasRun", true);
				ShowHelp();
			}

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
			var existing = Documents.FirstOrDefault(x => x.Name == d.Name && x.Id == d.Id);
			if (existing != null)
			{
				SelectedDocument = existing;
				return;
			}
			Documents.Add(d);
			SelectedDocument = d;
		}

		public async Task PinDocument(Document d)
		{
			string tileActivationArguments = d.Id;

			try
			{

				var secondaryTile = new SecondaryTile(d.Name, d.Name, d.Name, tileActivationArguments, TileOptions.ShowNameOnLogo, new Uri("ms-appx:///assets/PinnedLogo.png"));
				secondaryTile.RequestCreateAsync();
			}
			catch (Exception o_0)
			{

			}
		}
	}
}