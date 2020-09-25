using DIContracts.Dto;
using DrawingContracts.Dto.RemoveUser;

namespace DrawingContracts.Interface
{
    public interface IRemoveUserService
    {
        Response RemoveUser(RemoveUserRequest request);
    }
}
