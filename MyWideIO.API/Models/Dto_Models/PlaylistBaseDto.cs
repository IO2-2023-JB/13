using System.Text;
using System.Runtime.Serialization;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Models.Dto_Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class PlaylistBaseDto : IEquatable<PlaylistBaseDto>
    {
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        /// <example>&quot;Favorites&quot;</example>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        /// <example>&quot;123e4567-e89b-12d3-a456-426614174000&quot;</example>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or Sets Visibility
        /// </summary>
        [DataMember(Name="visibility", EmitDefaultValue=true)]
        public VisibilityEnum Visibility { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PlaylistBaseDto {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Visibility: ").Append(Visibility).Append("\n");
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
            return obj.GetType() == GetType() && Equals((PlaylistBaseDto)obj);
        }

        /// <summary>
        /// Returns true if PlaylistBaseDto instances are equal
        /// </summary>
        /// <param name="other">Instance of PlaylistBaseDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PlaylistBaseDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) && 
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) && 
                (
                    Visibility == other.Visibility ||
                    
                    Visibility.Equals(other.Visibility)
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
                    if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
                    if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                    
                    hashCode = hashCode * 59 + Visibility.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(PlaylistBaseDto left, PlaylistBaseDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PlaylistBaseDto left, PlaylistBaseDto right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
