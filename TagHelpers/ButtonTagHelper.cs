using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace deneme.TagHelpers
{
    [HtmlTargetElement("custom-button")]
    public class CustomButtonTagHelper : TagHelper
    {
        public string Type { get; set; } = "primary"; // primary, secondary, success, danger, warning, info
        public string Icon { get; set; } = "";
        public string Href { get; set; } = "#";
        public bool IsSubmit { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsSubmit)
            {
                output.TagName = "button";
                output.Attributes.Add("type", "submit");
            }
            else
            {
                output.TagName = "a";
                output.Attributes.Add("href", Href);
            }

            var classAttribute = output.Attributes["class"];
            string classes = classAttribute?.Value?.ToString() ?? "";
            classes = string.IsNullOrEmpty(classes) ? "btn" : classes + " btn";
            classes += $" btn-{Type}";
            output.Attributes.SetAttribute("class", classes.Trim());

            var content = output.GetChildContentAsync().Result.GetContent();

            if (!string.IsNullOrEmpty(Icon))
            {
                var iconTag = new TagBuilder("i");
                iconTag.AddCssClass(Icon);
                output.Content.AppendHtml(iconTag);
                if (!string.IsNullOrEmpty(content))
                {
                    output.Content.Append(" ");
                }
            }

            output.Content.Append(content);
        }
    }
}

