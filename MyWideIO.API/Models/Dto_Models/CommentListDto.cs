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
    public partial class CommentListDto : IEquatable<CommentListDto>
    {
        /// <summary>
        /// Gets or Sets Comments
        /// </summary>
        [DataMember(Name = "comments", EmitDefaultValue = false)]
        public List<CommentDto> Comments { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CommentListDto {\n");
            sb.Append("  Comments: ").Append(Comments).Append("\n");
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
            return obj.GetType() == GetType() && Equals((CommentListDto)obj);
        }

        /// <summary>
        /// Returns true if CommentListDto instances are equal
        /// </summary>
        /// <param name="other">Instance of CommentListDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CommentListDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                
                    Comments == other.Comments ||
                    Comments != null &&
                    other.Comments != null &&
                    Comments.SequenceEqual(other.Comments)
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
                if (Comments != null)
                    hashCode = hashCode * 59 + Comments.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(CommentListDto left, CommentListDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CommentListDto left, CommentListDto right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
