using System.Collections.Generic;
using DIContracts.Dto;
using DrawingContracts.Dto.Documents;

namespace DrawingContracts.Dto.Sharing
{
    public class GetSharedDocumentsResponseOk:Response
    {
        public GetSharedDocumentsResponseOk(GetSharedDocumentsRequest request)
        {
            Request = request;
        }

        public GetSharedDocumentsRequest Request { get; set; }
        public IEnumerable<Document> Documents { get; set; }
    }
}
