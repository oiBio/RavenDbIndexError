//using Raven.Client.Documents;
//using Xunit;
//using System.Linq;
//using Raven.Client.Documents.Indexes;
//using System;
//using System.Security.Cryptography.X509Certificates;
//using Raven.Embedded;
//using System.Collections.Generic;
//using RavenDBTestDriverFullExample;
//using static RavenDBTestDriverFullExample.RavenDBTestDriver2;

//namespace RavenDBTestDriverFullExample
//{
//    public class RavenDBTestDriver2
//    {

//        [Fact]
//        public void MultipleAnySearches()
//        {
//            EmbeddedServer.Instance.StartServer();
//            using (var store = EmbeddedServer.Instance.GetDocumentStore("Embedded"))
//            {
//                SetupData(store);

//                using (var _session = store.OpenSession())
//                {
//                    var stichtag = new DateTime(2020, 5, 1);
//                    var akitvMitZwei = _session.Query<TestIndex.Result, TestIndex>()
//                        .Where(e => e.Details.Any(e => e.PortalAktiv && e.Start <= stichtag && e.Ende >= stichtag))
//                        .ToList();

//                    Assert.Equal(2, akitvMitZwei.Count);

//                    Assert.Equal(akitvMitZwei
//                        .Where(e => e.Details.Any(e => e.PortalAktiv))
//                        .Where(e => e.Details.Any(e => e.Start <= stichtag && e.Ende >= stichtag)).Count()
//                        , akitvMitZwei.Count);

//                    stichtag = new DateTime(2020, 8, 1);
//                    var akitvMitZwe2 = _session.Query<TestIndex.Result, TestIndex>()
//                        .Where(e => e.Details.Any(e => e.Start <= stichtag && e.Ende >= stichtag))
//                        .ToList();




//                    var stichtag2 = new DateTime(2019, 5, 15);
//                    var stichtagLeer = _session.Query<TestIndex.Result, TestIndex>()
//                        .Where(e => e.Details.Any(e => e.PortalAktiv && e.Start <= stichtag2 && e.Ende >= stichtag2))
//                        .ToList();




//                    Assert.Empty(stichtagLeer);

//                }
//            }
//        }


//        [Fact]
//        public void SearchNestedArray()
//        {
//            EmbeddedServer.Instance.StartServer();
//            using (var store = EmbeddedServer.Instance.GetDocumentStore("Embedded"))
//            {
//                SetupData(store);

//                using (var _session = store.OpenSession())
//                {
//                    var aktive = _session.Query<TestIndex.Result, TestIndex>()
//                        .Where(e => e.Details.Any(e => e.PortalAktiv))
//                        .ToList();  // Should be Vakanz/1 and Vakanz/2

//                    var inaktive = _session.Query<TestIndex.Result, TestIndex>()
//                        .Where(e => !e.Details.Any(e => e.PortalAktiv))
//                        .ToList();  // Should be Vakanz/3

//                    Assert.Equal(2, aktive.Count);
//                    Assert.Single(inaktive);
//                }
//            }
//        }

//        private static void SetupData(IDocumentStore store)
//        {
//            IndexCreation.CreateIndexes(typeof(TestIndex).Assembly, store);


//            using (var _session = store.OpenSession())
//            {
//                // Fill Database
//                _session.Store(new Vakanz() { Id = "Vakanz/1", SomeProperty = "Vakanz 1" });
//                _session.Store(new Vakanz() { Id = "Vakanz/2", SomeProperty = "Vakanz 2" });
//                _session.Store(new Vakanz() { Id = "Vakanz/3", SomeProperty = "Vakanz 3" });

//                var portalAktiv = new Portal { Aktiv = true };
//                var portalInaktiv = new Portal { Aktiv = false };

//                _session.Store(new Stellenanzeige
//                {
//                    Id = "Stellenanzeige/1",
//                    VakanzId = "Vakanz/1",
//                    Portale = new List<Portal> { portalAktiv, portalAktiv, portalInaktiv },
//                    Start = new DateTime(2020, 5, 1),
//                    Ende = new DateTime(2020, 6, 30)
//                });
//                _session.Store(new Stellenanzeige
//                {
//                    Id = "Stellenanzeige/2",
//                    VakanzId = "Vakanz/1",
//                    Portale = new List<Portal> { portalInaktiv },
//                    Start = new DateTime(2019, 5, 1),
//                    Ende = new DateTime(2019, 6, 30)
//                });
//                _session.Store(new Stellenanzeige
//                {
//                    Id = "Stellenanzeige/3",
//                    VakanzId = "Vakanz/1",
//                    Portale = new List<Portal> { portalAktiv },
//                    Start = new DateTime(2020, 5, 1),
//                    Ende = new DateTime(2020, 6, 30)
//                });
//                // Vakanz #2
//                _session.Store(new Stellenanzeige
//                {
//                    Id = "Stellenanzeige/4",
//                    VakanzId = "Vakanz/2",
//                    Portale = new List<Portal> { portalAktiv, portalAktiv, portalInaktiv },
//                    Start = new DateTime(2020, 5, 1),
//                    Ende = new DateTime(2020, 5, 2)
//                });

//                _session.Store(new Stellenanzeige
//                {
//                    Id = "Stellenanzeige/2-1",
//                    VakanzId = "Vakanz/2",
//                    Portale = new List<Portal> { portalAktiv },
//                    Start = new DateTime(2020, 6, 1),
//                    Ende = new DateTime(2020, 6, 2)
//                });

//                // Vakanz #3
//                _session.Store(new Stellenanzeige
//                {
//                    Id = "Stellenanzeige/5",
//                    VakanzId = "Vakanz/3",
//                    Portale = new List<Portal> { portalInaktiv },
//                    Start = new DateTime(2000, 5, 1),
//                    Ende = new DateTime(2200, 6, 30)
//                });

//                _session.Advanced.WaitForIndexesAfterSaveChanges();
//                _session.SaveChanges();
//            }
//        }

//        public class Vakanz
//        {
//            public string Id { get; set; }
//            public string SomeProperty { get; set; }
//            public DateTime? Start { get; set; }
//            public DateTime? Ende { get; set; }
//        }

//        public class Stellenanzeige
//        {
//            public string Id { get; set; }
//            public string VakanzId { get; set; }
//            public DateTime? Start { get; set; }
//            public DateTime? Ende { get; set; }
//            public List<Portal> Portale { get; set; }
//        }

//        public class Portal
//        {
//            public bool Aktiv { get; set; }

//        }
//    }


//    public class TestIndex : AbstractMultiMapIndexCreationTask<TestIndex.Result>
//    {
//        public class Result
//        {
//            public string VakanzId { get; set; }
//            public string SomePropertyFromVakanz { get; set; }
//            public IEnumerable<Detail> Details { get; set; }
//            public class Detail
//            {
//                public bool PortalAktiv { get; set; }
//                public DateTime? Start { get; set; }
//                public DateTime? Ende { get; set; }
//            }
//            //public DateTime? Details_Start;
//            //public DateTime? Details_Ende;
//        }



//        public TestIndex()
//        {
//            AddMap<Stellenanzeige>(stellenanzeigen => from st in stellenanzeigen
//                                                      let v = LoadDocument<Vakanz>(st.VakanzId)
//                                                      select new
//                                                      {
//                                                          VakanzId = st.VakanzId,
//                                                          SomePropertyFromVakanz = v.SomeProperty,


//                                                          Details = new Result.Detail[] { new Result.Detail() { PortalAktiv = st.Portale.Any(p => p.Aktiv), Start = st.Start, Ende = st.Ende } },
//                                                          Details_PortalAktiv = st.Portale.Any(p => p.Aktiv),
//                                                          Details_Start = st.Start,
//                                                          Details_Ende = st.Ende,
//                                                      }
//                                                      );

//            AddMap<Vakanz>(vakanz => from v in vakanz
//                                     select new
//                                     {
//                                         VakanzId = v.Id,
//                                         SomePropertyFromVakanz = v.SomeProperty,

//                                         Details = new Result.Detail[0],
//                                         Details_PortalAktiv = false,
//                                         Details_Start = default(DateTime?),
//                                         Details_Ende = default(DateTime?),
//                                     }
//                                     );


//            Reduce = results => from result in results
//                                group result by new { result.VakanzId } into g
//                                let first = g.First()
//                                select new
//                                {
//                                    VakanzId = first.VakanzId,
//                                    SomePropertyFromVakanz = first.SomePropertyFromVakanz,
//                                    Details = g.SelectMany(e => e.Details),
//                                    Details_PortalAktiv = g.SelectMany(e => e.Details).Any(e => e.PortalAktiv),
//                                    Details_Start = g.SelectMany(e => e.Details).Select(e => e.Start),
//                                    Details_Ende = g.SelectMany(e => e.Details).Select(e => e.Ende),
//                                };

//        }

//    }
//}