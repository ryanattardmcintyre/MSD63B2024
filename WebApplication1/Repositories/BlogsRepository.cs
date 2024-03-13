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

        
        public async Task<List<Blog>> GetBlogs() //Task is used when the method is returning something ASYNCHRONOUSLY
        {
            List<Blog> blogs = new List<Blog>();

            Query blogQuery = db.Collection("blogs");
            QuerySnapshot blogQuerySnapshot = await blogQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in blogQuerySnapshot.Documents)
            {
                Blog   b= documentSnapshot.ConvertTo<Blog>();
                b.Id = documentSnapshot.Id;
                blogs.Add(b);
            }

            return blogs;

        }


        public async void UpdateBlog(Blog updatedBlog)
        {
            updatedBlog.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);

            DocumentReference blogRef = db.Collection("blogs").Document(updatedBlog.Id);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Name", updatedBlog.Name },
                { "DateUpdated", updatedBlog.DateUpdated }
            };
            await blogRef.UpdateAsync(updates);

        }



    }
}
