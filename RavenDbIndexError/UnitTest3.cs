using Raven.Client.Documents;
using Xunit;
using System.Linq;
using Raven.Client.Documents.Indexes;
using System;
using System.Security.Cryptography.X509Certificates;
using Raven.Embedded;
using System.Collections.Generic;
using RavenDBTestDriverFullExample;
using static RavenDBTestDriverFullExample.RavenDBTestDriver3;

namespace RavenDBTestDriverFullExample
{
    public class RavenDBTestDriver3
    {

        [Fact]
        public void MultipleAnySearches()
        {
            EmbeddedServer.Instance.StartServer();
            using (var store = EmbeddedServer.Instance.GetDocumentStore("Embedded"))
            {
                SetupData(store);
               // EmbeddedServer.Instance.OpenStudioInBrowser();
                

                using (var _session = store.OpenSession())
                {
                    var searchedVakanzId = "Vakanz/89d3d971-a227-430e-97ba-613799253a37";

                    var actualCount = _session.Query<Bewerbung>().Where(e => e.VakanzId == searchedVakanzId).Count();

                    var list = _session.Query<IndexResultModel, TestIndexVakanz_Works>().Where(e => e.VakanzId == searchedVakanzId).FirstOrDefault();

                    Assert.Equal(actualCount, list.Bewerbungen.Count);
                    var list2 = _session.Query<IndexResultModel, TestIndexVakanz_DoesNotWork>().Where(e => e.VakanzId == searchedVakanzId).FirstOrDefault();
                    Assert.Equal(actualCount, list2.Bewerbungen.Count);

                }
            }
        }


        private static void SetupData(IDocumentStore store)
        {
            IndexCreation.CreateIndexes(typeof(TestIndexVakanz_Works).Assembly, store);

            using (var _session = store.OpenSession())
            {
                var bewerberStatus = System.IO.File.ReadAllLines(@"CSV\BewerberStatus.csv");
                var bewerbung = System.IO.File.ReadAllLines(@"CSV\Bewerbung.csv");
                var vakanz = System.IO.File.ReadAllLines(@"CSV\Vakanz.csv");

                foreach (var line in bewerberStatus.Skip(1))
                {
                    var fields = line.Split(",");
                    var newEntity = new BewerberStatus() { Id = fields[0], Status = fields[1] };
                    _session.Store(newEntity);
                }
                foreach (var line in bewerbung.Skip(1))
                {
                    // @id,VakanzId,BewerberStatusId
                    var fields = line.Split(",");
                    var newEntity = new Bewerbung() { Id = fields[0], VakanzId = fields[1], BewerberStatusId = fields[2] };
                    _session.Store(newEntity);
                }
                foreach (var line in vakanz.Skip(1))
                {
                    // @id,SoftDeleted
                    var fields = line.Split(",");
                    var newEntity = new Vakanz() { Id = fields[0], SoftDeleted = fields[1].Equals("true", StringComparison.OrdinalIgnoreCase) };
                    _session.Store(newEntity);
                }


                _session.Store(new Bewerbung { Id = "Bewerbung/1", VakanzId = "Vakanz/DoesNotExist", BewerberStatusId = "BewerberStatus/1" });

                _session.Advanced.WaitForIndexesAfterSaveChanges();
                _session.SaveChanges();
            }
        }

        public class Vakanz
        {
            public string Id { get; set; }
            public string SomeProperty { get; set; }
            public bool SoftDeleted { get; set; }
        }

        public class Bewerbung
        {
            public string Id { get; set; }
            public string VakanzId { get; set; }
            public string BewerberStatusId { get; set; }
        }

        public class BewerberStatus
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public object SomeOtherProperty { get; set; }

        }
    }

    public class IndexResultModel
    {
        public string VakanzId { get; set; }
        public bool SoftDeleted { get; set; }
        public bool IstVakanz { get; set; }

        public List<Bewerbung> Bewerbungen { get; set; }
        public class Bewerbung
        {
            public BewerberStatus BewerberStatus { get; set; }
        }
    }

    public class TestIndexVakanz_Works : AbstractMultiMapIndexCreationTask<IndexResultModel>
    {
        public TestIndexVakanz_Works()
        {
            AddMap<Vakanz>(vakanzen => from v in vakanzen
                                       where v.SoftDeleted == false
                                       select new
                                       {
                                           IstVakanz = true,
                                           VakanzId = v.Id,

                                           SoftDeleted = v.SoftDeleted,  

                                           Bewerbungen = new IndexResultModel.Bewerbung[0],
                                           Bewerbungen_BewerberStatus_Status = default(string),

                                       });

            AddMap<Bewerbung>(bewerbung => from b in bewerbung
                                           let bewerberStatus = LoadDocument<BewerberStatus>(b.BewerberStatusId)
                                           select new
                                           {
                                               IstVakanz = false,
                                               VakanzId = b.VakanzId,

                                               SoftDeleted = false,

                                               Bewerbungen = new IndexResultModel.Bewerbung[] { new IndexResultModel.Bewerbung { BewerberStatus = bewerberStatus } },
                                               Bewerbungen_BewerberStatus_Status = bewerberStatus.Status,
                                           });


            Reduce = results => from result in results
                                where result.SoftDeleted == false
                                group result by result.VakanzId into g
                                let first = g.First()
                                select new
                                {
                                    SoftDeleted = false,
                                    IstVakanz = first.IstVakanz,
                                    VakanzId = first.VakanzId,

                                    Bewerbungen = g.SelectMany(e => e.Bewerbungen),
                                    Bewerbungen_BewerberStatus_Status = g.SelectMany(e => e.Bewerbungen).Select(e => e.BewerberStatus.Status).Distinct(),
                                };

        }


    }

    public class TestIndexVakanz_DoesNotWork : AbstractMultiMapIndexCreationTask<IndexResultModel>
    {
        public TestIndexVakanz_DoesNotWork()
        {
            AddMap<Vakanz>(vakanzen => from v in vakanzen
                                       where v.SoftDeleted == false
                                       select new
                                       {
                                           IstVakanz = true,
                                           VakanzId = v.Id,

                                           SoftDeleted = v.SoftDeleted,

                                           Bewerbungen = new IndexResultModel.Bewerbung[0],
                                           Bewerbungen_BewerberStatus_Status = default(string),

                                       });


            // The reason for "IstVakanz" is that i only want "Bewerbung"-Dokuments that still exist and are not softdeleted
            // There might be some "Bewerbung" with not existing "Vakanz" or softdeleted Vakanz
            AddMap<Bewerbung>(bewerbung => from b in bewerbung
                                           let bewerberStatus = LoadDocument<BewerberStatus>(b.BewerberStatusId)
                                           select new
                                           {
                                               IstVakanz = false,
                                               VakanzId = b.VakanzId,

                                               SoftDeleted = false,

                                               Bewerbungen = new IndexResultModel.Bewerbung[] { new IndexResultModel.Bewerbung { BewerberStatus = bewerberStatus } },
                                               Bewerbungen_BewerberStatus_Status = bewerberStatus.Status,
                                           });



            Reduce = results => from result in results
                                where result.SoftDeleted == false
                                group result by result.VakanzId into g 
                                let first = g.First(e => e.IstVakanz)   // I just want Results with existing "Vakanz"
                                where first.IstVakanz                   // with some other data, the .First() was not enough, so i had to add some extra where
                                select new
                                {
                                    SoftDeleted = false,
                                    IstVakanz = first.IstVakanz,
                                    VakanzId = first.VakanzId,

                                    Bewerbungen = g.SelectMany(e => e.Bewerbungen),
                                    Bewerbungen_BewerberStatus_Status = g.SelectMany(e => e.Bewerbungen).Select(e => e.BewerberStatus.Status).Distinct(),
                                };

        }
    }
}