interface ITodoRepository 
{
    Todo Add(Todo todo);

    IEnumerable<Todo> GetAll();

    Todo FindById(int id);

    IEnumerable<Todo> FindByTitle(string title);

    Todo Update(int id, Todo todo);

    void Delete(int id);

    IEnumerable<Todo> AddMany(IEnumerable<Todo> todos);
}