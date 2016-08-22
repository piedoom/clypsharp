using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clyp
{
    public class Playlist
    {
        [JsonProperty(PropertyName = "AudioFiles")]
        public List<AudioPost> Posts { get; private set; }

        [JsonProperty(PropertyName = "PlaylistId")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "Modifiable")]
        public bool IsModifiable { get; set; }

        [JsonProperty(PropertyName = "ContentAdministrator")]
        public bool IsContentAdministrator { get; set; }

        [JsonProperty(PropertyName = "FeatureSubmissionEligibility")]
        public Eligibility FeatureSubmissionEligibility { get; set; }

        [JsonProperty(PropertyName = "PlaylistUploadToken")]
        public string UploadToken { get; set; }

        public enum Eligibility
        {
            Eligible,
            Ineligible
        }
    }
}
