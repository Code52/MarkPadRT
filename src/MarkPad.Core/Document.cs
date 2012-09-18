using GalaSoft.MvvmLight;

namespace MarkPad.Core
{
    public abstract class Document : ViewModelBase
    {
        private string _text;
        private string _name;
        private string _originalText;

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

        public abstract string Id { get; }

        public string OriginalText
        {
            get { return _originalText; }
            set
            {
                _originalText = value;
                RaisePropertyChanged(() => IsModified);
            }
        }

        public ISource Source { get; set; }
        public bool IsModified
        {
            get
            {
                //This is ugly.
                var trimmedText = Text.Trim('\r', '\n').Replace("\n", "");
                var trimmedOrig = OriginalText.Trim('\r', '\n').Replace("\n", "");

                return trimmedText != trimmedOrig;
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