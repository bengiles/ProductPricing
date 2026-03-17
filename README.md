# ProductPricing

A product pricing management application built with **Blazor Server** (.NET 9) and a backing **ASP.NET Core Web API**.

## Solution Structure

| Project | Description |
|---|---|
| `ProductPricing` | Blazor Server front-end (Interactive Server rendering) |
| `ProductPricing.API` | ASP.NET Core Web API back-end |
| `ProductPricing.Models` | Shared DTOs and request/response models |
| `ProductPricing.TestUnits` | Unit tests (NUnit + Moq) |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- An IDE such as [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with the C# extension

## Running the Application

Both the **API** and the **Blazor app** must be running simultaneously. The Blazor front-end calls the API at `https://localhost:7130`.

1. Open `ProductPricing.sln` in Visual Studio.
2. Right-click the solution in **Solution Explorer** → **Configure Startup Projects…**
3. Select **Multiple startup projects** and set both `ProductPricing.API` and `ProductPricing` to **Start**.
4. Press **F5** (or **Ctrl + F5** for without debugging).
