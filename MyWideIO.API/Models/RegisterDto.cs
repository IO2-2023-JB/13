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
    public partial class RegisterDto : IEquatable<RegisterDto>
    {
        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        /// <example>&quot;john.doe@mail.com&quot;</example>
        [DataMember(Name = "email", EmitDefaultValue = false)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or Sets Nickname
        /// </summary>
        /// <example>&quot;johnny123&quot;</example>
        [DataMember(Name = "nickname", EmitDefaultValue = false)]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        /// <example>&quot;John&quot;</example>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Surname
        /// </summary>
        /// <example>&quot;Doe&quot;</example>
        [DataMember(Name = "surname", EmitDefaultValue = false)]
        public string Surname { get; set; }

        /// <summary>
        /// Gets or Sets Password
        /// </summary>
        /// <example>&quot;Passwd123!&quot;</example>
        [DataMember(Name = "password", EmitDefaultValue = false)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or Sets UserType
        /// </summary>
        [DataMember(Name = "userType", EmitDefaultValue = true)]
        public UserTypeDto UserType { get; set; }

        /// <summary>
        /// Gets or Sets AvatarImage
        /// </summary>
        [DataMember(Name="avatarImage", EmitDefaultValue=true)]
        public string? AvatarImage { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RegisterDto {\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  Nickname: ").Append(Nickname).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Surname: ").Append(Surname).Append("\n");
            sb.Append("  Password: ").Append(Password).Append("\n");
            sb.Append("  UserType: ").Append(UserType).Append("\n");
            sb.Append("  AvatarImage: ").Append(AvatarImage).Append("\n");
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
            return obj.GetType() == GetType() && Equals((RegisterDto)obj);
        }

        /// <summary>
        /// Returns true if RegisterDto instances are equal
        /// </summary>
        /// <param name="other">Instance of RegisterDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RegisterDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Email == other.Email ||
                    Email != null &&
                    Email.Equals(other.Email)
                ) &&
                (
                    Nickname == other.Nickname ||
                    Nickname != null &&
                    Nickname.Equals(other.Nickname)
                ) &&
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) &&
                (
                    Surname == other.Surname ||
                    Surname != null &&
                    Surname.Equals(other.Surname)
                ) &&
                (
                    Password == other.Password ||
                    Password != null &&
                    Password.Equals(other.Password)
                ) &&
                (
                    UserType == other.UserType ||
                    
                    UserType.Equals(other.UserType)
                ) && 
                (
                    AvatarImage == other.AvatarImage ||
                    AvatarImage != null &&
                    AvatarImage.Equals(other.AvatarImage)
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
                if (Email != null)
                    hashCode = hashCode * 59 + Email.GetHashCode();
                if (Nickname != null)
                    hashCode = hashCode * 59 + Nickname.GetHashCode();
                if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
                if (Surname != null)
                    hashCode = hashCode * 59 + Surname.GetHashCode();
                if (Password != null)
                    hashCode = hashCode * 59 + Password.GetHashCode();
                    
                    hashCode = hashCode * 59 + UserType.GetHashCode();
                    if (AvatarImage != null)
                    hashCode = hashCode * 59 + AvatarImage.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(RegisterDto left, RegisterDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RegisterDto left, RegisterDto right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
