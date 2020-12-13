using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Manga_Downloader.Classes
{
    class Chapter : INotifyPropertyChanged
    {
        public enum ProgressStatus { PENDING, SKIPPED, PARSING, PARSED, DOWNLOADING, DOWNLOADED}
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Images { private set; get; }

        private string title;
        public string Title
        {
            private set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
            get { return title; }
        }

        private string link;
        public string Link {
            private set {
                link = value;
                RaisePropertyChanged("Link");
            }
            get { return link; }
        }

        private ProgressStatus status;
        public ProgressStatus Status
        {
            set
            {
                status = value;
                RaisePropertyChanged("Status");
                RaisePropertyChanged("Progress");
                RaisePropertyChanged("ProgressText");
            }
            get { return status; }
        }
        private int position;
        public int Position
        {
            set
            {
                position = value;
                RaisePropertyChanged("Position");
                RaisePropertyChanged("Progress");
                RaisePropertyChanged("ProgressText");
            }
            get { return position; }
        }

        public float Progress {
            get
            {
                if (Images.Count > 0) return (Position / (float) Images.Count) * 100;
                else return 0;
            }
        }
        public string ProgressText { get { return Position + "/" + Images.Count; } }

        public Chapter(string title, string link)
        {
            Title = title;
            Link = link;
            Images = new List<string>();
            Position = 0;
            Status = ProgressStatus.PENDING;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
