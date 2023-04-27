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
    /// Gets or Sets SortingTypes
    /// </summary>
    [TypeConverter(typeof(CustomEnumConverter<SortingTypes>))]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum SortingTypes
    {

        /// <summary>
        /// Enum PublishDateEnum for PublishDate
        /// </summary>
        [EnumMember(Value = "PublishDate")]
        PublishDateEnum = 1,

        /// <summary>
        /// Enum AlphabeticalEnum for Alphabetical
        /// </summary>
        [EnumMember(Value = "Alphabetical")]
        AlphabeticalEnum = 2,

        /// <summary>
        /// Enum PopularityEnum for Popularity
        /// </summary>
        [EnumMember(Value = "Popularity")]
        PopularityEnum = 3
    }
}
