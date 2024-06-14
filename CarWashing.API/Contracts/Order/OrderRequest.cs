namespace CarWashing.Contracts.Order;

public record OrderRequest(
    int AdministratorId,
    int EmployeeId,
    int CustomerCarId,
    List<int> ServiceIds);