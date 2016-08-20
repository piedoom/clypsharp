using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    }
}
