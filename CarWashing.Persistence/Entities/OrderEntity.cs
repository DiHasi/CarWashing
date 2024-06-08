namespace CarWashing.Persistence.Entities;

public class OrderEntity : BaseEntity
{
    public UserEntity Administrator { get; set; }
    public UserEntity Employee { get; set; }
    public CustomerCarEntity CustomerCar { get; set; }
    public int Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public List<ServiceEntity>? Services { get; set; }
}