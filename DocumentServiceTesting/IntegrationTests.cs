using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentService;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Interface;
using DrawingDal;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SignUpService;

namespace DocumentServiceTesting
{
    public class Tests
    {
        private IDocumentService _documentService;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private IDrawingDal _drawingDal;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            _drawingDal = new DrawingDalImpl(_configuration);
            _documentService = new DocumentServiceImpl(_drawingDal);
            _signUpService = new SignUpServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _documentService = null;
            _configuration = null;
            _signUpService = null;
            _drawingDal = null;
        }

        [TestCase("35")]
        [TestCase("35-SDGHH-124124")]
        [TestCase("")]
        [TestCase(null)]
        public void TestGetDocumentByNonExistingId_GetDocumentResponseInvalidId(string id)
        {
            var expectedType = typeof(GetDocumentResponseInvalidId);
            //given a database with some demo data
            var dummyData = CreateDummyData(3);

            //when we try to get a document with non existing id
            var result = _documentService.GetDocumentById(id);

            //then we get GetDocumentResponseInvalidId as a response
            Assert.IsInstanceOf(expectedType, result);

            DeleteDummyData(dummyData);
        }

        [Test]
        public void TestGetDocumentByExistingId_GetDocumentResponseOk()
        {
            var expectedType = typeof(GetDocumentResponseOk);
            var random = new Random();
            const int dataCount = 5;
            var mock = new Mock<DocumentServiceImpl>(_drawingDal);
            mock.Protected()
                .Setup<byte[]>("GetFileBytes", ItExpr.IsAny<string>())
                .Returns(Encoding.ASCII.GetBytes("IMAGE"))
                .Verifiable();

            //given a database with some demo data
            var dummyData = CreateDummyData(dataCount);

            //when we try to get a document with non existing id
            var result = mock.Object.GetDocumentById(dummyData[random.Next(dataCount)]);

            //then we get GetDocumentResponseOk as a response
            //and the image data is equal to "IMAGE"
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf(expectedType));
                Assert.That(Encoding.ASCII.GetString(((GetDocumentResponseOk)result).Image), Is.EqualTo("IMAGE"));
            });

            mock.VerifyAll();

            DeleteDummyData(dummyData);
        }

        [TestCase("35")]
        [TestCase("35-SDGHH-124124")]
        [TestCase("")]
        [TestCase(null)]
        public void TestDeleteDocumentByNonExistingId_DeleteDocumentInvalidIdResponse(string id)
        {
            var expectedType = typeof(DeleteDocumentInvalidIdResponse);
            const int dataCount = 5;

            //given a database with some demo data
            var dummyData = CreateDummyData(dataCount);

            //when we try to delete a document with non existing id
            var request = new DeleteDocumentRequest() { DocId = id };
            var result = _documentService.DeleteDocumentById(request);

            //then we get DeleteDocumentInvalidIdResponse as a response
            Assert.That(result, Is.InstanceOf(expectedType));

            DeleteDummyData(dummyData);
        }

        [Test]
        public void TestDeleteDocumentByExistingId_DeleteDocumentResponseOk()
        {
            var expectedType = typeof(DeleteDocumentResponseOk);
            var random = new Random();
            const int dataCount = 5;
            var mock = new Mock<DocumentServiceImpl>(_drawingDal);
            mock.Protected()
                .Setup("TryDeletingFile", ItExpr.IsAny<string>())
                .Verifiable();

            //given a database with some demo data
            var dummyData = CreateDummyData(dataCount);
            var ownerDocCount =
                ((GetAllDocumentsResponseOk)_documentService.GetAllDocuments("DUMMY.MAIL@DUMMY"))
                .Documents.Count();

            //when we try to get a delete with an existing id
            var request = new DeleteDocumentRequest() { DocId = dummyData[random.Next(dataCount)] };
            var result = mock.Object.DeleteDocumentById(request);

            //then we get DeleteDocumentResponseOk as a response
            //and we have ownerDocCount -1 docs in the db
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf(expectedType));
                Assert.That(((GetAllDocumentsResponseOk)_documentService
                        .GetAllDocuments("DUMMY.MAIL@DUMMY"))
                    .Documents.Count(), Is.EqualTo(ownerDocCount - 1));
            });

            mock.VerifyAll();

            DeleteDummyData(dummyData);
        }

        [TestCase("35")]
        [TestCase("gmasd@dffmm.com")]
        [TestCase("")]
        [TestCase(null)]
        public void TestGetAllDocumentsOfNonExistingOwner_GetAllDocumentsResponseOkWithEmptyResults(string id)
        {
            var expectedType = typeof(GetAllDocumentsResponseOk);
            const int dataCount = 5;

            //given a database with some demo data
            var dummyData = CreateDummyData(dataCount);

            //when we try to get all documents with non existing id
            var result = _documentService.GetAllDocuments(id);

            //then we get GetAllDocumentsResponseOk as a response
            //and we get no data at all
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf(expectedType));
                Assert.That(((GetAllDocumentsResponseOk)result).Documents.Count(), Is.Zero);
            });

            DeleteDummyData(dummyData);
        }

        [TestCase("DUMMY.MAIL@DUMMY",5)]
        [TestCase("DUMMY.MAIL@DUMMY",15)]
        [TestCase("DUMMY.MAIL@DUMMY",0)]
        public void TestGetAllDocumentsOfExistingOwner_GetAllDocumentsResponseOkSomeResults(string id,int dataCount)
        {
            var expectedType = typeof(GetAllDocumentsResponseOk);

            //given a database with some demo data
            var dummyData = CreateDummyData(dataCount);

            //when we try to get all documents with non existing id
            var result = _documentService.GetAllDocuments(id);

            //then we get GetAllDocumentsResponseOk as a response
            //and we get dataCount amount of records
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf(expectedType));
                Assert.That(((GetAllDocumentsResponseOk)result).Documents.Count(), Is.EqualTo(dataCount));
            });

            DeleteDummyData(dummyData);
        }

        private void DeleteDummyData(List<string> ids)
        {
            foreach (var id in ids)
            {
                _documentService.DeleteDocumentById(new DeleteDocumentRequest { DocId = id });
            }
        }

        private List<string> CreateDummyData(int count)
        {
            var docIdList = new List<string>();
            //var userResponse = 
            //    _signUpService.SignUp(new SignUpRequest {Login = new LoginDto {Email = "DUMMY.MAIL@DUMMY", Username = "DUMMY"}});
            //var user = ((SignUpResponseOk) userResponse).SignUpRequest.Login;
            for (int index = 0; index < count; index++)
            {
                _documentService.UploadDocument("DUMMY_DEMO_DOC" + index, "PATH" + index, "DUMMY.MAIL@DUMMY");
            }

            var docs = _documentService.GetAllDocuments("DUMMY.MAIL@DUMMY") as GetAllDocumentsResponseOk;
            docIdList.AddRange(docs?.Documents.Select(doc => doc.DocId));
            return docIdList;
        }
    }
}