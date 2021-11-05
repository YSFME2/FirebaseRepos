using Firebase.Database;
using FirebaseRepos.Reposatories;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseRepos.WPFSimple.Models
{
    public static class Context
    {
        //Details for firebase realtime database
        private static string baseURL = "";
        private static string apiSecret = "";


        //Details for firestore
        private static string projectID = "";


        //Details for Storage
        private static string apiName = "";
        private static string apiKey = "";
        private static string uploadEmail = "";
        private static string uploadEmailPawword = "";

        private static readonly FirebaseClient _firebaseClient = new FirebaseClient(baseURL, new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(apiSecret) });
        public static Storage Storage { get; } = new Storage(apiName, apiKey, uploadEmail, uploadEmailPawword);
        private static readonly FirestoreDb FirestoreDb = FirestoreDb.Create(projectID);
        public static RealTimeRepo<User> Users { get; private set; } = new RealTimeRepo<User>(_firebaseClient.Child("Users"), true);
    }
}
