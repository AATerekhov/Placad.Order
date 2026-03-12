Order (Aggregate Root)
├── OrderId
├── CustomerId
├── OrderType (Enum: NewSubscription, Renewal, Upgrade, Refund)
├── OrderStatus (Pending → Confirmed → Failed)
├── BillingAddress (VO)
├── OrderLines[ ] (Entity)
│   ├── ApplicationId (VO)
│   ├── ApplicationName (snapshot)
│   ├── PlanId (VO)
│   ├── PlanName + BillingCycle (snapshot)
│   └── Price (Money VO)
├── CouponCode (VO, optional)
├── TotalAmount (Money VO)
├── PaymentReference (VO)
└── Domain Events
    ├── OrderPlaced
    ├── OrderConfirmed
    ├── OrderFailed
    └── OrderRefunded

In a separate project .net in the form of a library, build this model in the style of DDD, Aggregate root, entity, valuetype, microservice Order for e-commercial. The essence of the whole service is the sale of subscriptions to downloads and the use of desktop applications.

After that, add the controller to the basic CRUD by Order in program.cs