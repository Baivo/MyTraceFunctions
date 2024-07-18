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
    public interface IRequestProductByBarcodeFunction
    {
        Task<HttpResponseData> Run(HttpRequestData req, FunctionContext executionContext);
    }

    public class RequestProductByBarcodeFunction : IRequestProductByBarcodeFunction
    {
        private readonly ILogger _logger;

        public RequestProductByBarcodeFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RequestProductByBarcodeFunction>();
        }

        [Function("RequestProductByBarcode")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "product" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "barcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Barcode** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(WoolworthsProduct), Description = "The product details")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for product by barcode");

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

            await response.WriteAsJsonAsync(product);
            return response;
        }
    }
}
