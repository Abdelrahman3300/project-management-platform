using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FullStackProject.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskItemService _taskService;

    public TasksController(ITaskItemService taskService)
    {
        _taskService = taskService;
    }

    // GET: api/projects/{projectId}/tasks
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetTasks(
        Guid projectId)
    {
        var tasks = await _taskService.GetByProjectIdAsync(projectId);

        return Ok(tasks);
    }

    // GET: api/projects/{projectId}/tasks/{taskId}
    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<TaskItemDto>> GetTask(
        Guid projectId,
        Guid taskId)
    {
        var task = await _taskService.GetByIdAsync(taskId);

        if (task is null || task.ProjectId != projectId)
            return NotFound();

        return Ok(task);
    }

    // POST: api/projects/{projectId}/tasks
    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request)
    {
        var task = await _taskService.CreateAsync(
            projectId,
            request);

        if (task is null)
            return BadRequest(
                "Project does not exist or assigned user does not exist.");

        return CreatedAtAction(
            nameof(GetTask),
            new
            {
                projectId,
                taskId = task.Id
            },
            task);
    }

    // PUT: api/projects/{projectId}/tasks/{taskId}
    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult<TaskItemDto>> UpdateTask(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskRequest request)
    {
        var existingTask = await _taskService.GetByIdAsync(taskId);

        if (existingTask is null || existingTask.ProjectId != projectId)
            return NotFound();

        var task = await _taskService.UpdateAsync(
            taskId,
            request);

        if (task is null)
            return BadRequest(
                "Assigned user does not exist.");

        return Ok(task);
    }

    // DELETE: api/projects/{projectId}/tasks/{taskId}
    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(
        Guid projectId,
        Guid taskId)
    {
        var existingTask = await _taskService.GetByIdAsync(taskId);

        if (existingTask is null || existingTask.ProjectId != projectId)
            return NotFound();

        await _taskService.DeleteAsync(taskId);

        return NoContent();
    }
}