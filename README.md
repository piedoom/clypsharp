ClypSharp
======

ClypSharp is the unofficial C# API wrapper for the audio sharing site [Clyp](https://clyp.it/).  

Usage
======

Currently, ClypSharp is **not** on NuGet.  However, that will change when the library has reached a stable state.  Right now, you can use the library by cloning or downloading the repo, and adding a reference to the solution in your target project.

Quick Start
======
*Note: the Wiki will fully document all aspects and functions of the library soon*

 ### Getting a song by ID

 ```cs
Clyp.Client.GetPostAsync(id: "idofsong", getSoundwave: false);
```

In the above example, "getSoundwave" tells the API whether or not to fetch the 400 data points used to construct a waveform image (think like soundcloud).  It's an extra API call,
so you have the option.

### Getting just the soundwave points

```cs
Clyp.Client.GetSoundwaveAsync("idofsong")
```

Will return 400 data points used to construct a waveform.

### Uploading a song

```cs
Clyp.Client.UploadPostAsync(
    new Clyp.Create.AudioPost(@"path\to\file"));
```

We use the `Clyp.Create` namespace when uploading posts and other data.  This function automatically does a multipart post and returns an `AudioPost` object.  Many more options
can be passed in, like location and playlist to add.

### Getting posts from a list like Featured or Popular

```cs
Clyp.Client.GetPostsFromListAsync(Clyp.Client.List.Featured);
```

### Get a list of categories (tags)

```cs
Clyp.Client.GetCategoriesAsync();
```

### Get a list of songs in a category

If you used the `GetCategoriesAsync` method, and you want to look up songs within one of the returned categories, you can simply pass in that category.

```cs
var categories = await Clyp.Client.GetCategoriesAsync();
Clyp.Client.GetCategoriesAsync(categories.First());
```

If you have a URL without a `Category` object, you can simply input a `Url` or `string`.

```cs
Clyp.Client.GetPostsAsync("https://api.clyp.it/categorylist/wip");
```

### Get posts from a specific location

Get a list of posts near you (calculated from your IP address)

```cs
Clyp.Client.GetPostsByLocationAsync();
```

Get a list of posts near a specific coordinate

```cs
Clyp.Client.GetPostsByLocationAsync(latitude: 0, longitude: 0);
```

### Get all "special" category endpoints

This API call is kind of like an endpoint for more endpoints.  The `GetPostsFromListAsync` method just exposes these endpoints for easier access, but it's not future proof.  If you're
feeling proactive, make sure to check if the category exists with the special category method.  (If this doesn't make sense, don't worry.  The Clyp API is kind of weird sometimes.)

```cs
Clyp.Client.GetSpecialCategoriesAsync();
```

Authenticated Methods
======

Currently, authenticating with Clyp is not documented, but I have requested help from Clyp support, so it should be only a matter of time!

Contributing
======

You are welcome to contribute, whether it is a typo in the READEM.md, a bug, an issue, or just a suggestion.

License
======

BSD 3-Clause