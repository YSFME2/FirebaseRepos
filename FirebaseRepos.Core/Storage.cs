using Firebase.Auth;
using Firebase.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FirebaseRepos.Core.Reposatories
{
    public class Storage
    {
        string apiKey;
        string apiName;
        public string Email { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiName">the api name</param>
        /// <param name="apiKey">the secret api key</param>
        /// <param name="email">email acount that assigned to storage api</param>
        /// <param name="password">email password that assigned to storage api</param>
        public Storage(string apiName,string apiKey,string email,string password)
        {
            this.apiName = apiName;
            this.apiKey = apiKey;
            Email = email;
            Password = password;
        }
        /// <summary>
        /// Upload file to firebase storage (generate new GUID for file name)
        /// </summary>
        /// <param name="stream">data stream</param>
        /// <param name="path">storage path (without file name)</param>
        /// <returns></returns>
        public async Task<string> UploadImage(Stream stream, string path)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(Email, Password);
            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(
                apiName,
                 new FirebaseStorageOptions
                 {
                     AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                     ThrowOnCancel = true,
                 })
                .Child(path)
                .Child(Guid.NewGuid().ToString())
                .PutAsync(stream);

            // Track progress of the upload

            // await the task to wait until upload completes and get the download url
            return await task;
        } 
        
        
        /// <summary>
        /// Upload file to firebase storage 
        /// </summary>
        /// <param name="stream">data stream</param>
        /// <param name="path">storage path (without file name)</param>
        /// <returns></returns>
        public async Task<string> UploadImage(Stream stream, string path,string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(Email, Password);
            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(
                apiName,
                 new FirebaseStorageOptions
                 {
                     AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                     ThrowOnCancel = true,
                 })
                .Child(path)
                .Child(fileName)
                .PutAsync(stream);

            // await the task to wait until upload completes and get the download url
            return await task;
        }
    }
}