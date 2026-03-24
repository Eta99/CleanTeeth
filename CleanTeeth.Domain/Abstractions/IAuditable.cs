namespace CleanTeeth.Domain.Abstractions
{
    public interface IAuditable
    {
        string? CreatedBy { get; set; }
        string? ModifiedBy { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }
    }
}
