using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clyp
{
    public static class Client
    {
        const string BASE = "https://api.clyp.it";
        static MimeSharp.Mime mime = new MimeSharp.Mime();

        /// <summary>
        /// Get a single AudioPost with an ID
        /// </summary>
        /// <param name="id">The ID of the desired AudioPost</param>
        /// <param name="getSoundwave">If true, data for the soundwave will be added to the AudioPost.</param>
        /// <returns></returns>
        async public static Task<AudioPost> GetPostAsync(string id, bool getSoundwave)
        {
            // make sure we specified an ID
            if (string.IsNullOrEmpty(id))
                throw new Exception("Must specify an ID when getting an Audio Post.");

            // call the API and recieve our deserialized object
            var result = await BASE
                .AppendPathSegment(id)
                .GetJsonAsync<AudioPost>();

            // get the soundwave points if requested
            if (getSoundwave)
                result.Waveform = await GetSoundwaveAsync(id);

            // return the result
            return result;
        }

        /// <summary>
        /// Get 400 individual datapoints that represent the audio wave.
        /// </summary>
        /// <param name="id">The ID of the desired AudioPost.</param>
        /// <returns>A byte array of every point, ranging in value from 0 - 100.</returns>
        async public static Task<AudioPost.Soundwave> GetSoundwaveAsync(string id)
        {
            // make sure we specified an ID
            if (string.IsNullOrEmpty(id))
                throw new Exception("Must specify an ID when getting an Audio Post soundwave.");

            // call the API and recieve our deserialized object
            var result = await BASE
                .AppendPathSegments(id, "soundwave")
                .GetAsync();

            // create a new soundwave instance and deserialize
            var soundwave = new AudioPost.Soundwave();
            soundwave.Datapoints = JsonConvert.DeserializeAnonymousType(await result.Content.ReadAsStringAsync(), new byte[400]);

            // return the result
            return soundwave;
        }

        /// <summary>
        /// Upload a file anonymously.
        /// </summary>
        /// <param name="audioPost">A new AudioPost object created with <see cref="Create"/>.</param>
        /// <returns>A new AudioPost instance.</returns>
        async public static Task<AudioPost> UploadPostAsync(Create.AudioPost audioPost)
        {
            // POST https://upload.clyp.it/upload
            var baseRequest = "https://upload.clyp.it".AppendPathSegment("upload");

            // description
            if (!string.IsNullOrEmpty(audioPost.Description))
                baseRequest.SetQueryParam("description", audioPost.Description);

            // playlist logic
            if (string.IsNullOrEmpty(audioPost.PlaylistId) && string.IsNullOrEmpty(audioPost.PlaylistUploadToken))
            {
                baseRequest.SetQueryParam("playlistId", audioPost.PlaylistId);
                baseRequest.SetQueryParam("playlistId", audioPost.PlaylistUploadToken);
                if (audioPost.Order.HasValue)
                    baseRequest.SetQueryParam("order", audioPost.Order.Value);
            }

            // geolocation logic
            if (audioPost.Longitude.HasValue && audioPost.Latitude.HasValue)
            {
                baseRequest.SetQueryParam("longitude", audioPost.Longitude.Value);
                baseRequest.SetQueryParam("latitude", audioPost.Latitude.Value);
            }

            // upload with file
            var response = await baseRequest.PostMultipartAsync(
                new Action<CapturedMultipartContent>((CapturedMultipartContent file) => {
                    file.AddFile("audioFile", audioPost.FilePath, mime.Lookup(audioPost.FilePath));
                }));

            AudioPost result = new AudioPost();
            var res = await response.Content.ReadAsStringAsync();

            // deserialize the newly updated object
            await Task.Factory.StartNew(() =>
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<AudioPost>(res);
            });

            return result;
        }

        /// <summary>
        /// Get a list of categories.
        /// </summary>
        /// <returns>A category object.</returns>
        async public static Task<List<Category>> GetCategoriesAsync()
        {
            // get the json
            var response = await BASE.
                AppendPathSegment("categorylist")
                .GetJsonAsync<List<Category>>();

            return response;
        }

        /// <summary>
        /// Get a list of special category endpoints, like "Featured" or "Random".
        /// </summary>
        /// <returns></returns>
        async public static Task<List<Category>> GetSpecialCategoriesAsync()
        {
            // get the json
            var response = await BASE.
                AppendPathSegment("featuredlist")
                .GetJsonAsync<List<Category>>();

            return response;
        }

        /// <summary>
        /// Get posts from a special list, like "Featured" or "Popular".  NOTE: This is 
        /// not guarenteed to be future-proof - these 'endpoints' were got from the
        /// <see cref="GetSpecialCategoriesAsync"/> request.  If you want to be super-safe,
        /// Do a <see cref="GetSpecialCategoriesAsync"/>, and then call <see cref="GetPostsAsync(Category)"/>.
        /// </summary>
        /// <param name="list">The list from which to fetch audio posts.</param>
        /// <returns>A list of audio posts.</returns>
        async public static Task<List<AudioPost>> GetPostsFromListAsync(List list)
        {
            string url = "";

            // determine our endpoint
            switch (list)
            {
                case List.Featured:
                    url = "https://api.clyp.it/FeaturedList/Featured";
                    break;
                case List.Popular:
                    url = "https://api.clyp.it/FeaturedList/Popular";
                    break;
                case List.Random:
                    url = "https://api.clyp.it/FeaturedList/Random";
                    break;
                case List.Recent:
                    url = "https://api.clyp.it/FeaturedList/Recent";
                    break;
            }

            // get the json
            var response = await url
                .GetJsonAsync<List<AudioPost>>();

            return response;
        }

        /// <summary>
        /// Get random posts within a category.
        /// </summary>
        /// <param name="category">The category object from which the posts should be fetched.</param>
        /// <returns>A list of audio posts.</returns>
        async public static Task<List<AudioPost>> GetPostsAsync(Category category)
        {
            var response = await category.Url
                .GetJsonAsync<List<AudioPost>>();

            return response;
        }

        /// <summary>
        /// Get random posts within a category.
        /// </summary>
        /// <param name="categoryUrl">The URL of the category.</param>
        /// <returns>A list of audio posts.</returns>
        async public static Task<List<AudioPost>> GetPostsAsync(Url categoryUrl)
        {
            return await GetPostsAsync(
                new Category(categoryUrl));
        }

        /// <summary>
        /// Get random posts within a category.
        /// </summary>
        /// <param name="categoryUrl">The URL string of the category.</param>
        /// <returns>A list of audio posts.</returns>
        async public static Task<List<AudioPost>> GetPostsAsync(string categoryUrl)
        {
            return await GetPostsAsync(
                new Category(
                    new Url(categoryUrl)));
        }

        /// <summary>
        /// Get audio posts from a location.  If no specific location is provided, the site will 
        /// determine location based on the IP address.
        /// </summary>
        /// <param name="count">The number of posts to get.  Defaults to 10.</param>
        /// <param name="latitude">The latitude coordinate. Value must be between -15069 and 15069.</param>
        /// <param name="longitude">The longitude coordinate. Value must be between -90 and 90.</param>
        /// <returns></returns>
        async public static Task<List<AudioPost>> GetPostsByLocationAsync(int? count = null, double? latitude = null, double? longitude = null)
        {
            var request = BASE
                .AppendPathSegments("featuredlist", "nearby");

            if (count.HasValue)
                request.SetQueryParam("count", count.Value);

            if (latitude.HasValue && longitude.HasValue)
            {
                    request
                            .SetQueryParam("longitude", longitude.Value)
                            .SetQueryParam("latitude", latitude.Value);
            }

            var response = await request.GetJsonAsync<List<AudioPost>>();

            return response;   
        }

        public enum List
        {
            Featured,
            Popular,
            Random,
            Recent
        }

        /// Unfortunately, the URL Upload page seems to 404, so we can't use it :'(
        // async public static Task<AudioPost> UploadURLAsync(Create.AudioURL audioURL)
        // {
        //     var reqObj = new { uploadurl = audioURL.Url.ToString() };
        // 
        //     var request = "https://upload.clyp.it"
        //         .AppendPathSegment("uploadurl");
        // 
        //     if (!string.IsNullOrEmpty(audioURL.PlaylistId) && !string.IsNullOrEmpty(audioURL.PlaylistUploadToken))
        //         request
        //             .SetQueryParam("playlistId", audioURL.PlaylistId)
        //             .SetQueryParam("playlistUploadToken", audioURL.PlaylistUploadToken);
        // 
        //     var response = await request.PostJsonAsync(
        //         reqObj
        //         );
        // 
        //     var res = await response.Content.ReadAsStringAsync();
        // 
        //     AudioPost result = new AudioPost();
        //     await Task.Factory.StartNew(() =>
        //     {
        //         result = JsonConvert.DeserializeObject<AudioPost>(res);
        //     });
        // 
        //     return result;
        // }

    }
}
