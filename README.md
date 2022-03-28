# Marten v5 siloed multi-tenancy example

This is very simplified (read: not production ready) demo of the [Multi-Tenancy with Database per Tenant](https://martendb.io/configuration/multitenancy.html#multi-tenancy-with-database-per-tenant).

## Usage

### Starting the app

```bash
cd Marten5SiloedMultiTenancy
docker-compose up -d # start postgres
dotnet run # start the app
```

### Creating a tenant

````bash
http post https://localhost:5001/tenants CompanyName=Acme
````

This will create two events:

* `TenantCreated`, in the `main` database
* `AccountCreated`, in the `tenant-<tenantId>` database