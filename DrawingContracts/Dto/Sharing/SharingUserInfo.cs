namespace DrawingContracts.Dto.Sharing
{
    public class SharingUserInfo
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public bool IsSharedWith { get; set; }
    }
}
