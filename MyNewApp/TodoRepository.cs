class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _dbContext;

    public TodoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TodoItem Add(TodoItem todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new Exception("Title is required.");
        }
        
        var nextId = _dbContext.Todos.Any() ? _dbContext.Todos.Max(t => t.Id) + 1 : 1;

        todo = todo with { Id = nextId };
        _dbContext.Todos.Add(todo);
        _dbContext.SaveChanges();

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
        var todo = _dbContext.Todos.FirstOrDefault(t => t.Id == id);

        if (todo is null)
        {
            throw new Exception("Todo not found.");
        }

        _dbContext.Todos.Remove(todo);
        _dbContext.SaveChanges();
    }

    public TodoItem FindById(int id)
    {
        return _dbContext.Todos?.FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<TodoItem> FindByTitle(string title)
    {
        return _dbContext.Todos?.Where(t => t.Title.Contains(title));
    }

    public IEnumerable<TodoItem> GetAll() => _dbContext.Todos;

    public TodoItem Update(int id, TodoItem todo)
    {
        var existingTodo = _dbContext.Todos.FirstOrDefault(t => t.Id == id);

        if (existingTodo is null)
        {
            throw new Exception("Todo not found.");
        }

        _dbContext.Todos.Remove(existingTodo);
        var newTodo = todo with { Id = id };
        _dbContext.Todos.Add(newTodo);
        _dbContext.SaveChanges();

        return newTodo;
    }
}