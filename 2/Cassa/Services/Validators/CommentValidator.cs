using FluentValidation;
using lab_1.Cassa.Dtos.RequestDto;


namespace lab_1.Cassa.Services.Validators;

public class CommentValidator:AbstractValidator<CommentRequestDto>
{
    public CommentValidator()
    {
        RuleFor(x => x.Content).Length(2,2048);
    }
}