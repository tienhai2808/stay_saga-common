using System.ComponentModel.DataAnnotations;

namespace Common.DTOs;

public class PaginationQueryDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100.")]
    public int Limit { get; set; } = 10;
    
    public string Sort { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
}