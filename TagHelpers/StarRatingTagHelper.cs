using Microsoft.AspNetCore.Razor.TagHelpers;

namespace deneme.TagHelpers
{
    [HtmlTargetElement("star-rating")]
    public class StarRatingTagHelper : TagHelper
    {
        public int Rating { get; set; } = 0;
        public int MaxRating { get; set; } = 5;
        public bool ShowValue { get; set; } = false;
        public string Size { get; set; } = "sm"; // sm, md, lg

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            var classAttribute = output.Attributes["class"];
            if (classAttribute == null)
            {
                output.Attributes.Add("class", "star-rating");
            }
            else
            {
                output.Attributes.SetAttribute("class", classAttribute.Value + " star-rating");
            }

            var rating = Math.Max(0, Math.Min(Rating, MaxRating));
            var sizeClass = Size switch
            {
                "sm" => "fa-sm",
                "md" => "fa-md",
                "lg" => "fa-lg",
                _ => "fa-sm"
            };

            for (int i = 1; i <= MaxRating; i++)
            {
                var star = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("i");
                star.AddCssClass("fas fa-star");
                star.AddCssClass(sizeClass);
                
                if (i <= rating)
                {
                    star.AddCssClass("text-warning");
                }
                else
                {
                    star.AddCssClass("text-muted");
                }

                output.Content.AppendHtml(star);
            }

            if (ShowValue)
            {
                var valueSpan = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("span");
                valueSpan.AddCssClass("ms-2");
                valueSpan.InnerHtml.Append($"({rating}/{MaxRating})");
                output.Content.AppendHtml(valueSpan);
            }
        }
    }
}

