using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {
        //I need to REQUEST an instance of BlogsRepository to be used throughout this controller
        //to REQUEST  an instance: DEPENDENCY INJECTION

        private BlogsRepository blogsRepo;
        private BucketsRepository _bucketsRepository;
        private IConfiguration _config;
        public BlogsController(BlogsRepository blogsRepository, BucketsRepository bucketsRepository, IConfiguration config) 
        {
            blogsRepo= blogsRepository;
            _bucketsRepository= bucketsRepository;
            _config= config;
        }


        public async Task<IActionResult> Index()
        {
            var list = await blogsRepo.GetBlogs();

            return View(list);
        }

        /// <summary>
        /// This method will be called when the user wants to load the Create page to add a blog; In return an
        /// empty page with empty fields will be shown to the user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create() {
            return View(); 
        }


        /// <summary>
        /// This method will be called after the user filled the textboxes and clicked on the submit method
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Blog b, IFormFile file, string recipient) {
            
            
            string uniqueFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);


            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);//this line copies the uploaded file bytes into our own MemoryStream

            await _bucketsRepository.UploadFileAsync(uniqueFilename, ms);  
            await _bucketsRepository.GrantAccess(uniqueFilename, recipient);

            string bucketName = _config["bucket"].ToString();


            b.PictureUri = $"https://storage.cloud.google.com/{bucketName}/{uniqueFilename}";
            b.Id = Guid.NewGuid().ToString(); //generating a new id  
            b.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.Author = User.Identity.Name;

            blogsRepo.AddBlog(b);
             
            return View(); 
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string blogId) {

            //the SingleOrDefault on a list which accepts a condition where the Id of the blog(s) in the list matches the parameter blogId
            //if it finds a matching blog return it
            //if no return null
            var listOfBlogs = (await blogsRepo.GetBlogs());
            var existingBlog = listOfBlogs.SingleOrDefault(x => x.Id == blogId);

            return View(existingBlog);
        }

        [HttpPost]
        public IActionResult Edit(Blog b) { 
            blogsRepo.UpdateBlog(b);
            return View(b);
        
        }


        public IActionResult Delete(string blogId) {

            //blogsRepo.Delete(blogId);
            return RedirectToAction("Index");
        }
    }
}
