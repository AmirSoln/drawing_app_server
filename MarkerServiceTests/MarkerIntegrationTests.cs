using System;
using System.Collections.Generic;
using System.Linq;
using DocumentService;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Markers;
using DrawingContracts.Interface;
using DrawingDal;
using InfraDal;
using InfraDalContracts;
using MarkerService;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SignUpService;
using TestingUtilities;

namespace MarkerServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private IMarkerService _markerService;
        private IDocumentService _documentService;

        private TestUtilitiesImpl _testUtilitiesImpl;
        private List<string> _createdUsers;
        private List<string> _createdDocuments;

        [OneTimeSetUp]
        public void FirstSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            var strConn = _configuration.GetConnectionString("mainDb");
            _testUtilitiesImpl = new TestUtilitiesImpl(strConn);
            _drawingDal = new DrawingDalImpl(_configuration);
            _documentService = new DocumentServiceImpl(_drawingDal);
            _signUpService = new SignUpServiceImpl(_drawingDal);
            _createdUsers = _testUtilitiesImpl.CreateUserDummyData(_signUpService, 1);
            _createdDocuments = _testUtilitiesImpl.CreateDocumentDummyData(_documentService, 1, _createdUsers[0]);
        }

        [OneTimeTearDown]
        public void FinalTearDown()
        {
            _testUtilitiesImpl.DestroyUserDummyData(_createdUsers.ToArray());
            _testUtilitiesImpl.DeleteDocumentDummyData(_createdDocuments, _documentService);
        }

        [SetUp]
        public void Setup()
        {
            _markerService = new MarkerServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _markerService = null;
        }

        [Test]
        public void TestCreateMarker_Success()
        {
            var expectedType = typeof(CreateMarkerResponseOk);
            var dataCount = 5;

            //given a database with some markers
            var dummyData = _testUtilitiesImpl
                .CreateDummyMarkerData(_markerService, dataCount, _createdDocuments[0], _createdUsers[0]);

            //when we create a valid marker
            var marker = _testUtilitiesImpl.GetMarkerData(_createdDocuments[0],
                _createdUsers[0], EMarkerType.Ellipse);
            var request = new CreateMarkerRequest { Marker = marker };
            var result = _markerService.CreateMarker(request);

            //then we get a CreateMarkerResponseOk
            //and the id is not null or empty
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(((CreateMarkerResponseOk)result).Request.Marker.MarkerId, Is.Not.Null);
                Assert.That(((CreateMarkerResponseOk)result).Request.Marker.MarkerId, Is.Not.Empty);
            });

            dummyData.Add(((CreateMarkerResponseOk)result).Request.Marker.MarkerId);
            _testUtilitiesImpl.DestroyMarkersDummyData(_markerService, dummyData.ToArray());
        }

        [TestCase("dfdsf-342-dsfsdf")]
        [TestCase("123")]
        [TestCase("")]
        [TestCase(null)]
        public void TestDeleteMarkerInvalidId_DeleteMarkerResponseOkAndNoMarkerDeleted(string id)
        {
            var expectedType = typeof(DeleteMarkerResponseOk);
            const int dataCount = 5;

            //given a database with some markers
            var dummyData = _testUtilitiesImpl
                .CreateDummyMarkerData(_markerService, dataCount, _createdDocuments[0], _createdUsers[0]);
            var markerIdsBeforeDelete = 
                ((GetMarkersResponseOk) _markerService.GetAllMarkers(_createdDocuments[0]))
                .Markers
                .Select(obj => obj.MarkerId)
                .ToArray();

            //when we delete marker with non existing id
            var request = new DeleteMarkerRequest(){MarkerId = id};
            var result = _markerService.DeleteMarker(request);

            //then we get a DeleteMarkerResponseOk
            //and no marker is deleted from the document
            var markerIdsAfterDelete = 
                ((GetMarkersResponseOk) _markerService.GetAllMarkers(_createdDocuments[0]))
                .Markers
                .Select(obj => obj.MarkerId)
                .ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(markerIdsAfterDelete.Length,Is.EqualTo(markerIdsBeforeDelete.Length));
            });

            _testUtilitiesImpl.DestroyMarkersDummyData(_markerService, dummyData.ToArray());
        }

        [Test]
        public void TestDeleteMarkerValidId_DeleteMarkerResponseOk()
        {
            var expectedType = typeof(DeleteMarkerResponseOk);
            const int dataCount = 5;
            var random = new Random();

            //given a database with some markers
            var dummyData = _testUtilitiesImpl
                .CreateDummyMarkerData(_markerService, dataCount, _createdDocuments[0], _createdUsers[0]);

            var markerIdsBeforeDelete = 
                ((GetMarkersResponseOk) _markerService.GetAllMarkers(_createdDocuments[0]))
                .Markers
                .Select(obj => obj.MarkerId)
                .ToArray();

            //when we delete marker with non existing id
            var request = new DeleteMarkerRequest(){MarkerId = markerIdsBeforeDelete[random.Next(markerIdsBeforeDelete.Length)]};
            var result = _markerService.DeleteMarker(request);

            //then we get a DeleteMarkerResponseOk
            //and one marker is deleted from the document
            var markerIdsAfterDelete = 
                ((GetMarkersResponseOk) _markerService.GetAllMarkers(_createdDocuments[0]))
                .Markers
                .Select(obj => obj.MarkerId)
                .ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(markerIdsAfterDelete.Length,Is.EqualTo(markerIdsBeforeDelete.Length-1));
            });

            _testUtilitiesImpl.DestroyMarkersDummyData(_markerService, dummyData.ToArray());
        }

        [Test]
        public void TestGetAllMarkers_GetAllMarkersResponseOk()
        {
            var expectedType = typeof(GetMarkersResponseOk);
            const int dataCount = 5;

            //given a database with some markers
            var dummyData = _testUtilitiesImpl
                .CreateDummyMarkerData(_markerService, dataCount, _createdDocuments[0], _createdUsers[0]);

            //when we delete marker with non existing id
            var result = _markerService.GetAllMarkers(_createdDocuments[0]);

            //then we get a GetMarkersResponseOk
            //and we get dataCount markers
            //and all ids are different
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(((GetMarkersResponseOk) result).Markers.Count(),Is.EqualTo(dataCount));
                Assert.That(((GetMarkersResponseOk) result).Markers,Is.All.Not.Null);
                Assert.That(((GetMarkersResponseOk) result).Markers.Distinct().Count(),Is.EqualTo(dataCount));
            });

            _testUtilitiesImpl.DestroyMarkersDummyData(_markerService, dummyData.ToArray());
        }


    }
}