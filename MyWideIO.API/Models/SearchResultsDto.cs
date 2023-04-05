/*
 * VideIO API
 *
 * VideIO project API specification.
 *
 * The version of the OpenAPI document: 1.0.6
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WideIO.API.Converters;

namespace WideIO.API.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class SearchResultsDto : IEquatable<SearchResultsDto>
    {
        /// <summary>
        /// Gets or Sets Videos
        /// </summary>
        [DataMember(Name="videos", EmitDefaultValue=false)]
        public List<VideoMetadataDto> Videos { get; set; }

        /// <summary>
        /// Gets or Sets Users
        /// </summary>
        [DataMember(Name="users", EmitDefaultValue=false)]
        public List<UserDto> Users { get; set; }

        /// <summary>
        /// Gets or Sets Playlists
        /// </summary>
        [DataMember(Name="playlists", EmitDefaultValue=false)]
        public List<PlaylistBaseDto> Playlists { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SearchResultsDto {\n");
            sb.Append("  Videos: ").Append(Videos).Append("\n");
            sb.Append("  Users: ").Append(Users).Append("\n");
            sb.Append("  Playlists: ").Append(Playlists).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SearchResultsDto)obj);
        }

        /// <summary>
        /// Returns true if SearchResultsDto instances are equal
        /// </summary>
        /// <param name="other">Instance of SearchResultsDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SearchResultsDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Videos == other.Videos ||
                    Videos != null &&
                    other.Videos != null &&
                    Videos.SequenceEqual(other.Videos)
                ) && 
                (
                    Users == other.Users ||
                    Users != null &&
                    other.Users != null &&
                    Users.SequenceEqual(other.Users)
                ) && 
                (
                    Playlists == other.Playlists ||
                    Playlists != null &&
                    other.Playlists != null &&
                    Playlists.SequenceEqual(other.Playlists)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Videos != null)
                    hashCode = hashCode * 59 + Videos.GetHashCode();
                    if (Users != null)
                    hashCode = hashCode * 59 + Users.GetHashCode();
                    if (Playlists != null)
                    hashCode = hashCode * 59 + Playlists.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(SearchResultsDto left, SearchResultsDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SearchResultsDto left, SearchResultsDto right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
