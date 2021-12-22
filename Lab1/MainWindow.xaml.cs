using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
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
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateBucketButton_Click(object sender, RoutedEventArgs e)
        {
            new CreateBucket().ShowDialog();
        }
        public static BasicAWSCredentials GetCredentials()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);

            var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;
            return new BasicAWSCredentials(accessKeyID, secretKey);
        }

        private void ObjectLevelOperationsButton_Click(object sender, RoutedEventArgs e)
        {
            new ObjectLevelOperations().ShowDialog();
        }
    }
}
