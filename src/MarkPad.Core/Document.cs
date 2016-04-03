using System.ComponentModel;
using System.Runtime.CompilerServices;
using MarkPad.Core.Annotations;

namespace MarkPad.Core
{
	public abstract class Document : INotifyPropertyChanged
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
                OnPropertyChanged(Text);
                OnPropertyChanged("IsModified");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public abstract string Id { get; }

        public string OriginalText
        {
            get { return _originalText; }
            set
            {
                _originalText = value;
				OnPropertyChanged("IsModified");
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

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}