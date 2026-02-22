using Microsoft.AspNetCore.Razor.TagHelpers;

namespace deneme.TagHelpers
{
    [HtmlTargetElement("email-mask")]
    public class EmailMaskTagHelper : TagHelper
    {
        public string Email { get; set; } = "";
        public bool ShowFull { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (string.IsNullOrEmpty(Email))
            {
                output.Content.SetContent("-");
                var classAttribute = output.Attributes["class"];
                if (classAttribute == null)
                {
                    output.Attributes.Add("class", "text-muted");
                }
                else
                {
                    output.Attributes.SetAttribute("class", classAttribute.Value + " text-muted");
                }
                return;
            }

            if (ShowFull)
            {
                var link = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("a");
                link.Attributes.Add("href", $"mailto:{Email}");
                link.InnerHtml.Append(Email);
                output.Content.AppendHtml(link);
            }
            else
            {
                // E-postayı maskeler (örn: test@example.com -> t***@example.com)
                var parts = Email.Split('@');
                if (parts.Length == 2)
                {
                    var username = parts[0];
                    var domain = parts[1];
                    
                    if (username.Length > 1)
                    {
                        var maskedUsername = username[0] + new string('*', username.Length - 1);
                        output.Content.SetContent($"{maskedUsername}@{domain}");
                    }
                    else
                    {
                        output.Content.SetContent($"*@{domain}");
                    }
                }
                else
                {
                    output.Content.SetContent(Email);
                }
            }
        }
    }
}

