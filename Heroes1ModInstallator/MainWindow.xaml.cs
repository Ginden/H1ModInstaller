using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using DiscUtils.Common;
using System.IO;
using DiscUtils.Iso9660;
using System.Diagnostics;
using DiscUtils;
using System.IO.Compression;

namespace Heroes1ModInstallator
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Pick_Mod(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Mod file (*.zip)|*.zip";
            dlg.DefaultExt = ".zip";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            if (result == null || result == false)
            {
                ModFilePath.Text = "";
            }
            else
            {
                ModFilePath.Text = dlg.FileName;
            }
        }

        private void Pick_Gog(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Obraz płyty H1|homm1.gog",
                DefaultExt = ".gog"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            if (result == null || result == false)
            {
                GogFilePath.Text = "";
            }
            else
            {
                GogFilePath.Text = dlg.FileName;
            }
        }

        private void Merge_Mod(object sender, RoutedEventArgs e)
        {
            if (GogFilePath.Text == null || GogFilePath.Text == "")
            {
                MessageBox.Show("GOG file path is empty!");
                return;
            }
            if (ModFilePath.Text == null || ModFilePath.Text == "")
            {
                MessageBox.Show("Mod file path is empty!");
                return;
            }
            this.IsEnabled = false;

            string windowsTempDirectory = System.IO.Path.GetTempPath();
            string uniqueDirectoryName = System.IO.Path.GetRandomFileName();
            string tempDirectory = Pathy.Combine(windowsTempDirectory, uniqueDirectoryName);
            Debug.WriteLine($"windowsTempDirectory={windowsTempDirectory}");
            Debug.WriteLine($"uniqueDirectoryName={uniqueDirectoryName}");
            Debug.WriteLine($"tempDirectory={tempDirectory}");

            Directory.CreateDirectory(tempDirectory);
            IsoExtractor.ExtractIso(GogFilePath.Text, tempDirectory);
            string backupName = $"{GogFilePath.Text}.{DateTime.Now.ToString("dd.MM.yyyy.hh.mm")}.backup";
            if (BackupState.IsChecked == true)
            {
                File.Copy(GogFilePath.Text, backupName);
            }

            using (FileStream zipToOpen = new FileStream(ModFilePath.Text, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    ZipArchiveExtensions.ExtractToDirectory(archive, tempDirectory);
                }
            }


            IsoFromDirectory.Create(tempDirectory, GogFilePath.Text);
            MessageBox.Show("Patching complete!");
            this.Close();
        }

    }
}
