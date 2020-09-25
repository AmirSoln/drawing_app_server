using System.Collections.Generic;

namespace DrawingContracts.Dto.Documents
{
    public class GetAllDocumentsResponseOk:GetAllDocumentsResponse
    {
        public GetAllDocumentsResponseOk(string owner)
        {
            Owner = owner;
        }

        public string Owner { get; set; }
        public IEnumerable<Document> Documents { get; set; }
    }
}
