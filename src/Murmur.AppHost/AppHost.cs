// Local dev orchestration only (ADR-0013). Deployment is hand-written: Dockerfile, Terraform, GitHub Actions.
var builder = DistributedApplication.CreateBuilder(args);

// Postgres: data survives restarts via a named volume. The generated password is kept in this project's user secrets.
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var database = postgres.AddDatabase("murmur");

// Azurite (Blob + Queue). Wired to the API now, first used by the Media module (M5, ADR-0017).
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(azurite => azurite.WithDataVolume());

var blobs = storage.AddBlobs("blobs");
var queues = storage.AddQueues("queues");

builder.AddProject<Projects.Murmur_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(blobs)
    .WithReference(queues)
    .WithHttpHealthCheck("/api/health");

builder.Build().Run();
