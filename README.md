# Cookie Manager
ASP.Net Core Abstraction layer on top of Cookie .  ASP.NET Core Wrapper to read and write the cookie.

[![License](https://img.shields.io/badge/license-apache%202.0-60C060.svg)](https://github.com/nemi-chand/CookieManager/blob/master/LICENSE)
[![NuGet Package](https://img.shields.io/nuget/v/cookiemanager.svg)](https://www.nuget.org/packages/CookieManager/)

## Build Status
| Build server| Platform | Status |
|-------------|----------------|-------------|
| AppVeyor    | Windows  | [![AppVeyor](https://ci.appveyor.com/api/projects/status/github/nemi-chand/CookieManager?branch=master&svg=true)](https://ci.appveyor.com/project/nemi-chand/CookieManager/branch/master) | 
|Travis       | Linux / MacOS  | [![Build Status](https://travis-ci.org/nemi-chand/CookieManager.svg?branch=master)](https://travis-ci.org/nemi-chand/CookieManager) |

# Features

  - Strongly Typed : CookieManager interface allows you to play with generic object. You don't have to care about casting or serialization.
  - Secure Cookie Data :The cookie data is protected with the machine key, using security algorithm. For more about data protection (https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/)
  - Configuration :There are easy options to configure CookieManager. Just add the CookieManager in Configure Service.
  - Func`<TResult>` support :Encapsulates a method, which returns a value of the type specified by the TResult parameter
  - Ease to use :The interfaces allows to ease use of read and write http cookie.

Nuget Package :  https://www.nuget.org/packages/CookieManager/
```csharp
Install-Package CookieManager
```
.NET CLI
```csharp
dotnet add package CookieManager --version 2.0.0
```

## Usages

### ICookieManager interface

```csharp
public class MyCookie
{
  public string Id { get; set; }

  public DateTime Date { get; set; }

  public string Indentifier { get; set; }
}

// get the myCookie object
MyCookie objFromCookie = _cookieManager.Get<MyCookie>("Key");

//set the myCookie object
MyCookie cooObj= new MyCookie()
{
  Id = Guid.NewGuid().ToString(),
  Indentifier = "valueasgrsdgdf66514sdfgsd51d65s31g5dsg1rs5dg",
  Date = DateTime.Now
};
_cookieManager.Set("Key", cooObj, 60);

//Get or set <T>
//Cookieoption example
MyCookie myCook = _cookieManager.GetOrSet<MyCookie>("Key", () =>
{
     //write fucntion to store  output in cookie
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
//Gets a cookie item associated with key
_cookie.Get("Key");

//Sets the cookie
_cookie.Set("Key", "value here", new CookieOptions() { HttpOnly = true, Expires = DateTime.Now.AddDays(1) });

```

### Configure Option
add CookieManager in startup class in Configure Service
```csharp
//add CookieManager
services.AddCookieManager();

//or

//add CookieManager with options
services.AddCookieManager(options => 
{
  //allow cookie data to encrypt by default it allow encryption
  options.AllowEncryption = false;
  //Throw if not all chunks of a cookie are available on a request for re-assembly.
  options.ThrowForPartialCookies = true;
  // set null if not allow to devide in chunks
  options.ChunkSize = null;
  //Default Cookie expire time if expire time set to null of cookie
  //default time is 1 day to expire cookie 
  options.DefaultExpireTimeInDays = 10;
});
```
