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
using TestingUtilities;

namespace DocumentServiceTesting
{
    public class Tests
    {
        private IDocumentService _documentService;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private IDrawingDal _drawingDal;
        private TestUtilitiesImpl _testUtilitiesImpl;
        private List<string> _createdUsers;

        [OneTimeSetUp]
        public void FirstSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            var strConn = _configuration.GetConnectionString("mainDb");
            _testUtilitiesImpl = new TestUtilitiesImpl(strConn);
            _drawingDal = new DrawingDalImpl(_configuration);
            _signUpService = new SignUpServiceImpl(_drawingDal);
            _createdUsers = _testUtilitiesImpl.CreateUserDummyData(_signUpService, 1);
        }

        [OneTimeTearDown]
        public void FinalTearDown()
        {
            _testUtilitiesImpl.DestroyUserDummyData(_createdUsers.ToArray());
        }

        [SetUp]
        public void Setup()
        {
            _documentService = new DocumentServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _documentService = null;
        }

        [TestCase("35")]
        [TestCase("35-SDGHH-124124")]
        [TestCase("")]
        [TestCase(null)]
        public void TestGetDocumentByNonExistingId_GetDocumentResponseInvalidId(string id)
        {
            var expectedType = typeof(GetDocumentResponseInvalidId);
            const int dataCount = 5;
            //given a database with some demo data
            var dummyData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[0]);

            //when we try to get a document with non existing id
            var result = _documentService.GetDocumentById(id);

            //then we get GetDocumentResponseInvalidId as a response
            Assert.IsInstanceOf(expectedType, result);

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyData, _documentService);
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
            var dummyData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[0]);

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

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyData, _documentService);
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
            var dummyData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[0]);

            //when we try to delete a document with non existing id
            var request = new DeleteDocumentRequest() { DocId = id };
            var result = _documentService.DeleteDocumentById(request);

            //then we get DeleteDocumentInvalidIdResponse as a response
            Assert.That(result, Is.InstanceOf(expectedType));

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyData, _documentService);
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
            var dummyData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[0]);
            var ownerDocCount =
                ((GetAllDocumentsResponseOk)_documentService.GetAllDocuments(TestUtilitiesImpl.UserPrefix + "0"))
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
                        .GetAllDocuments(TestUtilitiesImpl.UserPrefix + "0"))
                    .Documents.Count(), Is.EqualTo(ownerDocCount - 1));
            });

            mock.VerifyAll();

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyData, _documentService);
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
            var dummyData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[0]);

            //when we try to get all documents with non existing id
            var result = _documentService.GetAllDocuments(id);

            //then we get GetAllDocumentsResponseOk as a response
            //and we get no data at all
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf(expectedType));
                Assert.That(((GetAllDocumentsResponseOk)result).Documents.Count(), Is.Zero);
            });

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyData, _documentService);
        }

        [TestCaseSource(nameof(GetAllDocumentsOfExistingOwner))]
        public void TestGetAllDocumentsOfExistingOwner_GetAllDocumentsResponseOkSomeResults(string id, int dataCount)
        {
            var expectedType = typeof(GetAllDocumentsResponseOk);

            //given a database with some demo data
            var dummyData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[0]);

            //when we try to get all documents with non existing id
            var result = _documentService.GetAllDocuments(id);

            //then we get GetAllDocumentsResponseOk as a response
            //and we get dataCount amount of records
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf(expectedType));
                Assert.That(((GetAllDocumentsResponseOk)result).Documents.Count(), Is.EqualTo(dataCount));
            });

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyData, _documentService);
        }

        public static IEnumerable<TestCaseData> GetAllDocumentsOfExistingOwner
        {
            get
            {
                yield return new TestCaseData(TestUtilitiesImpl.UserPrefix + "0", 5);
                yield return new TestCaseData(TestUtilitiesImpl.UserPrefix + "0", 15);
                yield return new TestCaseData(TestUtilitiesImpl.UserPrefix + "0", 0);
            }
        }

    }
}