TestStandOUT

This is a Web API developed in .NET 10 for managing foreign exchange rates. The system supports CRUD operations and integrates with the AlphaVantage external API.

It allows adding, listing, updating, and deleting currency pairs along with their Bid/Ask prices.

The API first queries the local database (SQLite). If the requested rate does not exist, it fetches the data from AlphaVantage and stores it for future use.

Swagger/OpenAPI is configured for quick and easy testing.

CI/CD

A pipeline is configured using GitHub Actions to automate build and testing processes.

Technologies Used
ASP.NET Core Web API
SQLite
Entity Framework Core (EF Core)
Swashbuckle (Swagger)
xUnit
Visual Studio 2022 or 2026 (or VS Code)
How to Run the Application

Clone the repository:

git clone https://github.com/seu-usuario/TestStandOUT.git

Open the solution:
Open the TestStandOUT.sln file in Visual Studio.

Run the application:
Press F5. The SQLite database (fxrates.db) will be created automatically on the first run.

Access Swagger:
The page will open automatically at:
https://localhost:XXXX/swagger


Running Unit Tests
You can run tests using Visual Studio Test Explorer or the command:
dotnet test


Technical Decisions

I chose SQLite instead of SQL Server for this technical challenge due to its portability. The database is a local file that “just works,” without requiring the evaluator to configure a full database server instance.

The database acts as a fast persistence layer, reducing latency for end users and minimizing external API calls (helping avoid AlphaVantage rate limits).

A critical aspect of financial systems is decimal formatting. I used CultureInfo.InvariantCulture for all decimal parsing to ensure the system behaves consistently, even on servers configured with different locales (e.g., Portuguese, which uses commas instead of dots).

CI/CD Details

The .github/workflows/main.yml file is configured so that every push to the main branch automatically triggers build and unit tests, ensuring that the delivered code is always functional.

Future Improvements

TTL (Time To Live):
Currently, if data exists in the database, it is returned as-is. A possible improvement would be adding an ExpirationDate field to automatically refresh outdated exchange rates.

The application implements an Asynchronous Messaging pattern using MassTransit to demonstrate decoupled architecture.

Author

Developed by: Renan Duarte
Date: March 2026
