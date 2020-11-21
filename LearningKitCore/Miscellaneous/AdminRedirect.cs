using System;
using System.Net;

using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;


public class AdminRedirect : IRule
{
    private readonly string adminUrl;

    public AdminRedirect(IConfiguration configuration)
    {
        // Retrieves the 'AdministrationUrl' application setting from the application's configuration providers (e.g., appsettings.json)
        adminUrl = configuration["AdministrationUrl"] ?? String.Empty;
    }

    public void ApplyRule(RewriteContext context)
    {
        if (String.IsNullOrEmpty(adminUrl))
        {
            return;
        }

        var request = context.HttpContext.Request;

        // Redirects requests leading to '~/admin' to the URL specified in the 'AdministrationUrl' setting
        // For example: https://administration.mydomain.com
        if (request.Path.Value.TrimEnd('/').Equals("/admin", StringComparison.OrdinalIgnoreCase))
        {
            var response = context.HttpContext.Response;

            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            response.Headers[HeaderNames.Location] = adminUrl;
            context.Result = RuleResult.EndResponse;
        }
    }
}
