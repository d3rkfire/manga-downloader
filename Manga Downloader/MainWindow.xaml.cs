using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Manga_Downloader.Classes;
using System.Threading;
using System.Windows.Threading;

namespace Manga_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Series series;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // TxtLink
            txtLink.GotFocus += (o, e) =>
            {
                if (txtLink.Text == "Please input link to the gallery page...")
                {
                    txtLink.Foreground = new BrushConverter().ConvertFrom("#FF000000") as Brush;
                    txtLink.Text = "";
                }
            };
            txtLink.LostFocus += (o, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtLink.Text))
                {
                    txtLink.Foreground = new BrushConverter().ConvertFrom("#FFAAAAAA") as Brush;
                    txtLink.Text = "Please input link to the gallery page...";
                }
            };
            txtLink.TextChanged += (o, e) =>
            {
                try
                {
                    series = Parser.ParseSeries(txtLink.Text);
                    SetVisibility(Visibility.Visible);
                    lblTitle.Content = series.Title;
                    lblChapters.Content = series.Chapters.Count + " chapters";
                    imgCover.Source = new BitmapImage(new Uri(series.Cover));
                    txtLocation.Text = series.BaseFolder;
                    // cbFirst
                    cbFirst.Items.Clear();
                    foreach (Chapter c in series.Chapters) cbFirst.Items.Add(c.Title);
                    cbFirst.SelectedIndex = 0;
                    // cbLast
                    cbLast.Items.Clear();
                    foreach (Chapter c in series.Chapters) cbLast.Items.Add(c.Title);
                    cbLast.SelectedIndex = cbLast.Items.Count - 1;
                    // dgvProgress
                    dgvProgress.ItemsSource = series.Chapters;
                }
                catch (Exception ex) {
                    SetVisibility(Visibility.Hidden);
                    Console.WriteLine(ex.Message);
                }
            };

            // btnChoose
            btnChoose.Click += (o, e) =>
            {
                if (series == null) return;
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    series.BaseFolder = System.IO.Path.Combine(fbd.SelectedPath, series.Title);
                    txtLocation.Text = series.BaseFolder;
                }
            };

            // btnDownload
            btnDownload.Click += (o, e) =>
            {
                DownloadManager.Download(series, cbFirst.SelectedIndex, cbLast.SelectedIndex);
            };

            // cbFirst
            cbFirst.SelectionChanged += (o, e) =>
            {
                for (int i = 0; i < cbFirst.SelectedIndex; i++) series.Chapters[i].Status = Chapter.ProgressStatus.SKIPPED;
                for (int i = cbFirst.SelectedIndex; i <= cbLast.SelectedIndex; i++) series.Chapters[i].Status = Chapter.ProgressStatus.PENDING;
            };
            // cbLast
            cbLast.SelectionChanged += (o, e) =>
            {
                for (int i = cbLast.SelectedIndex + 1; i < series.Chapters.Count; i++) series.Chapters[i].Status = Chapter.ProgressStatus.SKIPPED;
                for (int i = cbFirst.SelectedIndex; i <= cbLast.SelectedIndex; i++) series.Chapters[i].Status = Chapter.ProgressStatus.PENDING;
            };
        }

        void SetVisibility(Visibility v)
        {
            imgCover.Visibility = v;
            lblPreTitle.Visibility = v;
            lblTitle.Visibility = v;
            lblPreChapters.Visibility = v;
            lblChapters.Visibility = v;
            lblPreLocation.Visibility = v;
            txtLocation.Visibility = v;
            lblPreDownload.Visibility = v;
            cbFirst.Visibility = v;
            lblPreTo.Visibility = v;
            cbLast.Visibility = v;
            btnChoose.Visibility = v;
            btnDownload.Visibility = v;
            dgvProgress.Visibility = v;
        }
    }
}
