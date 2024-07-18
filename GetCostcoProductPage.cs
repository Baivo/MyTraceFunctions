using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
    public interface IGetCostcoProductPageFunction
    {
        Task<HttpResponseData> Run(HttpRequestData req, FunctionContext executionContext);
    }

    public class GetCostcoProductPageFunction : IGetCostcoProductPageFunction
    {
        private readonly ILogger _logger;
        private static readonly string _apiKey = "ca2Fg3art28TTfVRgCsm4iMaZF16WgaNkNOKO4yDc6uGc";
        //private static readonly string _apiKey = Environment.GetEnvironmentVariable("Costco_API_KEY");

        public GetCostcoProductPageFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetCostcoProductPageFunction>();
        }

        [Function("GetCostcoProductPage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "products" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The offset for pagination")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CostcoProductResponsePage), Description = "The OK response")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            if (!int.TryParse(query["offset"], out int offset))
            {
                offset = 0;
            }

            CostcoProductResponsePage productPage;
            try
            {
                productPage = await GetProductPageAsync(offset);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error crawling Costco product page: {ex.Message}");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }

            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            await okResponse.WriteAsJsonAsync(productPage);
            return okResponse;
        }

        private static async Task<CostcoProductResponsePage?> GetProductPageAsync(int offset)
        {
            using var client = new HttpClient();
            string requestUri = $"https://api.bazaarvoice.com/data/products.json?passkey={_apiKey}&locale=en_AU&allowMissing=true&apiVersion=5.4&limit=100&offset={offset}";
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CostcoProductResponsePage>(responseBody);
        }
    }
}
