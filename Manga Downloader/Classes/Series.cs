using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Manga_Downloader.Classes
{
    class Series
    {
        public string Title { private set; get; }
        public string Cover { private set; get; }
        public string BaseFolder { set; get; }
        public ObservableCollection<Chapter> Chapters { private set; get; }

        public Series(string title, string cover, ObservableCollection<Chapter> chapters)
        {
            Title = title;
            Cover = cover;
            BaseFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Title);
            Chapters = chapters;
        }
    }
}