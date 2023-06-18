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

namespace MyWideIO.API.Models.Dto_Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class CreatePlaylistResponseDto : IEquatable<CreatePlaylistResponseDto>
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        /// <example>&quot;123e4567-e89b-12d3-a456-426614174000&quot;</example>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CreatePlaylistResponseDto {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
            return obj.GetType() == GetType() && Equals((CreatePlaylistResponseDto)obj);
        }

        /// <summary>
        /// Returns true if CreatePlaylistResponseDto instances are equal
        /// </summary>
        /// <param name="other">Instance of CreatePlaylistResponseDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CreatePlaylistResponseDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ;
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
                if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(CreatePlaylistResponseDto left, CreatePlaylistResponseDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreatePlaylistResponseDto left, CreatePlaylistResponseDto right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
