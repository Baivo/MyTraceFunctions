using System.Collections.Generic;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyTraceLib.Services;
using MyTraceLib.Tables;
using Newtonsoft.Json;

namespace MyTraceFunctions
{
    public class GetSimilarProducts
    {
        private readonly ILogger<GetSimilarProducts> _logger;

        public GetSimilarProducts(ILogger<GetSimilarProducts> log)
        {
            _logger = log;
        }

        [FunctionName("GetSimilarProducts")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "similarproducts" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "barcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Barcode** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<WoolworthsProduct>), Description = "The similar products")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for matching products by barcode.");

            string barcode = req.Query["barcode"];

            if (string.IsNullOrEmpty(barcode))
            {
                return new BadRequestObjectResult("Please pass a barcode on the query string");
            }

            WoolworthsProduct product = await WoolworthsSqlService.GetProductByBarcodeAsync(barcode);
            List<WoolworthsProduct> matchingProducts = await WoolworthsSqlService.GetMatchingProductsAsync(product);

            if (product == null)
            {
                return new NotFoundObjectResult("Product not found");
            }
            if(matchingProducts.IsNullOrEmpty())
            {
                return new NotFoundObjectResult("No matching products found");
            }

            return new OkObjectResult(matchingProducts);
        }
    }
}

