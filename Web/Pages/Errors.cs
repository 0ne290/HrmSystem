using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Pages;

public static class Errors
{
    public static async Task Return500(HttpContext context)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync($"<h1>Processing of the request failed. Please provide technical support with the URL, time and request ID</h1><h2>URL: {context.Request.GetEncodedUrl()}<br/>Time: {DateTime.Now}<br/>Request ID: {context.TraceIdentifier}<br/>Technical support contact: </h2>");// Сейчас юзеру отдается время конца обработки запроса. Соответственно, будет некоторый разрыв с временем начала запроса. Если нужно будет отдать время начала, то создаешь и подключаешь в начале конвейера middleware, делающий что-то типа "HttpContext.Items["RequestTime"] = DateTime.Now" или "HttpContext.Features.Set<IHttpRequestTimeFeature>()"
    }
    
    public static ViewResult Return500(Controller controller)
    {
        var errorView = controller.View("Errors/Error500");
        errorView.StatusCode = 500;
        return errorView;
    }
}