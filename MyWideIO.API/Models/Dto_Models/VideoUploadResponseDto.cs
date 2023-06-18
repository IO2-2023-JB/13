using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Models.Dto_Models
{
    public class VideoUploadResponseDto : IEquatable<VideoUploadResponseDto>
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or Sets ProcessingProgress
        /// </summary>
        [DataMember(Name = "processingProgress", EmitDefaultValue = true)]
        public ProcessingProgressEnum ProcessingProgress { get; set; }

        public VideoUploadResponseDto() { } // trzeba dodac bezparametrowy jak sie jakis inny robi
        public VideoUploadResponseDto(ProcessingProgressEnum processingProgress, Guid id) // ?
        {
            ProcessingProgress = processingProgress;
            Id = id;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class VideoUploadDto {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  ProcessingProgress: ").Append(ProcessingProgress).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
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
            return obj.GetType() == GetType() && Equals((VideoUploadResponseDto)obj);
        }

        /// <summary>
        /// Returns true if VideoUploadDto instances are equal
        /// </summary>
        /// <param name="other">Instance of VideoUploadDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(VideoUploadResponseDto other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    ProcessingProgress == other.ProcessingProgress ||
                    ProcessingProgress.Equals(other.ProcessingProgress)
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

                hashCode = hashCode * 59 + Id.GetHashCode();

                hashCode = hashCode * 59 + ProcessingProgress.GetHashCode();

                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(VideoUploadResponseDto left, VideoUploadResponseDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VideoUploadResponseDto left, VideoUploadResponseDto right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators


        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
