using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Lab3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;
        public static User loginUser;
        public static string credentialError;
        [ViewData]
        public User LoginUser { get => loginUser; }
        [ViewData]
        public string CredentialError { get => credentialError; }
        public static Movie selectMovie;
        public const string bucketName = "lab3-movies";

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
        {
            Configuration = configuration;
        _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogOut()
        {
             loginUser=null;
            return View("Index");
        }

        public BasicAWSCredentials GetCredentials()
        {
            return new BasicAWSCredentials(Configuration["AWSCredentials:AccesskeyID"], Configuration["AWSCredentials:Secretaccesskey"]);
        }


        public async Task<IActionResult> Login(string username, string password)
        {
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(GetCredentials(), RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
         //User user = await context.LoadAsync<User>("ben@gmail.com");
            User user = await context.LoadAsync<User>(username.Trim().ToLower());
            //Dictionary<string, AttributeValue> user = response.Items[0];
            if (user?.password == password.Trim())
            {
                Console.WriteLine(username);
                loginUser = user;
                credentialError = null;
                return Redirect("Movies");
            }
            credentialError = "Email Or Password Wrong";
            return View("Index", new User { username=username});
        }
        public async Task<IActionResult> Register(string username, string password)
        {
            if (!username.Any() || !password.Any())
                return null;
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(GetCredentials(), RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            User user = new User { username = username, password = password };
            await context.SaveAsync(user);
            loginUser = user;
            credentialError = null;
            return Redirect("Movies");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AddMovie()
        {
            return View();
        }
        public IActionResult Comments()
        {
            if (selectMovie.comments == null) {
                selectMovie.comments = new();
            }
            return View(selectMovie.comments);
        }
        public async Task<IActionResult> EditMovie(string uuid, string view="AddMovie")
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
                selectMovie = await context.LoadAsync<Movie>(uuid);
                    return View(view, selectMovie);
        }
        [HttpPost]
        public async Task<IActionResult> RateMovie(string uuid, int rating)
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            try
            {
                //{"useremail":3,"useremail2":9 }
                selectMovie.ratings[loginUser.username] = rating;
                await context.SaveAsync(selectMovie);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View("MovieDetail", selectMovie);
        }
        [HttpPost]
        public async Task<IActionResult> EditComment(Comment data, string comment)
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            try
            {
                if (data != null)
                {
                    if (data.uuid != null)
                        selectMovie.comments.FirstOrDefault(x => x.uuid == data.uuid).comment = data.comment;
                    else
                    {
                        data.modified_dealline = DateTime.Now.AddDays(2);
                        data.uuid = Guid.NewGuid().ToString();
                        selectMovie.comments.Insert(0, data);
                    }
                    await context.SaveAsync(selectMovie);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View("Comments", selectMovie.comments);
        }
        public IActionResult EditComment(string uuid)
        {
            if(uuid==null)
            return View(new Comment { useremail=loginUser.username });
            else
                return View(selectMovie.comments.FirstOrDefault(x=>x.uuid==uuid));
        }
        public IActionResult RateMovie()
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            if(!selectMovie.ratings.ContainsKey(loginUser.username))
            selectMovie.ratings[loginUser.username] = 10;
            return View(new Rating { useremail = loginUser.username, rating = selectMovie.ratings[loginUser.username] });
        }
        public async Task<IActionResult> DeleteMovie(string uuid, string key)
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            try
            {
                Movie movie = await context.LoadAsync<Movie>(uuid);
               await context.DeleteAsync<Movie>(uuid);
                if(movie?.s3_filename!=null)
               await s3client.DeleteObjectAsync(bucketName, movie.s3_filename);
                return Redirect("Movies"); 
            }
            catch (Exception ex)
            {
                ViewBag["loginStatusMessage"] = "Username or Password is wrong! Try again..";
            }
            return NotFound();
        }
        public async Task<IActionResult> Download(string filename)
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            try
            {
                var fileTransferUtility = new TransferUtility(s3client);
                var objectResponse = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = selectMovie.s3_filename
                });
                if (objectResponse.ResponseStream == null)
                {
                    return NotFound();
                }
                return File(objectResponse.ResponseStream, objectResponse.Headers.ContentType, filename);
            }
            catch (Exception ex)
            {
                ViewBag["loginStatusMessage"] = "Username or Password is wrong! Try again..";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddMovie(string uuid,string title, DateTime releaseDate, IFormFile file)
        {
            var credentials = GetCredentials(); 
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            using MemoryStream newMemoryStream = new MemoryStream() ;
            try
            {
                Movie movie = null;
                if (uuid != null) {
                    movie = await context.LoadAsync<Movie>(uuid);
                    movie.title = title; movie.released_data = releaseDate;
                }
                else
                movie = new Movie {uuid=Guid.NewGuid().ToString(), title = title, released_data = releaseDate };
               
                if (file != null) {
                    string filename = movie.uuid + "." + file.FileName.Split('.').Last();
                    file.CopyTo(newMemoryStream);
                    Console.WriteLine(movie);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = filename,
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    var fileTransferUtility = new TransferUtility(s3client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    movie.s3_filename = filename;
                }
                await context.SaveAsync(movie);
                return Redirect("Movies");
            }
            catch (Exception ex)
            {
                ViewBag["loginStatusMessage"] = "Username or Password is wrong! Try again..";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SearchMovies(int rating)
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            List<Movie> movies = await context.ScanAsync<Movie>(new List<ScanCondition>()).GetRemainingAsync();
            return View("Movies", movies.FindAll(x=>x.AverageRating>rating));
        }

        [HttpGet]
        public async Task<IActionResult> Movies()
        {
            var credentials = GetCredentials();
            using AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
            using AmazonS3Client s3client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            using DynamoDBContext context = new DynamoDBContext(client);
            try
            {
                List<Movie> movies = await context.ScanAsync<Movie>(new List<ScanCondition>()).GetRemainingAsync();
                return View("Movies",movies);
            }
            catch (Exception ex)
            {
                ViewBag["loginStatusMessage"] = "Username or Password is wrong! Try again..";
            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
