using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticConsole
{
    [ElasticsearchType(IdProperty = "id", Name = "news")]
    public class NewsModel
    {
        public string id { get; set; }
        public DateTime created { get; set; }
        public int publisherId { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("press return");

            Console.ReadLine();

            //Submit 1 to test 

            //InsertOne(new NewsModel
            //{
            //    publisherId = -1,
            //    created = DateTime.UtcNow,
            //    id = "-1 2",
            //});

           // BulkInsert(200, 1000, "1").Wait();

           // BulkInsert(200, 1000, "2").Wait();


            // bulk insert
            //BulkInsert(1000, 300).Wait();

            //search
            Search(100, 100).Wait();

            Console.WriteLine("press return");

            Console.ReadLine();
        }

        static ElasticClient GetClient()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            
            settings.MapDefaultTypeIndices(d => d
            .Add(typeof(NewsModel), "news")
            );

            return new ElasticClient(settings);
        }

        static async Task Search(int friendCount, int size)
        {
            var client = GetClient();

            List<int> friends = new List<int>();
            for (int i = 0; i < friendCount; i++)
            {
                friends.Add(i);
            }

            var search = new SearchDescriptor<NewsModel>();
            
           // search.Routing("1", "2");
            search.Sort(so => so.Descending(a => a.created));
            search.Size(size);
            search.Query(q => q.Terms(t => t.Field(f => f.publisherId).Terms<int>(friends)));

            var result = client.Search<NewsModel>(search);

            Console.WriteLine("");
            Console.WriteLine("count " + result.Hits.Count + " took " + result.Took + "ms");
            Console.WriteLine("");

            foreach (var item in result.Documents)
            {
                var json = JsonConvert.SerializeObject(item, Formatting.Indented);
                Console.WriteLine(json);

            }

            Console.WriteLine("");
        }

        static async Task BulkInsert(int publisherCount, int recordCount, string route, int publisherStart = 0)
        {
            Console.WriteLine("BulkInsert : " + publisherCount + " x " + recordCount);

            var client = GetClient();

            var descriptor = new BulkDescriptor();

            var rand = new Random();

            for (int p = 0; p < publisherCount; p++)
            {
                for (int r = 0; r < recordCount; r++)
                {
                    var ops = new BulkCreateDescriptor<NewsModel>();
                    ops.Routing(route);
                    ops.Document(new NewsModel
                    {
                        id = Guid.NewGuid().ToString(),
                        created = DateTime.UtcNow.Subtract(TimeSpan.FromHours(rand.Next(1, publisherCount * recordCount))),
                        publisherId = p + publisherStart
                    });

                    descriptor.AddOperation(ops);
                }
            }

            var response = await client.BulkAsync(descriptor);

            var json = JsonConvert.SerializeObject(response, Formatting.Indented);

            Console.WriteLine("");
            Console.WriteLine(json);
        }

        static void InsertOne(NewsModel model)
        {
            Console.WriteLine("InsertOne : " + model.id + " x " + model.publisherId);

            var client = GetClient();

            var result = client.Create<NewsModel>(model);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            Console.WriteLine("");
            Console.WriteLine(json);
            Console.WriteLine("");
        }
    }
}