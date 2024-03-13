using Google.Cloud.Firestore;
using System.Reflection.Metadata;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class PostsRepository
    {

        FirestoreDb db; //declaration
        public PostsRepository(string project)
        {
            db = FirestoreDb.Create(project); //the initialization of the db

        }

        public async Task<List<Post>> GetPosts(string blogId)
        {
            ///blogs/80f73ae0-4006-4c4e-ab94-27993e8f5d4e/Posts/JkTXHqpmRqahUfKIRCQs
            //blogs/<blog-id>/posts

            List<Post> posts = new List<Post>();

            Query postQuery = db.Collection($"blogs/{blogId}/posts");
            QuerySnapshot postQuerySnapshot = await postQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in postQuerySnapshot.Documents)
            {
                Post b = documentSnapshot.ConvertTo<Post>();
                b.Id = documentSnapshot.Id;
                b.BlogId_FK = blogId;

                posts.Add(b);
            }

            return posts;
        }

        /// <summary>
        /// Creates a post in a nested collection within the selected blog
        /// </summary>
        /// <param name="p"></param>
        public async void CreatePost(Post p)
        {//home exercise
         //
            p.Id = Guid.NewGuid().ToString();

            DocumentReference docRef = db.Collection($"blogs/{p.BlogId_FK}/posts").Document(p.Id);
            await docRef.SetAsync(p);

        }

        public async Task<Post> GetPost(string postId, string blogId)
        {
            var docRef = await db.Collection($"blogs/{blogId}/posts").Document(postId).GetSnapshotAsync();
            if (docRef == null)
                return null;

            Post myPost = docRef.ConvertTo<Post>();
            myPost.Id = postId;
            myPost.BlogId_FK = blogId;

            return myPost;

        }
    }
}
