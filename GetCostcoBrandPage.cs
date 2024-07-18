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
    public interface IGetCostcoBrandPageFunction
    {
        Task<HttpResponseData> Run(HttpRequestData req, FunctionContext executionContext);
    }

    public class GetCostcoBrandPageFunction : IGetCostcoBrandPageFunction
    {
        private readonly ILogger _logger;
        private static readonly string _apiKey = "ca2Fg3art28TTfVRgCsm4iMaZF16WgaNkNOKO4yDc6uGc";
        //private static readonly string _apiKey = Environment.GetEnvironmentVariable("Costco_API_KEY");

        public GetCostcoBrandPageFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetCostcoBrandPageFunction>();
        }

        [Function("GetCostcoBrandPage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "brands" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The offset for pagination")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CostcoBrandResponsePage), Description = "The OK response")]
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

            CostcoBrandResponsePage brandPage;
            try
            {
                brandPage = await GetBrandPageAsync(offset);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error crawling Costco brand page: {ex.Message}");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Internal server error");
                return response;
            }

            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            await okResponse.WriteAsJsonAsync(brandPage);
            return okResponse;
        }

        private static async Task<CostcoBrandResponsePage?> GetBrandPageAsync(int offset)
        {
            using var client = new HttpClient();
            string requestUri = $"https://api.bazaarvoice.com/data/brands.json?passkey={_apiKey}&locale=en_AU&allowMissing=true&apiVersion=5.4&limit=100&offset={offset}";
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Log the response body for debugging purposes
            Console.WriteLine($"Response body: {responseBody}");

            try
            {
                return JsonConvert.DeserializeObject<CostcoBrandResponsePage>(responseBody);
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Error deserializing response: {jsonEx.Message}");
                throw;
            }
        }
    }
}
