namespace SharedTodoLists.Application.Exceptions;

public class BadRequestException(string message) : Exception(message);
