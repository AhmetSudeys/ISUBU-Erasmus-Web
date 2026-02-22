using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace deneme.TagHelpers
{
    [HtmlTargetElement("date-format")]
    public class DateFormatTagHelper : TagHelper
    {
        public DateTime? Date { get; set; }
        public string Format { get; set; } = "dd.MM.yyyy";
        public string EmptyText { get; set; } = "-";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (Date.HasValue)
            {
                output.Content.SetContent(Date.Value.ToString(Format));
            }
            else
            {
                output.Content.SetContent(EmptyText);
                var classAttribute = output.Attributes["class"];
                if (classAttribute == null)
                {
                    output.Attributes.Add("class", "text-muted");
                }
                else
                {
                    output.Attributes.SetAttribute("class", classAttribute.Value + " text-muted");
                }
            }
        }
    }
}

