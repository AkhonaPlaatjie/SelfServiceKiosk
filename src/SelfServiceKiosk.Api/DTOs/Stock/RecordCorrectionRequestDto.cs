using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.DTOs.Stock;

public class RecordCorrectionRequestDto
{
    public int QuantityAdjustment { get; set; }

    [Required(ErrorMessage = "A reason for the correction is required.")]
    [StringLength(200, ErrorMessage = "Reason must be at most 200 characters.")]
    public string Reason { get; set; } = string.Empty;
}
