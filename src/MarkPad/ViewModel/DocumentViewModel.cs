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
                RaisePropertyChanged(() => IsModified);
            }
        }

        public string OriginalText { get; private set; }

        public bool IsModified
        {
            get
            {
                //This is ugly.
                return Text.Trim('\r', '\n').Replace("\n", "") != OriginalText.Replace("\n", "");
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

        public DocumentViewModel(string originalText, string fileName = "New Document")
        {
            OriginalText = originalText;
            Text = originalText;
            Name = fileName;
        }
    }
}
