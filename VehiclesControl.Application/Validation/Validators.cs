using FluentValidation;
using VehiclesControl.Domain.Requests;

namespace VehiclesControl.Application.Validation;

internal static class Rules
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule) => rule
        .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
        .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
        .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
        .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");

    public static IRuleBuilderOptions<T, string?> OptionalEmail<T>(this IRuleBuilder<T, string?> rule) => rule
        .EmailAddress().WithMessage("Geçerli bir e-posta girin.").MaximumLength(256);
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta girin.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur.");
    }
}

public sealed class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad zorunludur.").MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad zorunludur.").MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta girin.").MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.NewPassword!).StrongPassword().When(x => !string.IsNullOrEmpty(x.NewPassword));
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Mevcut şifrenizi girin.").When(x => !string.IsNullOrEmpty(x.NewPassword));
    }
}

public sealed class GalleryRequestValidator : AbstractValidator<GalleryRequest>
{
    public GalleryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Galeri adı zorunludur.").MaximumLength(150);
        RuleFor(x => x.City).MaximumLength(80);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Email).OptionalEmail().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.TaxNumber).MaximumLength(20);
    }
}

public sealed class CreateGalleryRequestValidator : AbstractValidator<CreateGalleryRequest>
{
    public CreateGalleryRequestValidator()
    {
        RuleFor(x => x.Gallery).NotNull().SetValidator(new GalleryRequestValidator());
        RuleFor(x => x.OwnerFirstName).NotEmpty().WithMessage("Yönetici adı zorunludur.").MaximumLength(100);
        RuleFor(x => x.OwnerLastName).NotEmpty().WithMessage("Yönetici soyadı zorunludur.").MaximumLength(100);
        RuleFor(x => x.OwnerEmail).NotEmpty().WithMessage("Yönetici e-postası zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta girin.");
        RuleFor(x => x.OwnerPhone).MaximumLength(30);
        RuleFor(x => x.OwnerPassword).NotEmpty().WithMessage("Şifre zorunludur.").StrongPassword();
    }
}

public sealed class TeamMemberRequestValidator : AbstractValidator<TeamMemberRequest>
{
    public TeamMemberRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad zorunludur.").MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad zorunludur.").MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta girin.");
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur.").StrongPassword();
        RuleFor(x => x.Role).IsInEnum();
    }
}

public sealed class VehicleRequestValidator : AbstractValidator<VehicleRequest>
{
    public VehicleRequestValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().WithMessage("Marka zorunludur.").MaximumLength(60);
        RuleFor(x => x.Model).NotEmpty().WithMessage("Model zorunludur.").MaximumLength(80);
        RuleFor(x => x.Trim).MaximumLength(120);
        RuleFor(x => x.Year).InclusiveBetween(1950, DateTime.Today.Year + 1).WithMessage("Geçerli bir model yılı girin.");
        RuleFor(x => x.Kilometers).InclusiveBetween(0, 2_000_000).WithMessage("Kilometre 0 ile 2.000.000 arasında olmalıdır.");
        RuleFor(x => x.FuelType).IsInEnum();
        RuleFor(x => x.Transmission).IsInEnum();
        RuleFor(x => x.BodyType).IsInEnum();
        RuleFor(x => x.Color).NotEmpty().WithMessage("Renk zorunludur.").MaximumLength(40);
        RuleFor(x => x.EngineVolume).InclusiveBetween(50, 10_000).When(x => x.EngineVolume.HasValue).WithMessage("Motor hacmi 50–10.000 cc arasında olmalıdır.");
        RuleFor(x => x.EnginePower).InclusiveBetween(1, 2_000).When(x => x.EnginePower.HasValue).WithMessage("Motor gücü 1–2.000 HP arasında olmalıdır.");
        RuleFor(x => x.Plate).MaximumLength(15);
        RuleFor(x => x.Vin).Length(17).When(x => !string.IsNullOrWhiteSpace(x.Vin)).WithMessage("Şasi numarası 17 karakter olmalıdır.");
        RuleFor(x => x.DamageRecordAmount).GreaterThanOrEqualTo(0).When(x => x.DamageRecordAmount.HasValue);
        RuleFor(x => x.PaintedParts).MaximumLength(500);
        RuleFor(x => x.ReplacedParts).MaximumLength(500);
        RuleFor(x => x.PurchaseDate).NotNull().WithMessage("Alış tarihi zorunludur.")
            .LessThanOrEqualTo(_ => DateTime.Today.AddDays(1)).WithMessage("Alış tarihi ileri bir tarih olamaz.");
        RuleFor(x => x.PurchasePrice).GreaterThan(0).WithMessage("Alış fiyatı sıfırdan büyük olmalıdır.");
        RuleFor(x => x.ListPrice).GreaterThan(0).WithMessage("Satış (liste) fiyatı sıfırdan büyük olmalıdır.");
        RuleFor(x => x.SellerName).MaximumLength(150);
        RuleFor(x => x.SellerPhone).MaximumLength(30);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class CustomerRequestValidator : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad zorunludur.").MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad zorunludur.").MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Telefon zorunludur.").MaximumLength(30);
        RuleFor(x => x.Email).OptionalEmail().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.City).MaximumLength(80);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class SaleRequestValidator : AbstractValidator<SaleRequest>
{
    public SaleRequestValidator()
    {
        RuleFor(x => x.VehicleId).GreaterThan(0).WithMessage("Araç seçin.");
        RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Müşteri seçin.");
        RuleFor(x => x.SaleDate).NotNull().WithMessage("Satış tarihi zorunludur.");
        RuleFor(x => x.SalePrice).GreaterThan(0).WithMessage("Satış fiyatı sıfırdan büyük olmalıdır.");
        RuleFor(x => x.Deposit).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.SalePrice).WithMessage("Kapora satış fiyatını geçemez.");
        RuleFor(x => x.PaymentMethod).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class ExpenseRequestValidator : AbstractValidator<ExpenseRequest>
{
    public ExpenseRequestValidator()
    {
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalıdır.");
        RuleFor(x => x.Date).NotNull().WithMessage("Tarih zorunludur.");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class ReminderRequestValidator : AbstractValidator<ReminderRequest>
{
    public ReminderRequestValidator()
    {
        RuleFor(x => x.VehicleId).GreaterThan(0).WithMessage("Araç seçin.");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DueDate).NotNull().WithMessage("Tarih zorunludur.");
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
