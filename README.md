# BlazorBookApp
A Blazor WebAssembly app with .NET backend that allows users to:

Search books by title using Google Books API v1.
View details such as cover, description, and subjects.
Keep track of the 5 most recent searches.
Includes unit tests for services and UI components with xUnit, bUnit, and Moq.

Tech Stack

.NET 8.0
- Blazor WebAssembly (Client)
- ASP.NET Core Web API (Server)
- Google Books API v1 (Data source)
- xUnit + bUnit + Moq (Testing)


# Setup Instructions
# Prerequisites
- .NET 8 SDK installed


# Run the App
dotnet build
dotnet run --project BlazorBookApp.Server

App will be available at: https://localhost:7165

# Project Structure
BlazorBookApp.sln
 - BlazorBookApp.Client            # Blazor WebAssembly (UI)
 - BlazorBookApp.Server            # ASP.NET Core API
 - BlazorBookApp.Shared            # Shared DTOs & Models
 - BlazorBookApp.Server.Tests      # Unit tests for services and controller
 - BlazorBookApp.Shared.Tests      # Unit tests for result pattern
 - BlazorBookApp.Client.Tests      # Unit tests for services
 - BlazorBookApp.Client.BUnitTests # Component tests with bUnit

# NOTES
- Uses Google Books API v1 for book search and details.
- Recent searches are stored locally.
- Gracefully handles empty queries, network issues, and API errors.
- Unit tests cover both business logic and UI components.
