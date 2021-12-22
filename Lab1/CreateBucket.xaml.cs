using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for CreateBucket.xaml
    /// </summary>
    public partial class CreateBucket : Window
    {
        public CreateBucket()
        {
            InitializeComponent();
            GetBucketList();
        }
        private async void GetBucketList()
        {
            using (AmazonS3Client s3Client = new AmazonS3Client(MainWindow.GetCredentials(), RegionEndpoint.USEast1))
            {
                ListBucketsResponse response = await s3Client.ListBucketsAsync();
                bucketsGrid.ItemsSource = response.Buckets;
            }
        }

        private async void CreateBucketButton_Click(object sender, RoutedEventArgs e)
        {
            using (AmazonS3Client s3Client = new AmazonS3Client(MainWindow.GetCredentials(), RegionEndpoint.USEast1))
            {
                bucketsGrid.ItemsSource = null;
                try
                {
                    errorLabel.Content = $"Creating Bucket: \"{textBox.Text}\"";
                    await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = textBox.Text });
                    errorLabel.Content = $"Created Bucket: \"{textBox.Text}\" Succussfully!";
                    
                }
                catch (Exception ex)
                {
                    errorLabel.Content = ex.Message;
                }
                
            }
            GetBucketList();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    

}
