using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Manga_Downloader.Classes
{
    class DownloadManager
    {
        // 3 Clients for 3 simulataneous download
        static WebClient[] clients = new WebClient[] {
            new WebClient(),
            new WebClient(),
            new WebClient()
        };

        public static void Download(Series series, int startIndex, int endIndex)
        {
            for (int i=0; i < clients.Length; i++)
                if (clients[i].IsBusy)
                {
                    System.Windows.Forms.MessageBox.Show("Please wait for current downloads to be finished.");
                    return;
                }

            // Create Base Folder
            System.IO.Directory.CreateDirectory(series.BaseFolder);
            // Create Chapter Folder
            for (int i = startIndex; i <= endIndex; i++)
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(series.BaseFolder, series.Chapters[i].Title));

            // Use a different thread to avoid stopping the main thread which leads to UI not being updated
            new Thread(() => {
                // Parse Chapters
                for (int i = startIndex; i <= endIndex; i++) Parser.ParseChapter(series.Chapters[i]);
                // Call each Client to download Chapters
                for (int i = 0; i < clients.Length; i++)
                {
                    new Thread(() => {
                        DownloadCallback(clients[i], series);
                    }).Start();
                    Thread.Sleep(250);
                }
            }).Start();
        }
        static void DownloadCallback(WebClient client, Series series)
        {
            for (int i = 0; i < series.Chapters.Count; i++)
                if (series.Chapters[i].Status == Chapter.ProgressStatus.PARSED)
                {
                    series.Chapters[i].Status = Chapter.ProgressStatus.DOWNLOADING;
                    for (int j = 0; j < series.Chapters[i].Images.Count; j++)
                    {
                        string link = series.Chapters[i].Images[j];
                        string[] array = link.Split('.');
                        string filename = j.ToString("D3") + "." + array[array.Length - 1];
                        string filePath = System.IO.Path.Combine(series.BaseFolder, series.Chapters[i].Title, filename);
                        client.DownloadFile(new Uri(series.Chapters[i].Images[j]), filePath);
                        //Console.WriteLine(series.Chapters[i].Title + "[" + j + "/" + series.Chapters[i].Images.Count + "]: " + filePath);
                        series.Chapters[i].Position += 1;
                    }
                    series.Chapters[i].Status = Chapter.ProgressStatus.DOWNLOADED;
                    DownloadCallback(client, series);
                    break;
                }
        }
    }
}
