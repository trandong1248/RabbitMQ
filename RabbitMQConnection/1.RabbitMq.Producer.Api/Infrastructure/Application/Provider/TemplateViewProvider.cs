using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _1.RabbitMq.Producer.Api.Infrastructure.Application.Provider
{
    public class TemplateViewProvider
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _currentDirectory;

        public TemplateViewProvider(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _currentDirectory = Directory.GetCurrentDirectory();
        }

        public async Task<string> RazorRenderToStringAsync(string viewName, object model, dynamic viewBagModel = null)
        {
            // function will find view name in folders
            // Views/+ viewName
            // Views/Shared/+ viewName
            // Pages/Shared/+ viewName

            using (var sw = new StringWriter())
            {
                var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
                return await RazorRenderToStringAsync(model, viewBagModel, actionContext, sw, viewResult);
            }
        }

        public async Task<string> GetTemplateBody(string filePath, object bindingObject)
        {
            var path = Path.Combine(_currentDirectory, filePath);
            var rawTemplateContent = await GetTemplate(path);
            return ParseModelIntoTemplate(rawTemplateContent, bindingObject);
        }

        public async Task<string> GetTemplate(string filePath)
        {
            var rawTemplateContent = string.Empty;
            using (var reader = new StreamReader(filePath))
                rawTemplateContent = await reader.ReadToEndAsync();
            return rawTemplateContent;
        }
        public string ParseTemplate(string template, object model) => ParseModelIntoTemplate(template, model);

        #region private method

        private async Task<string> RazorRenderToStringAsync(object model, dynamic viewBagModel, ActionContext actionContext, StringWriter stringWriter, ViewEngineResult viewResult)
        {
            if (viewResult.View == null) return string.Empty;

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            viewDictionary.Model = model;

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                stringWriter,
                new HtmlHelperOptions()
            );

            if (viewBagModel != null)
                viewContext.ViewBag.Data = viewBagModel;

            await viewResult.View.RenderAsync(viewContext);
            return stringWriter.ToString();
        }

        private static string ParseModelIntoTemplate(string template, object model)
        {
            if (model != null)
            {
                var properties = model.GetType().GetProperties().Select(p => p.Name).ToArray();
                foreach (var property in properties)
                {
                    var value = model.GetType().GetProperty(property).GetValue(model, null);
                    var placeholder = $"@{property}";
                    if (template.Contains(placeholder))
                        template = template.Replace(placeholder, value == null ? "" : value.ToString());
                }
            }
            return template;
        }

        #endregion private method
    }
}