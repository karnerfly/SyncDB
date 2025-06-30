using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncHouseHero.Entities
{
  public class HouseType
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int ImageId { get; set; }
    public int DisplayOrder { get; set; }
    public bool Active { get; set; }
  }
}
