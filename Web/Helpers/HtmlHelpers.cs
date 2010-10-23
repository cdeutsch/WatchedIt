using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace System.Web.Mvc {
    public static class HtmlHelpers {
        const string pubDir="/public";
        const string cssDir="css";
        const string imageDir="images";
        const string scriptDir="scripts";

        public static HtmlString DatePickerEnable(this HtmlHelper helper) {
            StringBuilder sb=new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"">$(document).ready(function() {$("".date-selector"").datepicker();});</script>" + Environment.NewLine);
            return new HtmlString(sb.ToString());
        }

        public static HtmlString Friendly(this HtmlHelper helper)
        {
            if (helper.ViewContext.HttpContext.Request.Cookies["friendly"] != null) {
                return new HtmlString(helper.h(helper.ViewContext.HttpContext.Request.Cookies["friendly"].Value));
            } else {
                return new HtmlString("");
            }
        }

        public static HtmlString Script(this HtmlHelper helper, string fileName)
        {
            if (!fileName.EndsWith(".js"))
                fileName += ".js";
            var jsPath = string.Format(@"<script src=""{0}/{1}/{2}"" ></script>" + Environment.NewLine, pubDir, scriptDir, helper.AttributeEncode(fileName));
            return new HtmlString(jsPath);
        }
        public static HtmlString CSS(this HtmlHelper helper, string fileName)
        {
            return CSS(helper, fileName, "screen");
        }
        public static HtmlString CSS(this HtmlHelper helper, string fileName, string media)
        {
            if (!fileName.EndsWith(".css"))
                fileName += ".css";
            var jsPath = string.Format(@"<link rel=""stylesheet"" type=""text/css"" href=""{0}/{1}/{2}""  media=""" + media + @""" />" + Environment.NewLine, pubDir, cssDir, helper.AttributeEncode(fileName));
            return new HtmlString(jsPath);
        }
        public static HtmlString Image(this HtmlHelper helper, string fileName)
        {
            return Image(helper, fileName, "");
        }
        public static HtmlString Image(this HtmlHelper helper, string fileName, string attributes)
        {
            fileName = string.Format("{0}/{1}/{2}", pubDir, imageDir, fileName);
            return new HtmlString(string.Format(@"<img src=""{0}"" ""{1}"" />", helper.AttributeEncode(fileName), helper.AttributeEncode(attributes)));
        }
    }
}
