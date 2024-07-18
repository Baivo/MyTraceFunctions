using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using MyTraceLib.Services;
using MyTraceLib.Tables;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace MyTraceFunctions
{
    public interface IGetSimilarProductsFunction
    {
        Task<HttpResponseData> Run(HttpRequestData req, FunctionContext executionContext);
    }

    public class GetSimilarProductsFunction : IGetSimilarProductsFunction
    {
        private readonly ILogger _logger;

        public GetSimilarProductsFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetSimilarProductsFunction>();
        }

        [Function("GetSimilarProducts")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "similarproducts" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "barcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Barcode** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<WoolworthsProduct>), Description = "The similar products")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for matching products by barcode.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string barcode = query["barcode"];

            var response = req.CreateResponse(HttpStatusCode.OK);

            if (string.IsNullOrEmpty(barcode))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Please pass a barcode on the query string");
                return response;
            }

            WoolworthsProduct product = await WoolworthsSqlService.GetProductByBarcodeAsync(barcode);
            if (product == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync("Product not found");
                return response;
            }

            List<WoolworthsProduct> matchingProducts = await WoolworthsSqlService.GetMatchingProductsAsync(product);
            if (matchingProducts == null || matchingProducts.Count == 0)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync("No matching products found");
                return response;
            }

            await response.WriteAsJsonAsync(matchingProducts);
            return response;
        }
    }
}
