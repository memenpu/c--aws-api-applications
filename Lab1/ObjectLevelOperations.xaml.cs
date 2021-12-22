using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ObjectLevelOperations.xaml
    /// </summary>
    public partial class ObjectLevelOperations : Window
    {
        public ObjectLevelOperations()
        {
            InitializeComponent();
            GetBucketList();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem != null && comboBox.SelectedItem is S3Bucket bucket)
            {

                try
                {
                    string filePath = filePathTextBox.Text;
                    var key = filePath.Split('\\');
                    int l = key.Length;
                    string filename = key[l - 1];
                    uploadStatusLabel.Text = $"Uploading File \"{filename}\" to Bucket \"{bucket.BucketName}\"";
                    //putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");
                    using AmazonS3Client s3Client = new AmazonS3Client(MainWindow.GetCredentials(), RegionEndpoint.USEast1);
                    PutObjectResponse response2 = await s3Client.PutObjectAsync(new PutObjectRequest
                    {
                        BucketName = bucket.BucketName,
                        Key = filename,
                        FilePath = filePath,
                    }

                );
                    uploadStatusLabel.Text = $"Uploaded File \"{filename}\" to Bucket \"{bucket.BucketName}\"";
                    ComboBoxSelectionChanged(null, null);
                }
                catch (Exception ex)
                {
                    uploadStatusLabel.Text = $"Failed to Uploaded File to Bucket \"{bucket.BucketName}\"";
                    Console.WriteLine(
                        "Unknown encountered on server. Message:'{0}' when writing an object"
                        , ex.Message);
                }
            }
            
        }

            private async void GetBucketList()
        {
            using AmazonS3Client s3Client = new AmazonS3Client(MainWindow.GetCredentials(), RegionEndpoint.USEast1);
            ListBucketsResponse response = await s3Client.ListBucketsAsync();
            comboBox.ItemsSource = response.Buckets;
            comboBox.SelectedIndex = 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem != null && comboBox.SelectedItem is S3Bucket bucket)
            {
                using (AmazonS3Client s3Client = new AmazonS3Client(MainWindow.GetCredentials(), RegionEndpoint.USEast1))
                {
                  ListObjectsResponse response = await s3Client.ListObjectsAsync(bucket.BucketName);
                    objectsGrid.ItemsSource=response.S3Objects;
                }
            }
        }
    }
}
