using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for myPdfViewer.xaml
    /// </summary>
    public partial class MyPdfViewer : Window
    {

        public MyPdfViewer(string bookurl, Bookmark bookmark, User user, Action updateListBox)
        {
            InitializeComponent();
            WebClient client = new();
            byte[] myDataBuffer = client.DownloadData(new Uri(bookurl));
            MemoryStream storeStream = new MemoryStream();
            storeStream.SetLength(myDataBuffer.Length);
            storeStream.Write(myDataBuffer, 0, (int)storeStream.Length);
            pdfViewer.Load(storeStream);
            pdfViewer.CurrentPage = bookmark.bookmarked_page;
            storeStream.Flush();
            Closing += async (sender, e) =>
            {
                using var client = new AmazonDynamoDBClient(MainWindow.GetCredentials(), RegionEndpoint.CACentral1);
                using var context = new DynamoDBContext(client);
                user.bookmarks.Remove(user.bookmarks.FirstOrDefault(x => bookmark.name == x.name));
                bookmark.bookmarked_page = pdfViewer.CurrentPage;
                user.bookmarks.Insert(0, bookmark);
                await context.SaveAsync(user);
                updateListBox.Invoke();
            };

        }




    }
}
