using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace deneme.TagHelpers
{
    [HtmlTargetElement("country-flag")]
    public class CountryFlagTagHelper : TagHelper
    {
        public string? CountryName { get; set; }
        public string? FlagUrl { get; set; }
        public int Size { get; set; } = 24;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (!string.IsNullOrEmpty(FlagUrl))
            {
                var img = new TagBuilder("img");
                img.Attributes.Add("src", FlagUrl);
                img.Attributes.Add("alt", CountryName ?? "");
                img.Attributes.Add("width", Size.ToString());
                img.Attributes.Add("height", Size.ToString());
                img.Attributes.Add("style", $"width: {Size}px; height: {Size}px; object-fit: cover;");
                output.Content.AppendHtml(img);
            }
            else
            {
                output.Content.Append(CountryName ?? "");
            }
        }
    }
}

