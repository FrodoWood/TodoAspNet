using Microsoft.AspNetCore.Identity;
using Todo.Database;

namespace Todo.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
