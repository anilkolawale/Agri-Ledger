namespace AgriLedger.Domain.Enums;

public enum CropStatus
{
    Planned = 0,
    Planted = 1,
    Growing = 2,
    ReadyForHarvest = 3,
    Harvested = 4,
    Failed = 5
}

public enum PaymentMethod
{
    Cash = 0,
    UPI = 1,
    Bank = 2,
    Cheque = 3,
    Other = 4
}

public enum PaymentStatus
{
    Pending = 0,
    Partial = 1,
    Paid = 2
}

public enum InventoryItemType
{
    Seeds = 0,
    Fertilizer = 1,
    Pesticides = 2,
    Equipment = 3,
    PackagingMaterial = 4,
    Other = 5
}

public enum ReceiptOwnerType
{
    Expense = 0,
    Income = 1,
    General = 2
}
