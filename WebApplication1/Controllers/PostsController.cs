using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class PostsController : Controller
    {

        private PostsRepository postsRepo;

        public PostsController(PostsRepository postsRepository) {
            postsRepo = postsRepository;
        }

        public async Task<IActionResult> Index(string blogId)
        {
            ViewBag.blogId = blogId;
            var list = await postsRepo.GetPosts(blogId);
            return View(list);
        }


        public async Task<IActionResult> Read(string blogId, string postId)
        {
            var myPost = await postsRepo.GetPost(postId, blogId);
            return View(myPost);
        }


        [HttpGet]
        public IActionResult Create(string blogId)
        {
            ViewBag.blogId = blogId; //this a way how you can pass simple data into the View
            //i am passing to the View a blogId of the selected blog using the ViewBag
            return View();
        }

        /// <summary>
        /// This method will be called after the user filled the textboxes and clicked on the submit method
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(Post p)
        {
            p.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            p.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);

            postsRepo.CreatePost(p);

            return View(p);
        }

    }
}
