using System.Collections.Generic;
using DIContracts.Dto;

namespace DrawingContracts.Dto.Markers
{
    public class GetMarkersResponseOk:Response
    {
        public IEnumerable<Marker> Markers { get; set; }
    }
}
