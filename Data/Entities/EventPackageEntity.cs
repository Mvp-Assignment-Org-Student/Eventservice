using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class EventPackageEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(Event))]
    public string EventId { get; set; } = null!;
    public EventEntity Event { get; set; } = null!;

    [ForeignKey(nameof(Package))]
    public int? PackageId { get; set; }
    public PackageEntity Package { get; set; } = null!;
}
