using Google.Cloud.Firestore;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class BlogsRepository
    {
        //Repositories will contain the CRUD operations
        //which will directly interact with the db
        //C = Create
        //R = Read
        //U = Update
        //D = Delete

        FirestoreDb db; //declaration
        public BlogsRepository(string project) {
           db  = FirestoreDb.Create(project); //the initialization of the db

        }

        public async void AddBlog(Blog blog) {

            DocumentReference docRef = db.Collection("blogs").Document(blog.Id);
            await docRef.SetAsync(blog);
        }

    }
}
