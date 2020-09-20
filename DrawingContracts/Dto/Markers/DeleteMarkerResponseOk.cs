namespace DrawingContracts.Dto.Markers
{
    public class DeleteMarkerResponseOk:DeleteMarkerResponse
    {
        public DeleteMarkerResponseOk(DeleteMarkerRequest request)
        {
            Request = request;
        }

        public DeleteMarkerRequest Request { get; set; }
    }
}
