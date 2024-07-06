using System.ComponentModel.DataAnnotations;

namespace Todo.Models.DTOS
{
    public class TodoItemDto
    {
        [Required]
        public string? Name { get; set; }
        public bool isComplete { get; set; }
    }
}
