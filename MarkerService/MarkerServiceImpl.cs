using System;
using System.Data;
using DIContracts.Attribute;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Markers;
using DrawingContracts.Interface;

namespace MarkerService
{
    [Register(Policy.Transient, typeof(IMarkerService))]
    public class MarkerServiceImpl : IMarkerService
    {
        private readonly IDrawingDal _drawingDal;

        public MarkerServiceImpl(IDrawingDal drawingDal)
        {
            _drawingDal = drawingDal;
        }

        public Response CreateMarker(CreateMarkerRequest request)
        {
            Response response = new CreateMarkerResponseOk(request);
            var markerId = Guid.NewGuid().ToString();
            request.Marker.MarkerId = markerId;
            try
            {
                _drawingDal.CreateMarker(request);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }

        public Response DeleteMarker(DeleteMarkerRequest request)
        {
            Response response = new DeleteMarkerResponseOk(request);
            try
            {
                _drawingDal.DeleteMarker(request.MarkerId);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }
            return response;
        }

        public Response GetAllMarkers(string documentId)
        {
            Response response = new GetMarkersResponseOk();
            try
            {
                var results = _drawingDal.GetAllMarkers(documentId);
                var markers =
                    results.Tables[0].AsEnumerable()
                        .Select(row => new Marker
                        {
                            Color = row.Field<string>("COLOR"),
                            DocId = documentId,
                            OwnerUser = row.Field<string>("OWNER_USER_ID"),
                            MarkerId = row.Field<string>("MARKER_ID"),
                            Position = row.Field<string>("POS"),
                            MarkerType = TryParseMarker((string)row["MARK_TYPE"])
                        });
                ((GetMarkersResponseOk)response).Markers = markers;
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }
            return response;
        }

        public Response EditMarkerById(EditMarkerRequest request)
        {
            Response response = new EditMarkerResponseOk(request);
            try
            {
                _drawingDal.EditMarkerById(request);
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }
            return response;
        }

        private EMarkerType TryParseMarker(string marker)
        {
            Enum.TryParse<EMarkerType>(marker, out var markerType);
            return markerType;
        }
        
    }

}
