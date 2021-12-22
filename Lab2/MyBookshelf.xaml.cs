using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Syncfusion.Windows.PdfViewer;
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
using System.Windows.Shapes;
using Condition = Amazon.DynamoDBv2.Model.Condition;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for Reader.xaml
    /// </summary>
    public partial class MyBookshelf : Window
    {
        User user;
        List<Book> books;
        public MyBookshelf(User user)
        {
            this.user = user;
            InitializeComponent();
            usernameLabel.Content = user.username;
            GetAllBooks();
        }
        public async void GetAllBooks() {
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(MainWindow.GetCredentials(), RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            books = await context.ScanAsync<Book>(new List<ScanCondition>()).GetRemainingAsync();
            foreach (var book in books)
            {
                if (!user.bookmarks.Exists(x => x.name == book.Name)) 
                user.bookmarks.Add( new Bookmark { Book = book, bookmarked_page = 1 });
            }
            listBox.ItemsSource = null;
            listBox.ItemsSource = user.bookmarks;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Bookmark bookmark = ((FrameworkElement)sender).DataContext as Bookmark;
            Book book = books.FirstOrDefault(x => x.Name == bookmark.name);
            if (!user.bookmarks.Exists(x => book.Name == x.name)) {
                user.bookmarks.Add(new Bookmark { Book = book, bookmarked_page = 1 });
            }
            new MyPdfViewer(book.Url, user.bookmarks.FirstOrDefault(x => book.Name == x.name), user, GetAllBooks ).ShowDialog();
        }
    }
}
