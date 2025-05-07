namespace Dawam_backend.DTOs;

public class ExecutePaymentDto
{
    public required string OrderId { get; set; }
}

public class PayPalPaymentResponse
{
    public required string ApprovalUrl { get; set; }
}