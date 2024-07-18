using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using MyTraceLib.Services;
using MyTraceLib.Tables;
using Microsoft.OpenApi.Models;

namespace MyTraceFunctions
{
    public interface IGetIngredientInfoFunction
    {
        Task<HttpResponseData> Run(HttpRequestData req, FunctionContext executionContext);
    }

    public class GetIngredientInfoFunction : IGetIngredientInfoFunction
    {
        private readonly ILogger _logger;

        public GetIngredientInfoFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetIngredientInfoFunction>();
        }

        [Function("GetIngredientInfo")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "ingredient" })]
        [OpenApiParameter(name: "ingredient", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The ingredient")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IngredientBreakdown), Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid request")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Read query parameter
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string ingredient = query["ingredient"];

            if (string.IsNullOrEmpty(ingredient))
            {
                using var reader = new StreamReader(req.Body);
                string requestBody = await reader.ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                ingredient = ingredient ?? data?.ingredient;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);

            if (string.IsNullOrEmpty(ingredient))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Please pass an ingredient in the query string or in the request body");
                return response;
            }

            IngredientBreakdown ingredientBreakdown = await AiService.GetIngredientBreakdownAsync(ingredient);

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(ingredientBreakdown);

            return response;
        }
    }
}
