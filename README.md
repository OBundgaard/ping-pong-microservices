# Entity Framework (Microsoft SQL)
## Docker Setup
- Add a microservice (i.e. ASP.NET Core)
- Add a database service (i.e. MSSQL)
## Project Setup
### Program.cs
- Add a scoped service for **each** repository using the **interface** and **repository** class.
- Add the **Database Context**.
- Add the connection string in **appsettings.json**.
### NuGet Packages
- **Microsoft.EntityFrameworkCore**
- **Microsoft.EntityFrameworkCore.Design**
- **Microsoft.EntityFrameworkCore.SqlServer**
- **Microsoft.EntityFrameworkCore.Tools**
## Logic
### DB Context
This class is responsible for communication with the **Database**.
### Repositories
These classes are responsible for interaction between **Controller** and **Database Context**.

# Fault Isolation
## Project Setup
### NuGet Packages
- **Polly**
## Policies
### Retry
This policy will determine how many **retries** we do before we activate the **fallback** policy.
### Fallback
This policy will determine what we do in case all our retries fail, we could for example log using **serilog** or return a default value.
### Circuit Breaker
This policy prevents the application from making repeated calls to a failing service by **breaking** the circuit after a defined number of failures.

# Serilog
## Docker Setup
- Add the serilog service with the image **datalust/seq** and ports **5341:5341** and **5342:80**
## Project Setup
### NuGet Packages
- **Serilog**
- **Serilog.Enrichers.Span**
- **Serilog.Sinks.Console**
- **Serilog.Sinks.Seq**
## Logic
### Monitoring
This class is responsible for configuring the **logger**, this class is a global class placed in the **Core**.
### Logger
A logger is called with a given log-level whenever needed, **warning**, **error**, and **information**

# Zipkin
## Docker Setup
- Add the zipkin service with the image **openzipkin/zipkin** and ports **9411:9411**
## Project Setup
### NuGet Packages
- **OpenTelemetry**
- **OpenTelemetry.Exporter.Console**
- **OpenTelemetry.Exporter.Zipkin**
## Logic
### Monitoring
This class is responsible for configuring the **tracer** and **activity source**, this class is a global class placed in the **Core**.
### Activity
An activity is started at the start of a function inside a **controller** and **repository**, be descriptive (i.e. "Credential Repository - POST")

# RabbitMQ
## Docker Setup
## Project Setup
### Program.cs
- Add a singleton service based on **Message Client** with the **RabbitMQ** connection string - same as in **Message Handler**.
- Add a hosted service based on **Message Handler**
### NuGet Packages
- **EasyNetQ**
## Models
The models are based on a **Key-Value** format, shaped from our original data models in order to send **requests** and **responses** betwen our microservices. 
### Request
These classes will contain the **keys** we need to gather the **values** for our response (i.e. Email and Password).
### Response
These classes will contain the **values** we gathered based on the **keys** provided in the request (i.e. User ID).
## Logic
### Message Client
This class provides the ability to **send messages** and **receive messages** via RabbitMQ, the class is individual to each microservice even if the setup is identical.
### Message Handler
This class is responsible for **listening to messages** and **handling response actions** upon **receiving a message** via RabbitMQ, the class is individual to each microservice.
### Message Service
This class is responsible for communication between **Message Handler** and the **Controller** or **Repository**.