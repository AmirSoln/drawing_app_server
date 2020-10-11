using DIContracts.Dto;
using DrawingContracts.Dto.Markers;

namespace DrawingContracts.Interface
{
    public interface IMarkerService
    {
        Response CreateMarker(CreateMarkerRequest request);
        Response DeleteMarker(DeleteMarkerRequest request);
        Response GetAllMarkers(string documentId);
        Response EditMarkerById(EditMarkerRequest request);
    }
}
