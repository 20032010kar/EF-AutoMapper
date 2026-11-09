using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using p48_lessons_asp.Data;
using p48_lessons_asp_main.Models;

namespace p48_lessons_asp.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : Controller
    {
        private readonly DataContext _context;

        public TaskController(DataContext context)
        {
            _context = context;
        }

        private IActionResult CreateResponse(int status, string message, object? data, string? error = null)
        {
            return StatusCode(status, new
            {
                status = status,
                message = message,
                data = data,
                error = error
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(bool? completed, bool? expired, bool? current)
        {
            try
            {
                var query = _context.Tasks.AsQueryable();
                var now = DateTime.UtcNow;

                if (completed.HasValue)
                {
                    query = query.Where(t => t.Completed == completed.Value);
                }

                if (expired.HasValue && expired.Value)
                {
                    query = query.Where(t => !t.Completed && t.DeadlineDate < now);
                }

                if (current.HasValue && current.Value)
                {
                    query = query.Where(t => !t.Completed && t.DeadlineDate >= now);
                }

                var result = await query.ToListAsync();
                return CreateResponse(200, "Success", result);
            }
            catch (Exception ex)
            {
                return CreateResponse(500, "Error", null, ex.Message);
            }
        }

        [HttpGet("~/api/task/{id:guid}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return CreateResponse(404, "Not Found", null, $"Task with id {id} does not exist.");
                }

                return CreateResponse(200, "Success", task);
            }
            catch (Exception ex)
            {
                return CreateResponse(500, "Error", null, ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel model)
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization") || string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
                {
                    return CreateResponse(403, "Forbidden", null, "Authentication token is missing in headers.");
                }

                if (model == null || string.IsNullOrWhiteSpace(model.Title))
                {
                    return CreateResponse(400, "Bad Request", null, "Title is required.");
                }

                if (model.DeadlineDate == default)
                {
                    return CreateResponse(400, "Bad Request", null, "Invalid deadline date provided.");
                }

                var newTask = new Tasks
                {
                    Id = Guid.NewGuid(),
                    Completed = false,
                    Title = model.Title.Trim(),
                    Description = model.Description?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    DeadlineDate = model.DeadlineDate
                };

                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync(); 

                return CreateResponse(201, "Created", newTask);
            }
            catch (Exception ex)
            {
                return CreateResponse(500, "Error", null, ex.Message);
            }
        }

        [HttpDelete("~/api/task/{id:guid}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization") || string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
                {
                    return CreateResponse(403, "Forbidden", null, "Authentication token is missing in headers.");
                }

                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return CreateResponse(404, "Not Found", null, $"Task with id {id} does not exist.");
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync(); 

                return CreateResponse(200, "Deleted successfully", task);
            }
            catch (Exception ex)
            {
                return CreateResponse(500, "Error", null, ex.Message);
            }
        }
    }

    public class TaskCreateModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DeadlineDate { get; set; }
    }
}