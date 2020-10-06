using Raven.Client.Documents;
using Xunit;
using System.Linq;
using Raven.Client.Documents.Indexes;
using System;
using System.Security.Cryptography.X509Certificates;
using Raven.Embedded;

namespace RavenDBTestDriverFullExample
{
    //public class RavenDBTestDriver 
    //{
    //    [Fact]
    //    public void MyFirstTest()
    //    {
    //        EmbeddedServer.Instance.StartServer();
    //        using (var store = EmbeddedServer.Instance.GetDocumentStore("Embedded"))
    //        {
    //            IndexCreation.CreateIndexes(typeof(BAALastSucessfullJobPosting).Assembly, store);
    //        }
    //    }
    //}

    //public class BAALastSucessfullJobPosting : AbstractIndexCreationTask<BAAJobPosting, BAALastSucessfullJobPosting.Result>
    //{
    //    public class Result
    //    {
    //        public string StellenanzeigenId { get; set; }
    //        public Aktion Status { get; set; }
    //        public DateTime TimeStamp { get; set; }
    //        public JobPositionPosting UploadedJobPositionPosting { get; set; }

    //    }
    //    public BAALastSucessfullJobPosting()
    //    {
    //        Map = jobPostings => from jp in jobPostings
    //                             where jp.Verarbeitungsstatus == Verarbeitungsstatus.Done || jp.Verarbeitungsstatus == Verarbeitungsstatus.Proceesing
    //                             select new Result
    //                             {
    //                                 Status = jp.Aktion,
    //                                 StellenanzeigenId = jp.StellenanzeigenId,
    //                                 TimeStamp = jp.TimeStamp,
    //                                 UploadedJobPositionPosting = jp.HochgeladenesJobPositionPosting
    //                             };

    //        Reduce = results => from result in results
    //                            group result by result.StellenanzeigenId into g
    //                            select new
    //                            {
    //                                Status = g.OrderBy(e => e.TimeStamp).Last().Status,
    //                                StellenanzeigenId = g.Key,
    //                                TimeStamp = g.OrderBy(e => e.TimeStamp).Last().TimeStamp,
    //                                UploadedJobPositionPosting = g.OrderBy(e => e.TimeStamp).Last().UploadedJobPositionPosting,
    //                            };

    //        Index(e => e.StellenanzeigenId, FieldIndexing.Default);
    //    }
    //}


    //public class BAAJobPosting
    //{
    //    public string Id { get; set; } = $"{nameof(BAAJobPosting)}/{Guid.NewGuid()}";
    //    public string StellenanzeigenId { get; set; }
    //    public string ReferenzId { get; set; }

    //    public Aktion Aktion { get; set; }
    //    public Verarbeitungsstatus Verarbeitungsstatus { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //    public DateTime? LockUntil { get; set; }

    //    public string UploadDateiname { get; set; }
    //    public string ErrorMessage { get; set; }

    //    public JobPositionPosting HochgeladenesJobPositionPosting { get; set; }
    //}

    //public enum Aktion
    //{
    //    Neuanlage,
    //    Änderung,
    //    Löschen,
    //}

    //public enum Verarbeitungsstatus
    //{
    //    Waiting,       
    //    Preparation,       
    //    Proceesing,        
    //    Done,             
    //    Error,            
    //    Skipped,           
    //}

    //public class JobPositionPosting
    //{
    //    public string JobPositionPostingId { get; set; }
    //    //public HiringOrg HiringOrg { get; set; } = new HiringOrg();
    //    //public PostDetail PostDetail { get; set; } = new PostDetail();
    //    //public JobPositionInformation JobPositionInformation { get; set; } = new JobPositionInformation();
    //}
}