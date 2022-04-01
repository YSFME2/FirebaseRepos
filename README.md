# FirebaseRepos
Is a nuget Package that deals with Firebase RealTime Database and FireStore
## Supported Frameworks
* .NetFramework (>=4.5)
* .Net Standard (>=2.0)
* UWP
## Dependencies
### RealTime Database
* [FirebaseRepos.Base](https://www.nuget.org/packages/FirebaseRepos.Base/)
* [FirebaseDatabase.net](https://www.nuget.org/packages/FirebaseDatabase.net/)
##### Only for UWP
* [FirebaseDatabase.Platform.net](https://www.nuget.org/packages/FirebaseDatabase.Platform.net/)
### FireStore
* [FirebaseRepos.Base](https://www.nuget.org/packages/FirebaseRepos.Base/)
* [Google.Cloud.Firestore](https://www.nuget.org/packages/Google.Cloud.Firestore/)
##### Only for .Net Standard
* [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp/)
## Install
### RealTime Database
```
PM> Install-Package FirebaseRepos.RealTime
```
### FireStore
```
PM> Install-Package FirebaseRepos.FireStore
```

## Usage
### Create FireBase Client
#### RealTime DataBase
```csharp
FirebaseClient _firebaseClient = new FirebaseClient("databaseURL", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult("DataBase-Secret") });
```
* DataBase Secret From the url https://console.firebase.google.com/project/{Your-App-ID}/settings/serviceaccounts/databasesecrets
#### FireStore
```csharp
FirestoreDb _firestoreClient = FirestoreDb.Create("project_id");
```
* You must include the credentials file in the project
* [Credentials – APIs & Services - Google Cloud Platform](https://console.cloud.google.com/apis/credentials)
<br/>

### Create Reposatory of Model
#### Models Example
```csharp
public class Country: IFireBaseClass
{
  public string ID { get; set; }
  public string Name { get; set; }
  public List<City> Cities { get; set; }
}

public class City
{
  public string Name { get; set; }
}
```
#### RealTime DataBase
```csharp
RealTimeRepo<Country> countriesRepo = new RealTimeRepo<Country>(_firebaseClient.Child("Countries"));
```
* The string ("Countries") is the first node in the tree
#### FireStore
```csharp
FirestoreRepo<Country> countriesRepo = new FirestoreRepo<Countries>(_firestoreClient.Collection("Countries"));
```
* The string ("Countries") is name of the Collection
<br/>

### Add Data
```csharp
var country = new Country()
{
  Name = "country 1",
  Cities = new List()
  {
    new City() { Name = "city 1" },
    new City() { Name = "city 2" }
  }
};
string countryID = await countriesRepo.AddAsync(country);
```
<br/>

### Get All Data
```csharp
List<Country> countries = await countriesRepo.GetAllAsync();
```
```csharp
List<Country> countries = countriesRepo.GetAll();
```
<br/>

### Get Single Data by ID 
```csharp
Country country = await countriesRepo.GetAsync(3);
```
```csharp
Country country = countriesRepo.Get(3);
```
<br/>

### Delete by ID
```csharp
bool isDeleted = await countriesRepo.DeleteAsync(3);
```

<br/>

### Filter by fields
#### RealTime DataBase
```csharp
List<Country> countries = await countriesRepo.GetAsync(x => x.Object.Name == "country");
```
```csharp
List<Country> countries = await countriesRepo.GetAsync("Name", "country");
```
#### FireStore
```csharp
List<Country> countries = await countriesRepo.GetAsync(new List<(string fieldName, object value)> { ("Name", "country"));
```
<br/>

### Update
```csharp
bool isUpdated = await countriesRepo.UpdateAsync(updatedCountry,updatedCountry.ID);
```

<br/>

### Set Listener

#### RealTime DataBase
```csharp
countriesRepo.SetListener((e) =>
  {
    if (e.EventType != FirebaseEventType.Delete)
    {
      Console.WriteLine("Deleted ID : " + e.Key);
    }
    else if(e.EventType == FirebaseEventType.InsertOrUpdate)
    {
      Console.WriteLine("Inserted or Updated Name : " + e.Object.Name);
    }
});
```
#### FireStore
```csharp
countriesRepo.SetListener(x =>
  {
      Console.WriteLine("Name : " + x.Name);
});
```

<br/>

### Remove Listener

```csharp
countriesRepo.RemoveListener();
```
