using GalaSoft.MvvmLight;
using Legross.Audio;

namespace Legross.ChatClient
{
    public class NoteViewModel : ViewModelBase
    {
        public NoteViewModel(Note note, string displayName)
        {
            this.Note = note;
            this.DisplayName = displayName;
        }

        public Note Note { get; set; }
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    RaisePropertyChanged("Selected");
                }
            }
        }
        public string DisplayName { get; set; }
    }
}
