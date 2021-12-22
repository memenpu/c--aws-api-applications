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
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Amazon.DynamoDBv2.DataModel;

namespace Lab2
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            loginStatusMessage.Content = "Loging in";
            using var client = new AmazonDynamoDBClient(GetCredentials(), RegionEndpoint.CACentral1);
            using var context = new DynamoDBContext(client);
            try
            {
                //User user = await context.LoadAsync<User>("ben@gmail.com");
                User user = await context.LoadAsync<User>(usernameTextBox.Text.Trim());
            //Dictionary<string, AttributeValue> user = response.Items[0];
                //if (user["password"].S == passwordTextBox.Text.Trim()) {
                if (user.password == passwordTextBox.Password.Trim()) {
                    loginStatusMessage.Content = "successfully Loged in!";
                    new MyBookshelf(user).ShowDialog();
                }
                else
                    loginStatusMessage.Content = "Username or Password is wrong! Try again..";
            }
            catch (Exception ex)
            {
                loginStatusMessage.Content = "Username or Password is wrong! Try again..";
            }


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
    }
}
