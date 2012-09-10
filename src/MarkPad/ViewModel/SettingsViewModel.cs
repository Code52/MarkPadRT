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
        private string _selectedFont;
        private double _fontSize;

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

        public SettingsViewModel()
        {
            LoadFonts();
            var defaultFont = "Segoe UI";
            var defaultFontSize = 36;

            _fontSize = defaultFontSize;
            _selectedFont = defaultFont;
        }

        private async Task LoadFonts()
        {
            var x = new List<string>();
            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var familyCount = fontCollection.FontFamilyCount;
            for (int i = 0; i < familyCount; i++)
            {
                var fontFamily = fontCollection.GetFontFamily(i);
                var familyNames = fontFamily.FamilyNames;
                int index;
                if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out index))
                    familyNames.FindLocaleName("en-us", out index);

                string name = familyNames.GetString(index);
                x.Add(name);
            }
            Fonts = new ObservableCollection<string>(x.OrderBy(y => y));
        }
    }
}
