using DbContexts;   
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using Task = ViewModel.Task;


namespace Querys;
public class Query
{

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(Roles = new string[] {"Admin"})] 
    public IQueryable<User> GetUsers([Service] AppDbContext context,[Service] IHttpContextAccessor httpContextAccessor)
    {
        return context.Users
        .Include(t => t.userRole)
        .Select(t => new User
        {
            Id = t.Id,
            Username = t.Username,
            Email = t.Email,
            Role = t.userRole.Description
        });
    }

    [UsePaging]
    [UseProjection]   
    [UseFiltering]
    [UseSorting] 
    public IQueryable<Task> GetTasks([Service] AppDbContext context)
    {
        return context.Tasks
        .Include(t => t.taskStatus)
        .Include(t => t.user)
        .Select(t => new Task
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            Status = t.taskStatus.Description,
            CreatedBy = t.user.Username
        });
    }


    [GraphQLName("GetTaskById")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task? GetTaskById([Service] AppDbContext context, int id)
    {
        return context.Tasks.AsQueryable()
        .Include(t => t.taskStatus)
        .Include(t => t.user)
        .Select(t => new Task
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            Status = t.taskStatus.Description,
            CreatedBy = t.user.Username
        })
        .FirstOrDefault(t => t.Id == id);
    }
}