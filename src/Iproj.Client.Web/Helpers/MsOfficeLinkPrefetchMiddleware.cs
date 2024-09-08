using System.Net;
using System.Text.RegularExpressions;

namespace Iproj.Client.Web.Helpers;

public class IprojMiddleware
{
    RequestDelegate _next;

    public IprojMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        if (Is(context, HttpMethod.Get, HttpMethod.Head) && IsMsOffice(context))
        {
            // Success response indicates to Office that the link is OK.
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }
        else if (_next != null)
        {
            return _next.Invoke(context);
        }

        return Task.CompletedTask;
    }

    private static bool Is(HttpContext context, params HttpMethod[] methods)
    {
        var requestMethod = context.Request.Method;
        return methods.Any(method => StringComparer.OrdinalIgnoreCase.Equals(requestMethod, method.Method));
    }

    private static readonly Regex _UserAgent = new Regex(
        @"(^Microsoft Office\b)|([\(;]\s*ms-office\s*[;\)])",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    private static bool IsMsOffice(HttpContext context)
    {
        var headers = context.Request.Headers;

        var userAgent = headers["User-Agent"];

        return _UserAgent.IsMatch(userAgent)
            || !string.IsNullOrWhiteSpace(headers["X-Office-Major-Version"]);
    }
}