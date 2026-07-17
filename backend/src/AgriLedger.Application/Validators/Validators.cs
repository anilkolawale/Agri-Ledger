using AgriLedger.Application.DTOs.Auth;
using AgriLedger.Application.DTOs.Crop;
using AgriLedger.Application.DTOs.Expense;
using AgriLedger.Application.DTOs.Farm;
using AgriLedger.Application.DTOs.Income;
using FluentValidation;

namespace AgriLedger.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class CreateFarmValidator : AbstractValidator<CreateFarmDto>
{
    public CreateFarmValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.AreaInAcres).GreaterThanOrEqualTo(0).When(x => x.AreaInAcres.HasValue);
    }
}

public class CreateCropValidator : AbstractValidator<CreateCropDto>
{
    public CreateCropValidator()
    {
        RuleFor(x => x.FarmId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}

public class CreateExpenseValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseValidator()
    {
        RuleFor(x => x.FarmId).GreaterThan(0);
        RuleFor(x => x.ExpenseCategoryId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.ExpenseDate).NotEmpty();
    }
}

public class CreateIncomeValidator : AbstractValidator<CreateIncomeDto>
{
    public CreateIncomeValidator()
    {
        RuleFor(x => x.FarmId).GreaterThan(0);
        RuleFor(x => x.BuyerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PricePerUnit).GreaterThan(0);
        RuleFor(x => x.SaleDate).NotEmpty();
    }
}

public class CreateLaborValidator : AbstractValidator<AgriLedger.Application.DTOs.Labor.CreateLaborDto>
{
    public CreateLaborValidator()
    {
        RuleFor(x => x.FarmId).GreaterThan(0);
        RuleFor(x => x.WorkerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Task).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DailyWage).GreaterThan(0);
        RuleFor(x => x.WorkDate).NotEmpty();
    }
}

public class CreateInventoryItemValidator : AbstractValidator<AgriLedger.Application.DTOs.Inventory.CreateInventoryItemDto>
{
    public CreateInventoryItemValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.QuantityAvailable).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReorderThreshold).GreaterThanOrEqualTo(0);
    }
}

// ---------- Update Request Validators ----------
// Inherit rules from the Create validators to maintain DRY validation constraints

public class UpdateFarmValidator : AbstractValidator<UpdateFarmDto>
{
    public UpdateFarmValidator()
    {
        Include(new CreateFarmValidator());
    }
}

public class UpdateCropValidator : AbstractValidator<UpdateCropDto>
{
    public UpdateCropValidator()
    {
        Include(new CreateCropValidator());
    }
}

public class UpdateExpenseValidator : AbstractValidator<UpdateExpenseDto>
{
    public UpdateExpenseValidator()
    {
        Include(new CreateExpenseValidator());
    }
}

public class UpdateIncomeValidator : AbstractValidator<UpdateIncomeDto>
{
    public UpdateIncomeValidator()
    {
        Include(new CreateIncomeValidator());
    }
}

public class UpdateLaborValidator : AbstractValidator<AgriLedger.Application.DTOs.Labor.UpdateLaborDto>
{
    public UpdateLaborValidator()
    {
        Include(new CreateLaborValidator());
    }
}

public class UpdateInventoryItemValidator : AbstractValidator<AgriLedger.Application.DTOs.Inventory.UpdateInventoryItemDto>
{
    public UpdateInventoryItemValidator()
    {
        Include(new CreateInventoryItemValidator());
    }
}
