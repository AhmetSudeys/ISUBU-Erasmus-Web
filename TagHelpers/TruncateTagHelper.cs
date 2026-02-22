using Microsoft.AspNetCore.Razor.TagHelpers;

namespace deneme.TagHelpers
{
    [HtmlTargetElement("truncate")]
    public class TruncateTagHelper : TagHelper
    {
        public int Length { get; set; } = 50;
        public string Suffix { get; set; } = "...";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = output.GetChildContentAsync().Result.GetContent();
            
            if (content.Length > Length)
            {
                output.Content.SetContent(content.Substring(0, Length) + Suffix);
            }
            else
            {
                output.Content.SetContent(content);
            }
        }
    }
}

