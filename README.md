# GraphQL and REST API Interaction Test

This project demonstrates interacting with both GraphQL and REST APIs, automating workflows, and handling common errors. The implementation is done using .NET Core WebAPI and follows the provided test instructions.

## Features

1. **Fetch Data from GraphQL API**: Retrieves a list of countries with their name, capital, and currency.
2. **Post to REST API**: Posts the details of a selected country to a REST API endpoint.
3. **Error Handling**:
   - Skips requests with `403 Forbidden` errors.
   - Retries requests with `500 Internal Server Error` using exponential backoff.
4. **Automation**: Combines fetching, processing, and posting into a seamless workflow.
5. **Data Transformation**: Saves all fetched countries into a CSV file.

## Requirements

- .NET Core SDK
- NuGet Packages:
  - `CsvHelper` (for CSV generation)
  - `System.Text.Json`

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd <repository-folder>
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Access the API**
   Use any API testing tool like Postman or cURL to call the endpoint:
   ```
   GET http://localhost:<port>/api/country/fetchcountries
   ```
   ```
   POST http://localhost:<port>/api/country/postcountry
   ```

## Workflow

1. **Fetch Data from GraphQL API**:
   - Endpoint: `https://countries.trevorblades.com/`
   - Query: Retrieves all countries with their name, capital, and currency.
2. **Post to REST API**:
   - Endpoint: `https://jsonplaceholder.typicode.com/posts`
   - Body: Includes the selected country's details.
3. **Error Handling**:
   - Logs errors to the console.
   - Retries requests for recoverable errors.
4. **Save to CSV**:
   - Outputs a file named `countries.csv` in the project root directory containing the fetched country data.

## Code Structure

- `CountryController`: Main API controller that orchestrates the workflow.
- `FetchCountries`: Fetches data from the GraphQL API.
- `PostCountry`: Posts country data to the REST API.
- `SaveCountriesToCsv`: Saves the fetched data to a CSV file.
- `Models`: Contains classes for deserializing API responses.

## Error Handling

- **403 Forbidden**: Logs the error and skips the request.
- **500 Internal Server Error**: Retries the request with exponential backoff (up to 3 attempts).

## CSV Output

- Generated CSV file includes the following columns:
  - Country Name
  - Capital
  - Currency
- File location: Project root directory, named `countries.csv`.

## Example Response

- **GraphQL Response**:
  ```json
  {
    "data": {
      "countries": [
        {
          "name": "Afghanistan",
          "capital": "Kabul",
          "currency": "AFN"
        }
      ]
    }
  }
  ```

- **REST API Post Request**:
  ```json
  {
    "title": "Country: Afghanistan",
    "body": "Capital: Kabul, Currency: AFN",
    "userId": 1
  }
  ```

- **REST API Response**:
  ```json
  {
    "id": 101
  }
  ```

## Notes

- Make sure to update the ports in the API URL based on your local environment.
- Ensure internet connectivity to access the external APIs.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

