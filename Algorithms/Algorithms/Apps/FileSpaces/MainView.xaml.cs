using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Algorithms.Apps.FileSpaces
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private int currentDirectoryDepth = 0;

        public MainView()
        {
            InitializeComponent();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            var x = Scan(@"C:\", null);
            foreach (var fileExtendedInfo in x)
            {
                if (fileExtendedInfo.IsDirectory)
                    Console.WriteLine(fileExtendedInfo.DirectorySize);
                else
                    Console.WriteLine(fileExtendedInfo.FileSize);
            }
        }

        public List<FileExtendedInfo> Scan(string dir, FileExtendedInfo parent)
        {
            try
            {
                var fis = Directory.GetFileSystemEntries(dir);
                var extInfos = new List<FileExtendedInfo>();
                foreach (var sub in fis)
                {
                    var fi = new FileInfo(sub);
                    var ei = new FileExtendedInfo();
                    if (fi.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        ei.DirectoryName = fi.FullName;
                        ei.FileName = null;
                        ei.IsDirectory = true;

                        //Console.WriteLine("(D) " + ei.DirectoryName);

                        currentDirectoryDepth++;
                        var subs = Scan(ei.DirectoryName, ei);
                        currentDirectoryDepth--;
                        ei.Children = subs;
                    }
                    else
                    {
                        ei.DirectoryName = fi.DirectoryName;
                        ei.FileName = fi.Name;
                        ei.FileSize = fi.Length;
                        ei.IsDirectory = false;
                        ei.Parent = parent;

                        // add the size to dir size
                        ei.Parent.DirectorySize += ei.FileSize;

                        //Console.WriteLine("(F) " + ei.FileName + " (D) " + ei.DirectoryName);
                    }
                }
                return extInfos;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }
    }
}
