using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ZarIran.Plugin.Misc.Core.Bases;
using ZarIran.Plugin.Misc.TorobApi.Filters;
using ZarIran.Plugin.Misc.TorobApi.Infrastructure;
using ZarIran.Plugin.Misc.TorobApi.Models;
using ZarIran.Plugin.Misc.TorobApi.Services;
using ZarIran.Plugin.Misc.TorobApi.Validators;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.TorobApi.ApiControllers
{
	[ApiController]
    [Route("api/v1/torob")]
    [ServiceFilter(typeof(IpFilterAttribute))]
    public class ProductsController : BaseApiController
    {
        #region Fields

        private readonly ITorobProductService _torobProductService;
        private readonly ITokenValidator _tokenValidator;
        private readonly TorobApiSettings _torobApiSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ProductsController(ITorobProductService torobProductService,
                                  ITokenValidator tokenValidator,
                                  TorobApiSettings torobApiSettings,
                                  ILogger logger)
        {
            _torobProductService = torobProductService;
            _tokenValidator = tokenValidator;
            _torobApiSettings = torobApiSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        //[HttpPost("products")]
        ////public async Task<IActionResult> GetProducts([FromBody] string page_Unique, [FromBody] string page_Url, [FromBody] int? page = 1)
        //public async Task<IActionResult> GetProducts([FromBody] ProductRequest request = null)
        //{
        //    try
        //    {
        //        if (request == null)
        //            request = new();

        //        if (_torobApiSettings.LogEnabled)
        //            await _logger.InformationAsync($"Torob Request: page_Unique:{request.Page_Unique} page_Url:{request.Page_Url} page:{request.Page}");

        //        ProductResponse response;

        //        if (!string.IsNullOrWhiteSpace(request.Page_Unique))
        //        {
        //            var productId = int.Parse(request.Page_Unique);

        //            response = await _torobProductService.GetProductsByIds(new List<int> { productId });
        //        }
        //        else if (!string.IsNullOrWhiteSpace(request.Page_Url))
        //        {
        //            response = await _torobProductService.GetProductsBySlugs(new List<string> { request.Page_Url });
        //        }
        //        else
        //        {
        //            response = await _torobProductService.GetAllProducts(50, request.Page ?? 1);
        //        }

        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNamingPolicy = new LowerCaseNamingPolicy(),
        //            WriteIndented = true // Optional
        //        };

        //        var json = JsonSerializer.Serialize(response, options);

        //        if (_torobApiSettings.LogEnabled)
        //            await _logger.InformationAsync($"Torob Response: {json}");

        //        return Content(json, "application/json");
        //    }
        //    catch (Exception ex)
        //    {
        //        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        //        await _logger.ErrorAsync($"page_Unique:{request.Page_Unique} page_Url:{request.Page_Url} page:{request.Page} IP Address:{ipAddress}");
        //        await _logger.ErrorAsync("Torob API Exception:" + ex.Message, ex);

        //        return StatusCode(500, new
        //        {
        //            Response = "",
        //            Error = ex.Message,
        //            Version = "1.0.0"
        //        });
        //    }
        //}


        [HttpPost("products")]
        //public async Task<IActionResult> GetProducts([FromBody] string page_Unique, [FromBody] string page_Url, [FromBody] int? page = 1)
        public async Task<IActionResult> GetProductsByFormRequest()
        {
            try
            {
                ProductRequest request = null;

                if (Request.HasFormContentType)
                {
                    if (Request.ContentType?.Contains("boundary=") == false)
                    {
                        if (_torobApiSettings.LogEnabled)
                            await _logger.InformationAsync($"Torob Request:error: request without boundry");

                        return Content("");
                    }

                    if (_torobApiSettings.LogEnabled)
                        await _logger.InformationAsync($"Torob Request: is form content");

                    // Read the form (important: use ReadFormAsync to ensure form is parsed)
                    var form = await Request.ReadFormAsync();

                    // 1) Prefer a single form field that contains JSON, e.g. "data" => "{...}"
                    if (form.TryGetValue("data", out var dataValues) && !StringValues.IsNullOrEmpty(dataValues))
                    {
                        if (_torobApiSettings.LogEnabled)
                            await _logger.InformationAsync($"Torob Request: is form content data");

                        var json = dataValues.ToString(); // StringValues -> string
                        request = JsonSerializer.Deserialize<ProductRequest>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        if (_torobApiSettings.LogEnabled)
                            await _logger.InformationAsync($"Torob Request: is form content without data");

                        // 2) Maybe client posted the model as individual fields (name, price, ...)
                        // Build a dictionary from form fields and serialize it to JSON to reuse deserializer
                        //var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        request = new ProductRequest
                        {
                        };

                        foreach (var kv in form)
                        {
                            if (_torobApiSettings.LogEnabled)
                                await _logger.InformationAsync($"Torob Request: is form content without data key:{kv.Key} value:{kv.Value}");

                            // Skip files collection (they are also in the form)
                            if (kv.Key == "file" || kv.Key == "files") continue;
                            // Only take first value for simplicity

                            //var valueStr = kv.Value.FirstOrDefault();



                            if (kv.Key.Equals("page", StringComparison.InvariantCultureIgnoreCase))
                            {
                                request.Page = int.Parse(kv.Value.FirstOrDefault());
                            }
                            if (kv.Key.Equals("page_url", StringComparison.InvariantCultureIgnoreCase))
                            {
                                request.Page_Url = kv.Value.FirstOrDefault();
                            }

                            if (kv.Key.Equals("page_unique", StringComparison.InvariantCultureIgnoreCase))
                            {
                                request.Page_Unique = kv.Value.FirstOrDefault();
                            }
                        }

                        if (_torobApiSettings.LogEnabled)
                            await _logger.InformationAsync($"Torob Request: request model after deserialize form:{JsonSerializer.Serialize(request)}");

                    }
                }
                else
                {
                    if (_torobApiSettings.LogEnabled)
                        await _logger.InformationAsync($"Torob Request: is json");

                    // application/json
                    using var reader = new StreamReader(Request.Body);
                    var body = await reader.ReadToEndAsync();

                    if (_torobApiSettings.LogEnabled)
                        await _logger.InformationAsync($"Torob Request: is json : {body}");

                    request = JsonSerializer.Deserialize<ProductRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                try
                {
                    if (request == null)
                        request = new();

                    if (_torobApiSettings.LogEnabled)
                        await _logger.InformationAsync($"Torob Request: page_Unique:{request.Page_Unique} page_Url:{request.Page_Url} page:{request.Page}");

                    ProductResponse response;

                    if (!string.IsNullOrWhiteSpace(request.Page_Unique))
                    {
                        var productId = int.Parse(request.Page_Unique);

                        response = await _torobProductService.GetProductsByIds(new List<int> { productId });
                    }
                    else if (!string.IsNullOrWhiteSpace(request.Page_Url))
                    {
                        response = await _torobProductService.GetProductsBySlugs(new List<string> { request.Page_Url });
                    }
                    else
                    {
                        response = await _torobProductService.GetAllProducts(50, request.Page ?? 1);
                    }

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                        WriteIndented = true // Optional
                    };

                    var json = JsonSerializer.Serialize(response, options);

                    if (_torobApiSettings.LogEnabled)
                        await _logger.InformationAsync($"Torob Response: {json}");

                    return Content(json, "application/json");
                }
                catch (Exception ex)
                {
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                    await _logger.ErrorAsync($"page_Unique:{request.Page_Unique} page_Url:{request.Page_Url} page:{request.Page} IP Address:{ipAddress}");
                    await _logger.ErrorAsync("Torob API Exception:" + ex.Message, ex);

                    return StatusCode(500, new
                    {
                        Response = "",
                        Error = ex.Message,
                        Version = "1.0.0"
                    });
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("Torob API Exception:" + ex.Message, ex);

                return StatusCode(500, new
                {
                    Response = "",
                    Error = ex.Message,
                    Version = "1.0.0"
                });
            }
        }

        #endregion
    }
}
