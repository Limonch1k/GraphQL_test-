using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using DbContexts;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using ModelsDb;
using ViewModel;
using Task = ViewModel.Task;


namespace Mutations;

// Mutation.cs
public class Mutation
{
    [Authorize(Roles = new string[] { "Admin", "User" })]
    public async Task<MutationResult> CreateTask(
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] AppDbContext context,
        string title,
        string description,
        string status,
        DateTime? CreatedAt = null)
    {
        var user = httpContextAccessor.HttpContext.User;
        var username = user.Claims.Where(t => t.Type.Equals("name")).Select(x => x.Value).FirstOrDefault();
        if (username is null)
        {
            throw new GraphQLException(new Error("Forbidden", "403"));
        }
        int UserId = context.Users.AsQueryable().Where(t => t.Username.Equals(username)).Select(t => t.Id).FirstOrDefault();
        int StatusId = context.TaskStatuses.AsQueryable().Where(x => x.Description.Equals(status)).Select(x => x.StatusId).FirstOrDefault();

        if (UserId == 0)
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"User with such name {username} not found",
            };
        }

        if (StatusId == 0)
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"Task Status with such description {status} not found",
            };
        }

        var task = new TaskDb
        {
            Title = title,
            Description = description,
            CreatedBy = UserId,
            Status = StatusId,
            CreatedAt = CreatedAt ?? DateTime.Now,
            taskStatus = null
        };

        var Result = new MutationResult
        {
            Success = true,
            Message = "Task created successfully",
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return Result;
    }


    [Authorize(Roles = new string[] { "Admin", "User" })]
    public async Task<MutationResult> UpdateTask(
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] AppDbContext context,
        int id,
        string? title = null,
        string? description = null,
        string? status = null,
        DateTime? CreatedAt = null)
    {
        var user = httpContextAccessor.HttpContext.User;
        var username = user.Claims.Where(t => t.Type.Equals("name")).Select(x => x.Value).FirstOrDefault();
        var userRole = user.Claims.Where(t => t.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();
        if (username is null)
        {
            throw new GraphQLException(new Error("Forbidden", "403"));
        }
        int UserId = context.Users.AsQueryable().Where(t => t.Username.Equals(username)).Select(t => t.Id).FirstOrDefault();

        var task = await context.Tasks.FindAsync(id);

        if (task == null)
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"Task with such id {id} not found"
            };
        }

        if (task.CreatedBy != UserId && !userRole.Equals("Admin"))
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"You dont have enough rights to modify other users tasks"
            };
        }


        if (title != null) task.Title = title;
        if (description != null) task.Description = description;
        if (status != null)
        {
            int StatusId = context.TaskStatuses.AsQueryable().Where(x => x.Description.Equals(status)).Select(x => x.StatusId).FirstOrDefault();
            if (StatusId == 0)
            {
                return new MutationResult
                {
                    Success = false,
                    Message = $@"Task Status with such description {status} not found"
                };
            }

            task.Status = StatusId;
        }

        if (CreatedAt != null) task.CreatedAt = CreatedAt.Value;

        var Result = new MutationResult
        {
            Success = true,
            Message = "Task updated successfully",
        };

        await context.SaveChangesAsync();
        return Result;
    }


    [Authorize(Roles = new string[] { "Admin", "User" })]
    public async Task<MutationResult> DeleteTask(
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] AppDbContext context,
        int id)
    {
        var user = httpContextAccessor.HttpContext.User;
        var username = user.Claims.Where(t => t.Type.Equals("name")).Select(x => x.Value).FirstOrDefault();
        var userRole = user.Claims.Where(t => t.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();
        if (username is null)
        {
            throw new GraphQLException(new Error("Forbidden", "403"));
        }
        int UserId = context.Users.AsQueryable().Where(t => t.Username.Equals(username)).Select(t => t.Id).FirstOrDefault();


        var task = await context.Tasks.FindAsync(id);
        if (task == null)
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"Task with such id {id} not found"
            };
        }

        if (task.CreatedBy != UserId && !userRole.Equals("Admin"))
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"You dont have enough rights to modify other users tasks"
            };
        }

        var Result = new MutationResult
        {
            Success = true,
            Message = "Task delete successfully",
        };

        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return Result;
    }


    [Authorize(Roles = new string[] { "Admin" })]
    public async Task<MutationResult> AssignTask(
        [Service] AppDbContext context,
        int taskId,
        string username)
    {
        int userId = context.Users.AsQueryable().Where(t => t.Username.Equals(username)).Select(t => t.Id).FirstOrDefault();

        if (!context.TasksAssigned.Where(x => x.TaskId == taskId && x.UserId == userId).Any())
        {
            context.TasksAssigned.Add(new TaskAssignedDb { TaskId = taskId, UserId = userId, task = null, user = null });
        }
        if (context.TasksAssigned.Where(x => x.TaskId == taskId && x.UserId == userId).Any())
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"This user: {username} already assign to this task: {taskId} successfully",
            };
        }

        var Result = new MutationResult
        {
            Success = true,
            Message = "Task assign successfully",
        };

        await context.SaveChangesAsync();
        return Result;
    }
    

    [Authorize(Roles = new string[] { "Admin" })]
    public async Task<MutationResult> DropTaskAssign(
        [Service] AppDbContext context,
        int taskId,
        string username)
    {
        int userId = context.Users.AsQueryable().Where(t => t.Username.Equals(username)).Select(t => t.Id).FirstOrDefault();

        var task = context.TasksAssigned.Where(x => x.TaskId == taskId && x.UserId == userId).FirstOrDefault();
        if (task == null)
        {
            return new MutationResult
            {
                Success = false,
                Message = $@"TaskAssign with such TaskId {taskId} and username {username} not found"
            };
        }

        var Result = new MutationResult
        {
            Success = true,
            Message = "TaskAssign deleted successfully"
        };

        context.TasksAssigned.Remove(task);
        await context.SaveChangesAsync();
        return Result;
    }
}