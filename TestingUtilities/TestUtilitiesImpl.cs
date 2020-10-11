using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Dto.Markers;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using InfraDal;
using InfraDalContracts;

namespace TestingUtilities
{
    public class TestUtilitiesImpl
    {
        public const string UserPrefix = "DUMMY.MAIL@DUMMY";
        private IInfraDal InfraDal { get; set; }
        private IDbConnection Connection { get; set; }

        public TestUtilitiesImpl(string strConn)
        {
            InfraDal = new InfraDalImpl();
            Connection = InfraDal.Connect(strConn);
        }

        public List<string> CreateUserDummyData(ISignUpService signUpService, int dataCount)
        {
            var retval = new List<string>();
            for (int i = 0; i < dataCount; i++)
            {
                var userResponse =
                    signUpService.SignUp(new SignUpRequest { Login = new LoginDto { Email = UserPrefix + i, Username = "DUMMY" + i } });
                retval.Add(((SignUpResponseOk)userResponse).SignUpRequest.Login.Email);
            }

            return retval;
        }

        public void DestroyUserDummyData(params string[] ids)
        {
            var query = new StringBuilder();
            query.Append("DELETE FROM ").Append("USERS ").Append("WHERE USERS.USER_ID IN (");
            foreach (var id in ids)
            {
                query.Append("'").Append(id).Append("'").Append(",");
            }

            query.Length--;
            query.Append(")");
            var queryStr = query.ToString();
            InfraDal.ExecuteQuery(Connection, query.ToString());
        }

        public bool GetUserById(string requestUserId)
        {
            var dataSet = InfraDal.ExecuteQuery(Connection,
                "SELECT ACTIVE FROM USERS WHERE USER_ID ='" + requestUserId + "'");
            var data = dataSet.Tables[0].Rows[0][0];
            return Convert.ToBoolean(data);
        }

        public void DeleteDocumentDummyData(List<string> ids,IDocumentService documentService)
        {
            foreach (var id in ids)
            {
                documentService.DeleteDocumentById(new DeleteDocumentRequest { DocId = id });
            }
        }

        public List<string> CreateDocumentDummyData(IDocumentService documentService,int count,string userId)
        {
            var docIdList = new List<string>();
            for (int index = 0; index < count; index++)
            {
                documentService.UploadDocument("DUMMY_DEMO_DOC" + index, "PATH" + index, userId);
            }

            var docs = documentService.GetAllDocuments(userId) as GetAllDocumentsResponseOk;
            docIdList.AddRange(docs?.Documents.Select(doc => doc.DocId));
            return docIdList;
        }

        public void DestroyMarkersDummyData(IMarkerService markerService,params string[] ids)
        {
            foreach (var id in ids)
            {
                markerService.DeleteMarker(new DeleteMarkerRequest {MarkerId = id});
            }
        }

        public Marker GetMarkerData(string docId, string docOwner, EMarkerType markerType, string color = "black", string pos = "x=100,y=100,w=2,h=4")
        {
            return new Marker
            {
                DocId = docId,
                Color = color,
                MarkerType = markerType,
                OwnerUser = docOwner,
                Position = pos
            };
        }

        public List<string> CreateDummyMarkerData(IMarkerService markerService, int dataCount,string docId,string docOwner)
        {
            var retval = new List<string>();
            for (int i = 0; i < dataCount; i++)
            {
                var marker = GetMarkerData(docId, docOwner, EMarkerType.Ellipse);
                var request = new CreateMarkerRequest { Marker = marker };
                var result = markerService.CreateMarker(request) as CreateMarkerResponseOk;
                retval.Add(result?.Request.Marker.MarkerId);
            }

            return retval;
        }
    }
}
