namespace BackendTemplate.Shared.Models;

public class ValidationErrorResponse
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ValidationErrorResponse(string field, string message)
    {
        Field = field;
        Message = message;
    }
}
