using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SharpDX.DirectWrite;

namespace MarkPad.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string DefaultFont = "Segoe UI";
        private const int DefaultFontSize = 16;
        private double _fontSize;
        private string _selectedFont;

        public SettingsViewModel()
        {
            LoadFonts();
            _fontSize = DefaultFontSize;
            _selectedFont = DefaultFont;
        }

        public ObservableCollection<string> Fonts { get; set; }

        public string SelectedFont
        {
            get { return _selectedFont; }
            set
            {
                _selectedFont = value;
                RaisePropertyChanged(() => SelectedFont);
            }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                RaisePropertyChanged(() => FontSize);
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