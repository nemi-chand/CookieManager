## Cookie Manager Usages

### ICookieManager interface

```csharp
public class MyCookie
{
  public string Id { get; set; }

  public DateTime Date { get; set; }

  public string Indentifier { get; set; }
}

// Get the myCookie object
MyCookie objFromCookie = _cookieManager.Get<MyCookie>("Key");

// Set the myCookie object
MyCookie cooObj= new MyCookie()
{
  Id = Guid.NewGuid().ToString(),
  Indentifier = "valueasgrsdgdf66514sdfgsd51d65s31g5dsg1rs5dg",
  Date = DateTime.Now
};
_cookieManager.Set("Key", cooObj, 60);

// Get or set <T>
// CookieOption example
MyCookie myCook = _cookieManager.GetOrSet<MyCookie>("Key", () =>
{
     // Write function to store  output in cookie
     return new MyCookie()
     {
       Id = Guid.NewGuid().ToString(),
       Indentifier = "valueasgrsdgdf66514sdfgsd51d65s31g5dsg1rs5dg",
       Date = DateTime.Now
     };

}, new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddDays(1) });

```
### ICookie interface

```csharp
// Gets a cookie item associated with key
_cookie.Get("Key");

// Sets the cookie
_cookie.Set("Key", "value here", new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddDays(1) });

```

### Configure Option
Add CookieManager in startup class in Configure Service
```csharp
// Add CookieManager
services.AddCookieManager();

// or

// Add CookieManager with options
services.AddCookieManager(options => 
{
  // Allow cookie data to encrypt by default it allow encryption
  options.AllowEncryption = false;
  // Throw if not all chunks of a cookie are available on a request for re-assembly.
  options.ThrowForPartialCookies = true;
  // Set null if not allow to devide in chunks
  options.ChunkSize = null;
  // Default Cookie expire time if expire time set to null of cookie
  // Default time is 1 day to expire cookie 
  options.DefaultExpireTimeInDays = 10;
});
```