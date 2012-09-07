using GalaSoft.MvvmLight;

namespace MarkPad.Core
{
    public abstract class Document : ViewModelBase
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
                
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string OriginalText { get; set; }
        public ISource Source { get; set; }
        public bool IsModified
        {
            get
            {
                //This is ugly.
                return Text.Trim('\r', '\n').Replace("\n", "") != OriginalText.Replace("\n", "");
            }
        }

        protected Document()
        {
            
        }

        protected Document(string originalText, string fileName = "New Document")
        {
            OriginalText = originalText;
            Text = originalText;
            Name = fileName;
        }
    }
}