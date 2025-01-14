class TodoRepository : ITodoRepository
{
    private readonly TodoAppDbContext _dbContext;

    public TodoRepository(TodoAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Todo Add(Todo todo)
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

    public IEnumerable<Todo> AddMany(IEnumerable<Todo> todos)
    {
        List<Todo> addedTodos = new();

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

    public Todo FindById(int id)
    {
        return _dbContext.Todos?.FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<Todo> FindByTitle(string title)
    {
        return _dbContext.Todos?.Where(t => t.Title.ToLowerInvariant().Contains(title.ToLowerInvariant()));
    }

    public IEnumerable<Todo> GetAll() => _dbContext.Todos;

    public Todo Update(int id, Todo todo)
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