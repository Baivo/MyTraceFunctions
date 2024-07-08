using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using MyTraceLib;
using MyTraceLib.Services;
using MyTraceLib.Tables;
using System;

namespace MyTraceFunctions
{
    public class RequestProductByBarcode
    {
        private readonly ILogger<RequestProductByBarcode> _logger;

        public RequestProductByBarcode(ILogger<RequestProductByBarcode> log)
        {
            _logger = log;
        }

        [FunctionName("RequestProductByBarcode")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "product" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "barcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Barcode** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(WoolworthsProduct), Description = "The product details")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for product by barcode");

            string barcode = req.Query["barcode"];

            if (string.IsNullOrEmpty(barcode))
            {
                return new BadRequestObjectResult("Please pass a barcode on the query string");
            }

            WoolworthsProduct product = await WoolworthsSqlService.GetProductByBarcodeAsync(barcode);

            if (product == null)
            {
                return new NotFoundObjectResult("Product not found");
            }
                
            return new OkObjectResult(product);
        }
    }
}

