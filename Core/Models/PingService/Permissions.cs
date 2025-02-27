using System.ComponentModel.DataAnnotations;

namespace Core.Models.PingService;

public class Permissions
{
    [Key]
    public int ID { get; set; }

    [Required]
    public bool CanCreate { get; set; }

    [Required]
    public bool CanRead { get; set; }

    [Required]
    public bool CanUpdate { get; set; }

    [Required]
    public bool CanDelete { get; set; }
}
