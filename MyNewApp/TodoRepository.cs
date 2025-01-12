class TodoRepository : ITodoRepository
{
    List<TodoItem> _todos = new();

    public TodoItem Add(TodoItem todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new Exception("Title is required.");
        }
        
        var nextId = _todos.Any() ? _todos.Max(t => t.Id) + 1 : 1;

        todo = todo with { Id = nextId };
        _todos.Add(todo);

        return todo;
    }

    public IEnumerable<TodoItem> AddMany(IEnumerable<TodoItem> todos)
    {
        List<TodoItem> addedTodos = new();

        foreach (var todo in todos)
        {
            Add(todo);
            addedTodos.Add(todo);
        }

        return addedTodos;
    }

    public void Delete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);

        if (todo is null)
        {
            throw new Exception("Todo not found.");
        }

        _todos.Remove(todo);
    }

    public TodoItem FindById(int id)
    {
        return _todos?.FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<TodoItem> FindByTitle(string title)
    {
        return _todos?.Where(t => t.Title.Contains(title));
    }

    public IEnumerable<TodoItem> GetAll() => _todos;

    public TodoItem Update(int id, TodoItem todo)
    {
        var existingTodo = _todos.FirstOrDefault(t => t.Id == id);

        if (existingTodo is null)
        {
            throw new Exception("Todo not found.");
        }

        _todos.Remove(existingTodo);
        var newTodo = todo with { Id = id };
        _todos.Add(newTodo);

        return newTodo;
    }
}