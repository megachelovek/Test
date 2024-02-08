using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Test.Services;

namespace Test.Controllers
{
    //Проект собирается, но я его не тестировал. 
    [ApiController]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly TasksService _tasksService;

        public TasksController(ILogger<TasksController> logger, TasksService tasksService)
        {
            _logger = logger;
            _tasksService = tasksService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Task(CancellationToken ct)
        {
            try
            {
                var response = await _tasksService.CreateAndRunTask(ct);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return BadRequest();
        }

        [HttpGet("task/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskStatus([Required(AllowEmptyStrings = false)] string id,
            CancellationToken ct)
        {
            if (id == null)
            {
                return BadRequest();
            }
            try
            {
                var response = await _tasksService.GetTaskStatus(id, ct);
                if (response == null) return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return BadRequest();
        }
    }
}
