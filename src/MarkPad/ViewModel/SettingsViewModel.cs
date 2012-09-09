using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using SharpDX.DirectWrite;

namespace MarkPad.ViewModel
{
    public class SettingsViewModel
    {
        public ObservableCollection<string> Fonts { get; set; }
        
        public SettingsViewModel()
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

            Fonts= new ObservableCollection<string>(x.OrderBy(y => y));
        }
    }
}
