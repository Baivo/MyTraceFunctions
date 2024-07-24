# MyTraceFunctions

MyTraceFunctions is an Azure Functions application that provides various HTTP-triggered functions to interact with product and brand data from different vendors (Coles, Woolworths, Costco). It includes functions for retrieving ingredient information, fetching similar products, and more.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Dependencies](#dependencies)
- [Contributing](#contributing)
- [License](#license)

## Features
- HTTP-triggered Azure Functions for vendor-specific operations.
- Fetch ingredient information and similar products.
- OpenAPI support for detailed API documentation.
- Application Insights integration for telemetry.

## Installation
1. **Clone the repository:**
    ```sh
    git clone https://github.com/yourusername/MyTraceFunctions.git
    cd MyTraceFunctions
    ```

2. **Restore dependencies:**
    ```sh
    dotnet restore
    ```

3. **Build the project:**
    ```sh
    dotnet build
    ```

## Usage
Deploy the Azure Functions to your Azure account and use the provided HTTP endpoints to interact with the functions.

## Configuration

Set the SQL connection string and API keys as environment variables:

```sh
export SQL_CONNECTION_STRING="your_sql_connection_string"
export COLES_API_KEY="your_coles_api_key"
export COSTCO_API_KEY="your_costco_api_key"
```
## API Documentation

This project uses Swagger UI for API documentation. After deploying the Azure Functions, you can access the Swagger UI to explore and test the available endpoints.

To access the Swagger UI:

1. **Deploy the Azure Functions to your Azure account.**
2. **Navigate to the Swagger UI endpoint** provided in your Azure Functions deployment.

The Swagger UI will display all available endpoints, their parameters, and response formats, allowing you to easily interact with and test the API.

## Dependencies

- .NET 8.0
- Microsoft.Azure.Functions.Worker 1.22.0
- Microsoft.Azure.Functions.Worker.Extensions.OpenApi 1.5.1
- Microsoft.Azure.Functions.Worker.Extensions.Http 3.2.0
- Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore 1.3.2
- Microsoft.Azure.Functions.Worker.Sdk 1.17.4
- Microsoft.ApplicationInsights.WorkerService 2.22.0
- Microsoft.Azure.Functions.Worker.ApplicationInsights 1.2.0

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a Pull Request.

### License
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


### Contact
For more information, please contact braydennepean@gmail.com
