using Google.Cloud.Firestore;
using System;

 namespace MyPubSubFunction
 {
    [FirestoreData]
    public class Post
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public Timestamp DateCreated { get; set; }
        [FirestoreProperty]
        public Timestamp DateUpdated { get; set; }
        [FirestoreProperty]
        public string Content { get; set; }

        public string BlogId_FK { get; set; }

        
    }
}
