using GalaSoft.MvvmLight;

namespace MarkPad.ViewModel
{
    public class DocumentViewModel : ViewModelBase
    {
        private string _text;
        private string _name;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }
    }
}
