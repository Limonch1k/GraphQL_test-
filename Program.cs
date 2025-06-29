using DbContexts;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Mutations;
using Querys;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql("Host=localhost;Database=dbo;Username=postgres;Password=pg"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //токены лежат в appsettings.json
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("a-string-secret-at-least-256-bits-long"))
        };
    })
    ;

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()  
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .ModifyPagingOptions(options =>
    {
        options.ProviderName = "Offset";
        options.DefaultPageSize = 2;
        options.MaxPageSize = 5;
        options.IncludeTotalCount = true;
    })
    .AddProjections()
    .AddFiltering()
    .AddSorting();
    
builder.Services.AddHttpContextAccessor();
    

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL(); 

app.Run();
