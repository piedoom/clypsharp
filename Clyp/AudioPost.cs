using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clyp
{
    /// <summary>
    /// Generic audio class
    /// </summary>
    public class AudioPost
    {
        [JsonProperty(PropertyName = "Status")]
        public State Status { get; private set; }

        [JsonProperty(PropertyName = "Successful")]
        public bool Success { get; private set; }

        [JsonProperty(PropertyName = "PlaylistId")]
        public string PlaylistId { get; private set; }

        [JsonProperty(PropertyName = "PlaylistUploadToken")]
        public string PlaylistUploadToken { get; private set; }

        [JsonProperty(PropertyName = "CommentsEnabled")]
        public bool AllowsComments { get; set; }

        [JsonProperty(PropertyName = "Category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "AudioFileId")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Duration")]
        public float DurationSeconds { get; private set; }

        public int DurationMilliseconds { get
            {
                return (int)(DurationSeconds * 1000);
            }
        }

        [JsonProperty(PropertyName = "Url")]
        public string Url { get; private set; }

        [JsonProperty(PropertyName = "SecureMp3Url")]
        public string UrlMp3 { get; private set; }

        [JsonProperty(PropertyName = "SecureOggUrl")]
        public string UrlOgg { get; private set; }

        [JsonProperty(PropertyName = "DateCreated")]
        private string dateString { get; set; }

        public Soundwave Waveform { get; set; }

        public DateTime Date
        {
            get
            {
                return DateTime.Parse(dateString);
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine($"Title: {Title}");
            str.AppendLine($"- - - - - - - - - - - -");
            str.AppendLine($"ID: {Id}");
            str.AppendLine($"Status: {Status}");
            str.AppendLine($"Allows Comments: {AllowsComments}");
            str.AppendLine($"Category: {Category}");
            str.AppendLine($"Description: {Description}");
            str.AppendLine($"Duration in Seconds: {DurationSeconds}");
            str.AppendLine($"Urls: {Url}, {UrlMp3}, {UrlOgg}");
            str.AppendLine($"Date Created: {Date.ToString()}");

            return str.ToString();
        }

        /// <summary>
        /// The total 400 points that are used to draw audio waveforms.
        /// </summary>
        public class Soundwave
        {
            public byte[] Datapoints { get; set; } = new byte[400];

            public Soundwave() { }

            /// <summary>
            /// Draw your own soundwave - useful for local-only applications.
            /// </summary>
            /// <param name="points">400 points varying from 0 to 1.</param>
            public Soundwave(byte[] points)
            {
                if (points.Length != 400)
                    throw new Exception("Datapoints must total 400.");
                Datapoints = points;
            }
        }

        public enum State
        {
            Public,
            Private,
            Deleted,
            DownloadDisabled,
            PrivateDownloadDisabled
        }
    }

    
}
