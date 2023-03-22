/*
 * VideIO API
 *
 * VideIO project API specification.
 *
 * The version of the OpenAPI document: 1.0.0
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
        /// Gets or Sets UserTypeDto
        /// </summary>
        [TypeConverter(typeof(CustomEnumConverter<UserTypeDto>))]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum UserTypeDto
        {
            
            /// <summary>
            /// Enum SimpleEnum for Simple
            /// </summary>
            [EnumMember(Value = "Simple")]
            SimpleEnum = 1,
            
            /// <summary>
            /// Enum CreatorEnum for Creator
            /// </summary>
            [EnumMember(Value = "Creator")]
            CreatorEnum = 2,
            
            /// <summary>
            /// Enum AdministratorEnum for Administrator
            /// </summary>
            [EnumMember(Value = "Administrator")]
            AdministratorEnum = 3
        }
}
