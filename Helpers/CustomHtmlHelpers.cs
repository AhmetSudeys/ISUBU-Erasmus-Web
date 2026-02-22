using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace deneme.Helpers
{
    public static class CustomHtmlHelpers
    {

        // custom tag helper 
        // Badge Helper
        public static IHtmlContent Badge(this IHtmlHelper htmlHelper, string text, string badgeType = "primary")
        {
            var tagBuilder = new TagBuilder("span");
            tagBuilder.AddCssClass($"badge bg-{badgeType}");
            tagBuilder.InnerHtml.Append(text);
            return tagBuilder;
        }

        // Alert Helper
        public static IHtmlContent Alert(this IHtmlHelper htmlHelper, string message, string alertType = "info")
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass($"alert alert-{alertType} alert-dismissible fade show");
            tagBuilder.Attributes.Add("role", "alert");
            tagBuilder.InnerHtml.AppendHtml(message);
            
            var closeButton = new TagBuilder("button");
            closeButton.AddCssClass("btn-close");
            closeButton.Attributes.Add("type", "button");
            closeButton.Attributes.Add("data-bs-dismiss", "alert");
            closeButton.Attributes.Add("aria-label", "Close");
            
            tagBuilder.InnerHtml.AppendHtml(closeButton);
            return tagBuilder;
        }

        // Icon Button Helper
        public static IHtmlContent IconButton(this IHtmlHelper htmlHelper, string text, string iconClass, string buttonType = "primary", string action = "#")
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.AddCssClass($"btn btn-{buttonType}");
            tagBuilder.Attributes.Add("href", action);
            
            var icon = new TagBuilder("i");
            icon.AddCssClass(iconClass);
            
            tagBuilder.InnerHtml.AppendHtml(icon);
            tagBuilder.InnerHtml.Append(" " + text);
            
            return tagBuilder;
        }

        // Card Helper
        public static IHtmlContent Card(this IHtmlHelper htmlHelper, string title, IHtmlContent body, string cardClass = "")
        {
            var cardDiv = new TagBuilder("div");
            cardDiv.AddCssClass("card");
            if (!string.IsNullOrEmpty(cardClass))
            {
                cardDiv.AddCssClass(cardClass);
            }

            var headerDiv = new TagBuilder("div");
            headerDiv.AddCssClass("card-header");
            headerDiv.InnerHtml.Append(title);

            var bodyDiv = new TagBuilder("div");
            bodyDiv.AddCssClass("card-body");
            bodyDiv.InnerHtml.AppendHtml(body);

            cardDiv.InnerHtml.AppendHtml(headerDiv);
            cardDiv.InnerHtml.AppendHtml(bodyDiv);

            return cardDiv;
        }

        // Email Link Helper
        public static IHtmlContent EmailLink(this IHtmlHelper htmlHelper, string email, string displayText = null)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", $"mailto:{email}");
            tagBuilder.InnerHtml.Append(displayText ?? email);
            return tagBuilder;
        }

        //madde 10 - custom html helper tanýmlarý

        // Date Format Helper
        public static IHtmlContent FormatDate(this IHtmlHelper htmlHelper, DateTime? date, string format = "dd.MM.yyyy")
        {
            if (date.HasValue)
            {
                return new HtmlString(date.Value.ToString(format));
            }
            return new HtmlString("<span class='text-muted'>-</span>");
        }

        // Status Badge Helper
        public static IHtmlContent StatusBadge(this IHtmlHelper htmlHelper, string? status)
        {
            string badgeType = "secondary";
            string displayStatus = status ?? "Bilinmiyor";
            
            if (status != null)
            {
                var lowerStatus = status.ToLower();
                if (lowerStatus.Contains("aktif") || lowerStatus.Contains("active"))
                    badgeType = "success";
                else if (lowerStatus.Contains("pasif") || lowerStatus.Contains("passive") || lowerStatus.Contains("inactive"))
                    badgeType = "danger";
                else if (lowerStatus.Contains("beklemede") || lowerStatus.Contains("pending"))
                    badgeType = "warning";
            }

            return Badge(htmlHelper, displayStatus, badgeType);
        }
    }
}

