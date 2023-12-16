using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Web.Scripts;

namespace JahanJooy.RealEstate.Web.Helpers
{
    public static class PopupHelper
    {
        public static HelperResult Menu(Func<dynamic, HelperResult> content, string popupSelector, PopupTrigger trigger = PopupTrigger.Hover, 
            PopupPosition position = PopupPosition.BottomRightAlign)
        {
            return new HelperResult(writer =>
                                    {
                                        writer.Write("<div class='clickable'");
                                        writer.Write(" data-qtip-enabled='true'");
                                        writer.Write(" data-qtip-content-selector='" + popupSelector + "'");
                                        writer.Write(" data-qtip-style-classes='qtip-menu'");
                                        writer.Write(" data-qtip-style-notip='notip'");

                                        if (trigger == PopupTrigger.Click)
                                            writer.Write(" data-qtip-show-event='click'");

                                        writer.Write(" data-qtip-position-my='" + GetMyPositionString(position) + "'");
                                        writer.Write(" data-qtip-position-at='" + GetAtPositionString(position) + "'");
                                        writer.Write(" data-qtip-show-effect='slideDown'");
                                        writer.Write(" data-qtip-hide-effect='slideUp'");

                                        writer.Write(">");
                                        content(null).WriteTo(writer);
                                        writer.Write("</div>");
                                    });
        }

        public static IHtmlString Menu(WebViewPage page, string triggerSelector, string popupSelector, PopupPosition position = PopupPosition.BottomRightAlign, bool slideEffect = true)
        {
            var script = new StringBuilder();

            script
                .Append("<script type=\"text/javascript\">")
                .Append("$(document).ready(function() {")
                .Append("  $('").Append(triggerSelector).Append("').qtip({")
                .Append("    content: { text: $('").Append(popupSelector).Append("').html() },")
                .Append("    position: { my: '").Append(GetMyPositionString(position)).Append("', at: '").Append(GetAtPositionString(position)).Append("', viewport: $(window) },")
                .Append("    hide: { ")
                .Append("      delay: 100, ")
                .Append("      event: 'unfocus mouseleave', ")
                .Append("      fixed: true, ");

            if (slideEffect)
                script.Append("      effect: function(offset) { $(this).slideUp(150); }, ");

            script
                .Append("    },") // hide
                .Append("    show: { ")
                .Append("      event: 'mouseenter', ");
            
            if (slideEffect)
                script.Append("      effect: function(offset) { $(this).slideDown(150); }, ");

            script
                .Append( "   },") // show
                .Append("    style: { classes: 'qtip-menu', tip: false }")
                .Append("  });")
                .Append("});")
                .Append("</script>");

            return page.ScriptBlock(d => new HelperResult(writer => writer.Write(script)));
        }

        public static HelperResult AjaxModal(Func<dynamic, HelperResult> content, string url, 
            string method = "POST", bool noBlur = false, bool solo = true, PopupPosition position = PopupPosition.WindowCenter,
            string onHide = null)
        {
            return new HelperResult(writer =>
                                    {
                                        writer.Write("<div class='clickable span'");
                                        writer.Write(" data-qtip-enabled='true'");
                                        writer.Write(" data-qtip-content='" + GeneralResources.PleaseWait + "'");
                                        writer.Write(" data-qtip-style-classes='qtip-modal'");
                                        writer.Write(" data-qtip-content-ajax-url='" + url + "'");
                                        writer.Write(" data-qtip-content-ajax-type='" + method + "'");
                                        writer.Write(" data-qtip-content-ajax-once=\"false\"");
                                        writer.Write(" data-qtip-show-event='click'");
                                        writer.Write(" data-qtip-show-modal-on='true'");
                                        writer.Write(" data-qtip-nohide='true'");
                                        writer.Write(" data-qtip-destroy-on-hide='true'");
                                        writer.Write(" data-qtip-position-my='" + GetMyPositionString(position) + "'");
                                        writer.Write(" data-qtip-position-at='" + GetAtPositionString(position) + "'");

                                        if (position == PopupPosition.WindowCenter)
                                            writer.Write(" data-qtip-position-target='window'");

                                        if (noBlur)
                                            writer.Write(" data-qtip-show-modal-noblur='true'");

                                        if (solo)
                                            writer.Write(" data-qtip-show-solo='true'");

                                        if (!string.IsNullOrWhiteSpace(onHide))
                                            writer.Write(" data-qtip-on-hide='" + onHide + "'");

                                        writer.Write(">");
                                        content(null).WriteTo(writer);
                                        writer.Write("</div>");
                                    });
        }

        public static IHtmlString LocalModal(WebViewPage page, string triggerSelector, string popupSelector,
            bool noBlur = false, bool noEscape = false, bool solo = true, PopupPosition position = PopupPosition.WindowCenter,
            string onHide = null)
        {
            var script = new StringBuilder();

            script
                .Append("<script type=\"text/javascript\">")
                .Append("$(document).ready(function() {")
                .Append("  $('").Append(triggerSelector).Append("').qtip({")
                .Append("    content: { text: $('").Append(popupSelector).Append("').html() },")
                .Append("    position: { ")
                .Append("      my: '").Append(GetMyPositionString(position)).Append("', ")
                .Append("      at: '").Append(GetAtPositionString(position)).Append("', ")
                .Append("      viewport: $(window), ");

            if (position == PopupPosition.WindowCenter)
                script.Append("      target: $(window), ");

            script
                .Append("    },") // position
                .Append("    hide: false,")
                .Append("    show: { ")
                .Append("      event: 'click', ");

            if (solo)
                script.Append("      solo: true, ");

            script
                .Append("      modal: { ")
                .Append("        on: true,");

            if (noBlur)
                script.Append("        blur: false,");

            if (noEscape)
                script.Append("        excape: false,");

            script
                .Append("      },") // modal
                .Append("    },") // show
                .Append("    style: { classes: 'qtip-modal', tip: false },")
                .Append("    events: { ");

            if (!string.IsNullOrWhiteSpace(onHide))
                script.Append("      hide: function(event, api) { ").Append(onHide).Append("(); },");

            script
                .Append("    },") // events
                .Append("  });")
                .Append("});")
                .Append("</script>");

            return page.ScriptBlock(d => new HelperResult(writer => writer.Write(script)));
        }

        private static string GetMyPositionString(PopupPosition position)
        {
            switch (position)
            {
                case PopupPosition.WindowCenter:
                    return "center";

                case PopupPosition.Center:
                    return "center";

                case PopupPosition.TopCenter:
                    return "bottom center";

                case PopupPosition.TopLeft:
                    return "bottom right";

                case PopupPosition.TopRight:
                    return "bottom left";

                case PopupPosition.TopLeftAlign:
                    return "bottom left";

                case PopupPosition.TopRightAlign:
                    return "bottom right";

                case PopupPosition.BottomCenter:
                    return "top center";

                case PopupPosition.BottomLeft:
                    return "top right";

                case PopupPosition.BottomRight:
                    return "top left";

                case PopupPosition.BottomLeftAlign:
                    return "top left";

                case PopupPosition.BottomRightAlign:
                    return "top right";

                case PopupPosition.RightMiddle:
                    return "left center";

                case PopupPosition.RightTopAlign:
                    return "left top";

                case PopupPosition.RightBottomAlign:
                    return "left bottom";

                case PopupPosition.LeftMiddle:
                    return "right center";

                case PopupPosition.LeftTopAlign:
                    return "right top";

                case PopupPosition.LeftBottomAlign:
                    return "right bottom";
            }

            return "center";
        }

        private static string GetAtPositionString(PopupPosition position)
        {
            switch (position)
            {
                case PopupPosition.WindowCenter:
                    return "center";

                case PopupPosition.Center:
                    return "center";

                case PopupPosition.TopCenter:
                    return "top center";

                case PopupPosition.TopLeft:
                    return "top left";

                case PopupPosition.TopRight:
                    return "top right";

                case PopupPosition.TopLeftAlign:
                    return "top left";

                case PopupPosition.TopRightAlign:
                    return "top right";

                case PopupPosition.BottomCenter:
                    return "bottom center";

                case PopupPosition.BottomLeft:
                    return "bottom left";

                case PopupPosition.BottomRight:
                    return "bottom right";

                case PopupPosition.BottomLeftAlign:
                    return "bottom left";

                case PopupPosition.BottomRightAlign:
                    return "bottom right";

                case PopupPosition.RightMiddle:
                    return "right center";

                case PopupPosition.RightTopAlign:
                    return "right top";

                case PopupPosition.RightBottomAlign:
                    return "right bottom";

                case PopupPosition.LeftMiddle:
                    return "left center";

                case PopupPosition.LeftTopAlign:
                    return "left top";

                case PopupPosition.LeftBottomAlign:
                    return "left bottom";
            }

            return "center";
        }
    }

    public enum PopupTrigger
    {
        Click,
        Hover
    }

    public enum PopupPosition
    {
        WindowCenter,
        Center,
        TopCenter,
        TopLeft,
        TopRight,
        TopLeftAlign,
        TopRightAlign,
        BottomCenter,
        BottomLeft,
        BottomRight,
        BottomLeftAlign,
        BottomRightAlign,
        RightMiddle,
        RightTopAlign,
        RightBottomAlign,
        LeftMiddle,
        LeftTopAlign,
        LeftBottomAlign
    }
}