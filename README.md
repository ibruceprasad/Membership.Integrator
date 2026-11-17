Membership Integrator

Overview

Membership Integrator is a .NET-based HTTP-triggered Azure Function to receive and process membership registration requests.

The solution has three projects.  An API implementation, shared data contracts, and automated tests 

Project Structure

Membership.Integrator/
│
├── Integrator/
│   └── HTTP-triggered Azure Function
│
├── Registration.Contracts/
│   └── Shared registration data contracts
│
├── Tests/
│   └── xUnit automated tests
│
└── Integrator.sln

1. Integrator

The Integrator project contains the HTTP-triggered Azure Function.

Responsibilities:

Exposes an HTTP POST endpoint for submitting membership registration data.

Accepts incoming registration requests.

Uses the shared registration contracts.

Processes registration requests and returns HTTP responses.

2. Registration.Contracts

The Registration.Contracts project is a .NET class library containing shared data contracts.

3. Tests

The validation unit test cases.


