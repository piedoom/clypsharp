using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clyp.Create
{
    public class AudioURL
    {
        public Url Url { get; set; }
        public string PlaylistId { get; set; } = null;
        public string PlaylistUploadToken { get; set; } = null;

        public AudioURL(Url url, string playlistId = null, string playlistUploadToken = null)
        {
            // set a playlist if it is specified.
            if (!string.IsNullOrEmpty(playlistId) && !string.IsNullOrEmpty(playlistUploadToken))
            {
                PlaylistId = playlistId;
                PlaylistUploadToken = playlistUploadToken;
            }

            if (url == null)
                throw new Exception("Must provide upload URL.");

            Url = url;
        }

        public AudioURL(string url, string playlistId = null, string playlistUploadToken = null) : this (new Url(""), playlistId, playlistUploadToken)
        {
            Url = url;
        }
    }

    public class AudioPost {

        /// <summary>
        /// Create an AudioPost template for uploading a file
        /// </summary>
        /// <param name="filePath">The absolute path to the file to be uploaded</param>
        /// <param name="playlistId">The optional playlist identifier to add this song to. 
        /// Must be provided in combination with <see cref="PlaylistUploadToken">playlistUploadToken</see> </param>
        /// <param name="playlistUploadToken">The optional playlist upload (access) token.
        /// Must be provided in combination with <see cref="PlaylistId">playlistId.</see></param>
        /// <param name="order">The optional order of this song in the playlist.  Will be ignored if playlist fields are blank.</param>
        /// <param name="description">The optional description.  Can be no longer than 420 characters (will automatically truncate)</param>
        /// <param name="longitude">The optional Longitude (must be between -15069 and 15069)</param>
        /// <param name="latitude">The optional Latitude (must be between -90 and 90)</param>
        public AudioPost(
            string filePath, 
            string playlistId = null, 
            string playlistUploadToken = null,
            int? order = null,
            string description = null,
            double? longitude = null,
            double? latitude = null)
        {
            // make sure a filepath exists
            if (string.IsNullOrEmpty(filePath))
                throw new Exception("Must specify file path.");

            FilePath = filePath;

            // make sure playlistId and playlistUploadToken are both null or both have value
            if (string.IsNullOrEmpty(playlistId) && !string.IsNullOrEmpty(playlistUploadToken))
                throw new Exception("A playlist upload token must be provided when specifying a playlist ID.");
            else if (!string.IsNullOrEmpty(playlistId) && string.IsNullOrEmpty(playlistUploadToken))
                throw new Exception("A playlist ID must be provided when specifying a playlist upload token.");
            else if (!string.IsNullOrEmpty(playlistId) && !string.IsNullOrEmpty(playlistUploadToken))
            {
                PlaylistId = playlistId;
                PlaylistUploadToken = playlistUploadToken;
            }

            if (order.HasValue)
                Order = order.Value;

            if (!string.IsNullOrEmpty(description))
                Description = description;

            if (longitude.HasValue && !latitude.HasValue || !longitude.HasValue && latitude.HasValue)
                throw new Exception("Latitude and Longitude must both be present, or both null.");
            else if (longitude.HasValue && latitude.HasValue)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        public string FilePath { get; set; }
        public string PlaylistId { get; set; } = null;
        public string PlaylistUploadToken { get; set; } = null;
        public int? Order { get; set; } = null;
        private string _description = null;
        public string Description {
            get
            {
                return _description;
            }
            set
            {
                // as per the API, our description cannot be more than 420 characters in length (blaze it)
                // Usually we won't want to throw an error here, so we'll just silently truncate the string
                if (value.Length > 420)
                    value = value.Substring(0, 420);

                _description = value;
            }
        }

        private double? _longitude = null;
        public double? Longitude
        {
            get { return _longitude; }
            set
            {
                // longitude must be between -15069 and 15069
                if (value > 15069 || value < -15069)
                    throw new Exception("Longitude must be between -15069 and 15069.");

                _longitude = value;
            }
        }

        private double? _latitude = null;
        public double? Latitude
        {
            get { return _latitude; }
            set
            {
                // latitude must be between -90 and 90
                if (value > 90 || value < -90)
                    throw new Exception("Latitude must be between -90 and 90.");

                _latitude = value;
            }
        }
    }
}
