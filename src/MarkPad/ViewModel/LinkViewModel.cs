using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkPad.Common;

namespace MarkPad.ViewModel
{
    public  class LinkViewModel : BindableBase
    {
        private string _displayText;
        private string _linkAddress;

        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                if (value == _displayText) return;
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public string LinkAddress
        {
            get { return _linkAddress; }
            set
            {
                if (value == _linkAddress) return;
                _linkAddress = value;
                OnPropertyChanged();
            }
        }
    }
}
