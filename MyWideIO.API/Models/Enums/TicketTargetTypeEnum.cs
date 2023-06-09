/*
 * VideIO API
 *
 * VideIO project API specification.
 *
 * The version of the OpenAPI document: 1.0.6
 * 
 * Generated by: https://openapi-generator.tech
 */

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MyWideIO.API.Models.Enums
{
    /// <summary>
    /// Gets or Sets ProcessingProgressEnum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TicketTargetTypeEnum
    {

        [EnumMember(Value = "Video")]
        Video = 1,

        [EnumMember(Value = "User")]
        User = 2,

        [EnumMember(Value = "Playlist")]
        Playlist = 3,

        [EnumMember(Value = "Comment")]
        Comment = 4,

        [EnumMember(Value = "CommentResponse")]
        CommentResponse = 5
    }
}
