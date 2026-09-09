using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataMatrix.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent RenderScript(this IHtmlHelper helper, string scriptName)
        {
            string scriptsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "scripts", "dist");
            var scriptSource =
                Directory.GetFiles(scriptsDirectory).
                          Select(file => new FileInfo(file).Name).
                          Single(file => file.StartsWith(scriptName, StringComparison.OrdinalIgnoreCase));

            return helper.Raw($"<script src=\"/scripts/dist/{scriptSource}\"></script>");
        }
    }
}
