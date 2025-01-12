interface ITodoRepository 
{
    TodoItem Add(TodoItem todo);

    IEnumerable<TodoItem> GetAll();

    TodoItem FindById(int id);

    IEnumerable<TodoItem> FindByTitle(string title);

    TodoItem Update(int id, TodoItem todo);

    void Delete(int id);

    IEnumerable<TodoItem> AddMany(IEnumerable<TodoItem> todos);
}