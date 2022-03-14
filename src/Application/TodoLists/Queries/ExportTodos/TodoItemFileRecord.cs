using Prezentex.Application.Common.Mappings;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.TodoLists.Queries.ExportTodos;

public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
