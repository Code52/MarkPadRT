using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SharpDX.DirectWrite;
using Windows.Storage;

namespace MarkPad.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always);
        private const string DefaultFont = "Segoe UI";
        private const double DefaultFontSize = 16.0;
        private double _fontSize;
        private string _selectedFont;
        private bool _distraction;

        public SettingsViewModel()
        {
            LoadFonts();
            SetDefaults();
            _selectedFont = (string)_localSettings.Values["Font"];
        }

        private void SetDefaults()
        {
            if (!_localSettings.Values.ContainsKey("FontSize"))
                _localSettings.Values.Add("FontSize", DefaultFontSize);

            if (!_localSettings.Values.ContainsKey("Font"))
                _localSettings.Values.Add("Font", DefaultFont);

            try
            {
                _fontSize = (double)_localSettings.Values["FontSize"];
            }
            catch
            {
                _localSettings.Values.Remove("FontSize");
                SetDefaults();
            }

            if (!_localSettings.Values.ContainsKey("Distraction"))
                _localSettings.Values.Add("Distraction", true);
        }

        public ObservableCollection<string> Fonts { get; set; }

        public string SelectedFont
        {
            get { return _selectedFont; }
            set
            {
                _selectedFont = value;
                _localSettings.Values["Font"] = value;
                RaisePropertyChanged(() => SelectedFont);
            }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                _localSettings.Values["FontSize"] = value;
                RaisePropertyChanged(() => FontSize);
            }
        }

        public bool Distraction
        {
            get
            {
                return (bool)_localSettings.Values["Distraction"];
            }
            set
            {
                _distraction = value;
                _localSettings.Values["Distraction"] = value;
                RaisePropertyChanged(() => Distraction);
            }
        }

        private void LoadFonts()
        {
            Task.Run(() =>
                {
                    var x = new List<string>();
                    var factory = new Factory();
                    FontCollection fontCollection = factory.GetSystemFontCollection(false);
                    int familyCount = fontCollection.FontFamilyCount;
                    for (int i = 0; i < familyCount; i++)
                    {
                        FontFamily fontFamily = fontCollection.GetFontFamily(i);
                        LocalizedStrings familyNames = fontFamily.FamilyNames;
                        int index;
                        if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out index))
                            familyNames.FindLocaleName("en-us", out index);

                        string name = familyNames.GetString(index);
                        x.Add(name);
                    }
                    Fonts = new ObservableCollection<string>(x.OrderBy(y => y));
                });
        }
    }
}