# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PromotionsEngine is a .NET 8 microservice that calculates cashback rewards for an e-commerce ecosystem. It uses Clean Architecture with Event Sourcing patterns. This is a portfolio project demonstrating enterprise microservice design.

**Infrastructure dependencies**: Azure CosmosDB, Azure Service Bus, Redis

## Commands

```bash
# Build
dotnet build
dotnet build --configuration Release

# Run all tests
dotnet test
dotnet test --verbosity normal

# Run a specific test project
dotnet test tests/PromotionsEngine.Tests.Unit/
dotnet test tests/PromotionsEngine.Application.Tests/
dotnet test tests/PromotionsEngine.Infrastructure.Tests/
dotnet test tests/PromotionsEngine.Tests.Integration/

# Filter to a single test class or method
dotnet test --filter "ClassName=PromotionRulesEngineTests"
dotnet test --filter "MethodName~SomeMethodName"

# Run the Web API (Swagger at /swagger, base path /promotions-engine)
dotnet run --project src/PromotionsEngine/PromotionsEngine.WebApi.csproj

# Run the Service Bus Worker
dotnet run --project src/PromotionsEngine.ServiceBusWorker/PromotionsEngine.ServiceBusWorker.csproj
```

## Architecture

The solution has two application roots and three shared layers:

**Application Roots**
- `src/PromotionsEngine/` — Web API. REST CRUD for merchants, promotions, and offers. JWT Bearer auth via Azure AD.
- `src/PromotionsEngine.ServiceBusWorker/` — Background worker. Consumes Azure Service Bus queues and drives the order lifecycle command handlers.

**Shared Layers (Clean Architecture)**
- `src/PromotionsEngine.Domain/` — Pure business logic. No framework dependencies. Contains the two core engines and repository interfaces.
- `src/PromotionsEngine.Application/` — Orchestration. Command/query handlers, application services, validation, caching, regex evaluation.
- `src/PromotionsEngine.Infrastructure/` — CosmosDB repository implementations, entity/domain mappers, and CosmosDB change feed processors.

### Data Flow

Order events arrive via Service Bus → `ServiceBusWorker` dispatches them to command handlers in Application → handlers call Application services → services invoke Domain engines → results persisted via Infrastructure repositories to CosmosDB.

Offer queries (checkout/app) come via the Web API → query handlers in Application → hit Redis cache first, then CosmosDB.

### Core Domain Engines

| Class | Location | Purpose |
|---|---|---|
| `PromotionRulesEngine` | `Domain/Engines/RulesEngines/` | Evaluates whether a transaction qualifies for a promotion given its rules |
| `RewardsCalculationEngine` | `Domain/Engines/RewardsEngines/` | Computes reward amount (percent or fixed) from a qualifying transaction |

### Key Application Services

- `MerchantIdentificationService` — Identifies a merchant from raw transaction data using cached regex patterns (`MerchantRegexLookupCacheManager`)
- `OrderCreatedCommandHandler`, `OrderRefundedCommandHandler`, `OrderSettledCommandHandler` — Order lifecycle handlers
- `GetOffersForCheckoutQueryHandler`, `GetOffersForAppQueryHandler` — Offer presentation queries

### Startup / DI Wiring

Each layer has its own startup extension class registered in the application roots:
- `DomainStartup` → domain services
- `ApplicationStartup` → application services, command/query handlers, cache managers
- `InfrastructureStartup` → CosmosDB clients, repositories, change feed processors

### CosmosDB Containers

Configured in `appsettings.json` under `CosmosDb`:
- `MerchantContainer`, `PromotionsContainer`, `CustomerOrderRewardsLedgerContainer`, `PromotionSummaryContainer`, `MerchantRegexLookupContainer`
- Separate lease containers for change feed processing

### Testing

Test frameworks in use: xUnit (primary runner), Shouldly, FakeItEasy, FluentAssertions. Unit tests live in `tests/PromotionsEngine.Tests.Unit/` and cover the domain engines and helpers. Integration tests use `Microsoft.AspNetCore.Mvc.Testing`.
