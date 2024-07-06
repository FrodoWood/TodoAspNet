using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Database;
using Todo.Models;
using Todo.Models.DTOS;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TodoItemsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var userId = _userManager.GetUserId(User);
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDto todoItemDto)
        {
            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem == null || userId == null)
            {
                return BadRequest();
            }

            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.isComplete;

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id, userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItemDto todoItemDto)
        {
            var userId = _userManager.GetUserId(User);

            if(userId == null)
            {
                return BadRequest();
            }

            var todoItem = new TodoItem
            {
                Name = todoItemDto.Name,
                IsComplete = todoItemDto.isComplete,
                UserId = userId,
                User = await _context.Users.FindAsync(userId)
            };


            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var userId = _userManager.GetUserId(User);

            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id, string userId)
        {
            return _context.TodoItems.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}
