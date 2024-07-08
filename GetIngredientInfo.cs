using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using MyTraceLib.Services;
using Newtonsoft.Json;

namespace MyTraceFunctions
{
    public class GetIngredientInfo
    {
        private readonly ILogger<GetIngredientInfo> _logger;

        public GetIngredientInfo(ILogger<GetIngredientInfo> log)
        {
            _logger = log;
        }

        [FunctionName("GetIngredientInfo")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "ingredient" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "ingredient", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The ingredient")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string ingredient = req.Query["ingredient"];

            if (string.IsNullOrEmpty(ingredient))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                ingredient = ingredient ?? data?.ingredient;
            }

            if (string.IsNullOrEmpty(ingredient))
            {
                return new BadRequestObjectResult("Please pass an ingredient in the query string or in the request body");
            }

            string responseMessage = await AiService.GetIngredientBreakdownAsync(ingredient);

            return new OkObjectResult(responseMessage);
        }
    }

}
