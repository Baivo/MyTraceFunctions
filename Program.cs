using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTraceFunctions;
using MyTraceLib.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() 
    .ConfigureOpenApi()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Register function classes as singletons
        services.AddSingleton<IGetIngredientInfoFunction, GetIngredientInfoFunction>();
        services.AddSingleton<IGetSimilarProductsFunction, GetSimilarProductsFunction>();
        services.AddSingleton<IRequestProductByBarcodeFunction, RequestProductByBarcodeFunction>();
        services.AddSingleton<IGetColesProductPageFunction, GetColesProductPageFunction>();
        services.AddSingleton<IGetColesBrandPageFunction, GetColesBrandPageFunction>();
        services.AddSingleton<IGetCostcoProductPageFunction, GetCostcoProductPageFunction>();
        services.AddSingleton<IGetCostcoBrandPageFunction, GetCostcoBrandPageFunction>();

    })
    .Build();

host.Run();
