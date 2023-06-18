using System.Text;
using System.Runtime.Serialization;
using MyWideIO.API.Models.Enums;
using Newtonsoft.Json;

namespace MyWideIO.API.Models.Dto_Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class GetTicketDto : IEquatable<GetTicketDto>
    {
        [DataMember(Name = "ticketId", EmitDefaultValue = false)]
        public Guid TicketId { get; set; }
        
        /// <summary>
        /// Gets or Sets SubmitterId
        /// </summary>
        /// <example>&quot;123e4567-e89b-12d3-a456-426614174000&quot;</example>
        [DataMember(Name = "submitterId", EmitDefaultValue = false)]
        public Guid SubmitterId { get; set; }

        /// <summary>
        /// Gets or Sets TargetId
        /// </summary>
        /// <example>&quot;123e4567-e89b-12d3-a456-426614174000&quot;</example>
        [DataMember(Name = "targetId", EmitDefaultValue = false)]
        public Guid TargetId { get; set; }
        
        [DataMember(Name = "targetType", EmitDefaultValue = false)]
        public TicketTargetTypeEnum TargetType { get; set; }    

        /// <summary>
        /// Gets or Sets Reason
        /// </summary>
        /// <example>&quot;This comment is inappropriate.&quot;</example>
        [DataMember(Name = "reason", EmitDefaultValue = false)]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public TicketStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or Sets Response
        /// </summary>
        /// <example>&quot;OK, deleted. Thank you for your input.&quot;</example>
        [DataMember(Name = "response", EmitDefaultValue = false)]
        public string Response { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class GetTicketDto {\n");
            sb.Append("  TicketId: ").Append(TicketId).Append("\n");
            sb.Append("  SubmitterId: ").Append(SubmitterId).Append("\n");
            sb.Append("  TargetId: ").Append(TargetId).Append("\n");
            sb.Append("  TargetType: ").Append(TargetType).Append("\n");
            sb.Append("  Reason: ").Append(Reason).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Response: ").Append(Response).Append("\n");
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
            return obj.GetType() == GetType() && Equals((GetTicketDto)obj);
        }

        /// <summary>
        /// Returns true if GetTicketDto instances are equal
        /// </summary>
        /// <param name="other">Instance of GetTicketDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(GetTicketDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    TicketId== other.TicketId ||
                    TicketId != null &&
                    TicketId.Equals(other.SubmitterId)
                ) &&
                (
                    SubmitterId == other.SubmitterId ||
                    SubmitterId != null &&
                    SubmitterId.Equals(other.SubmitterId)
                ) &&
                (
                    TargetId == other.TargetId ||
                    TargetId != null &&
                    TargetId.Equals(other.TargetId)
                ) &&
                (
                    TargetType == other.TargetType ||
                    TargetType != null &&
                    TargetType.Equals(other.TargetType)
                ) &&
                (
                    Reason == other.Reason ||
                    Reason != null &&
                    Reason.Equals(other.Reason)
                ) &&
                (
                    Status == other.Status ||
                    Status != null &&
                    Status.Equals(other.Status)
                ) &&
                (
                    Response == other.Response ||
                    Response != null &&
                    Response.Equals(other.Response)
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
                if (TicketId != null)
                    hashCode = hashCode * 59 + TicketId.GetHashCode();
                if (SubmitterId != null)
                    hashCode = hashCode * 59 + SubmitterId.GetHashCode();
                if (TargetId != null)
                    hashCode = hashCode * 59 + TargetId.GetHashCode();
                if (TargetType != null)
                    hashCode = hashCode * 59 + TargetType.GetHashCode();
                if (Reason != null)
                    hashCode = hashCode * 59 + Reason.GetHashCode();
                if (Status != null)
                    hashCode = hashCode * 59 + Status.GetHashCode();
                if (Response != null)
                    hashCode = hashCode * 59 + Response.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(GetTicketDto left, GetTicketDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GetTicketDto left, GetTicketDto right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
