using Marten;
using Microsoft.AspNetCore.Mvc;

namespace Marten5SiloedMultiTenancy.Controllers;

public class TenantController : Controller
{
  private readonly IDocumentStore documentStore;

  public TenantController(IDocumentStore documentStore)
  {
    this.documentStore = documentStore;
  }

  [HttpPost]
  [Route("/tenants")]
  public async Task<IActionResult> Index([FromBody] CreateTenantRequest request)
  {
    var id = Guid.NewGuid();
    var companyName = request.CompanyName;
    
    var tenantCreated = new TenantCreated(id, companyName);
    
    await using (var session = documentStore.OpenSession("main"))
    {
      session.Events.Append(id, tenantCreated);
      await session.SaveChangesAsync();
    }

    var accountCreated = new AccountCreated(id, companyName);

    await using (var session = documentStore.OpenSession($"tenant-{id}"))
    {
      session.Events.Append(id, accountCreated);
      await session.SaveChangesAsync();
    }

    return Created(new Uri($"http://localhost:5000/tenants/{id}"), null);
  }
}

public record TenantCreated(Guid Id, string CompanyName);

public record AccountCreated(Guid Id, string CompanyName);

public record CreateTenantRequest(string CompanyName);