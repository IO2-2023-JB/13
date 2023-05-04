using Microsoft.AspNetCore.Mvc.Formatters;

namespace MyWideIO.API.Formatters
{
    // Input Type Formatter to allow model binding to Streams
    public class InputFormatterStream : InputFormatter
    {
        public InputFormatterStream()
        {
            SupportedMediaTypes.Add("application/octet-stream");
            SupportedMediaTypes.Add("image/jpeg");
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(Stream);

        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            return InputFormatterResult.SuccessAsync(context.HttpContext.Request.Body);
        }
    }
}