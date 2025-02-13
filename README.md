# PromotionEngine

A PromotionsEngine build within an Event Sourcing architecture that calculates cashback rewards based on running promotions in an E-Commerce context.

## Description

This project is a demonstration of a microservice I built as part of my professional duties. It is intended to be a portfolio piece that highlights my technical aquity building complex microservices. The overall context of this microservices is that it receives domain events from the Purchase Order stream of an E-Commerce ecosystem. There are a few major domain concepts to identify:
- Merchant
    A retailer or business where e-commerce transactions are occurring
- Promotion
    A cash rewards or discount time bound promotion running at a given merchant that will either discount the checkout price of a transaction or issue cash rewards for a transaction to be redeemed at another time.

This solution has two applications roots. One is an Azure ServiceBus Worker that pulls messages off the queue for all transactions in the Purchase Order event stream. The second is a Web API that is used to perform CRUD operations on domain entities.

When a transaction occurs and an event is processed from the service bus, the Promotions Engine will identify the merchant that the event belongs to and check the transaction details against the rules of each running promotion to determine if the transaction is eligible for a cash reward.

## Architecture

This microservice is built using the Clean Architecture Pattern with the following layers:
- Application Layer
    - This is the layer where the vast majority of the business logic lives. Events that are processed off the service bus worker or requests that are processed through the web api will be handled in the application layer.
- Domain Layer
    - This is the layer that defines all of the domain objects. This layer is encapsulated from all other layers in the microservice. The only logic that should occur in the domain layer should be self contained around manipulating domain objects. This layer should not reference any other layers in the microservice.
- Infrastructure Layer
    - This layer defines all the data access logic for the microservice. All the repos will live here. This layer is abstracted away from the Domain and Application layer so that if you need to modify your persistence technology in the future you only need to modify this layer of the application.
- ServiceBusWorker (application root)
    - This layer sends and receives messages from the service bus.
- Web API (application root)
    - This layer processes HTTP requests.

### Dependencies

The Promotions Engine leverages the following dependencies:
- Azure ServiceBus
- Azure CosmosDB
- In Cluster Redis

## License

This project is not intended for consumption. It is intended as portfolio piece to demonstrate the authors 
technical ability at building scalable enterprise level microservices.