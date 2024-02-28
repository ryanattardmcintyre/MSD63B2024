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
        public BlogsController(BlogsRepository blogsRepository) 
        {
            blogsRepo= blogsRepository;
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
        public IActionResult Create(Blog b) {
            b.Id = Guid.NewGuid().ToString();
            b.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.Author = User.Identity.Name;

            blogsRepo.AddBlog(b);
             
            return View(); 
        }
    }
}
