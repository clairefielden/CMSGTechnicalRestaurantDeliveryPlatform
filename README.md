# CMSGTechnical :fork_and_knife:

An ASP.NET Core (Razor Components + server hosting) app for a menu and basket flow, with MediatR handlers and EF Core data access.

## Highlights :sparkles:
- Menu browsing by category with image-backed icons.
- Add and remove menu items to basket.
- Group items in basket according to quantities.
- Slide-out basket drawer with a checkout button.
- Clean separation across UI, domain, mediator, and repository layers.

## Project Structure :compass:
- `CMSGTechnical/` - ASP.NET Core UI (Razor Components) + static assets in `wwwroot/`.
- `CMSGTechnical.Domain/` - core models and interfaces.
- `CMSGTechnical.Mediator/` - MediatR handlers and DTOs.
- `CMSGTechnical.Repository/` - EF Core data access and seed data.
- `CMSGTechnical.Mediator.Tests/` - xUnit tests for mediator logic.

## Feature Ticket Implementation Notes :memo:
- Basket persistence between page loads is handled via MediatR handlers that update the basket in the repository, and the current basket is loaded on app start in `MainLayout.razor`. With the default EF Core InMemory provider, the page can be refreshed and data is persisted.
- Mobile-friendly basket behavior comes from the drawer layout and responsive sizing in `MainLayout.razor.css`. The basket layout rules in `CMSGTechnical/Components/Shared/BasketDisplay.razor.css` also allow a variation of screen sizes.
- Menu item hover emphasis is implemented in `MenuItemDisplay.razor.css` via `.menu-item:hover` and image scale effects.
- Menu categories are rendered by grouping items and outputting headers in `Home.razor`, with category icon styling in the corresponding CSS file.
- Delivery fee and totals are computed in `Basket.cs` (`DeliveryFee` and `Total`), and displayed in `BasketDisplay.razor`.
- Basket quantity grouping is handled in `BasketDisplay.razor` by grouping `MenuItems` by id and showing a quantity with a line total.
- Test coverage scaffolding is in `CMSGTechnical.Mediator.Tests/` (xUnit).

## Getting Started :rocket:
Requirements:
- .NET 8 SDK

Run locally:
```bash
dotnet run --project CMSGTechnical/CMSGTechnical.csproj
```

Build the solution:
```bash
dotnet build CMSGTechnical.sln
```

Run tests:
```bash
dotnet test CMSGTechnical.Mediator.Tests/CMSGTechnical.Mediator.Tests.csproj
```

Run all tests with coverage:
```bash
dotnet test CMSGTechnical.sln --collect:"XPlat Code Coverage"
```

## Configuration :gear:
App settings:
- `CMSGTechnical/appsettings.json`
- `CMSGTechnical/appsettings.Development.json`

The data layer uses EF Core InMemory by default.

## UI Notes :art:
- Menu category imagery and styling live in `CMSGTechnical/Components/Pages/Home.razor(.css)`.
- Basket drawer and toggle are in `CMSGTechnical/Components/Layout/MainLayout.razor(.css)`.
- Basket item rendering is in `CMSGTechnical/Components/Shared/BasketDisplay.razor(.css)`.

## Testing Notes :test_tube:
- Unit tests live in `CMSGTechnical.Mediator.Tests/` and target MediatR handlers and DTOs.
- Tests use xUnit and validate ordering, totals, and persistence rules in the handler layer.
- Run a focused suite during development (`dotnet test CMSGTechnical.Mediator.Tests/CMSGTechnical.Mediator.Tests.csproj`) and the full solution with coverage before sharing changes (`dotnet test CMSGTechnical.sln --collect:"XPlat Code Coverage"`).

## Bootstrap Details :wrench:
Bootstrap provides the base typography, layout utilities, and form/control styling for the UI. The app ships Bootstrap as a local static asset under `CMSGTechnical/wwwroot/bootstrap/` rather than pulling from a CDN.

Implementation details:
- `CMSGTechnical/Components/App.razor` links `bootstrap/bootstrap.min.css` in the document `<head>` before `app.css` and `CMSGTechnical.styles.css` so custom styles can override defaults.
- `Program.cs` enables static file serving (`app.UseStaticFiles()`), which makes the `wwwroot/` Bootstrap assets available at runtime.
- Razor components are compiled into `CMSGTechnical.styles.css`, allowing component-level styles.

## Scalability Notes :chart_with_upwards_trend:
- Added a `BasketItem` join entity with quantities so basket growth does not rely on duplicate entity entries and scales cleanly in EF Core.
- Kept the domain, mediator, and repository layers separate so workflow changes or new data stores can evolve independently.
- Centralized basket mutations in MediatR handlers and DTO mapping to keep UI-facing shapes stable as persistence changes.
- The repository abstraction and EF Core configuration make it straightforward to swap providers (InMemory to a relational store) without refactoring handlers.

## Contributing :handshake:
Keep changes focused and match existing naming and formatting conventions. For UI tweaks, add screenshots or GIFs showing the results.
