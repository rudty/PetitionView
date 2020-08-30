using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetitionsList
{
    class PetitionItem
    {
        public string id { get; set; }
        public int paging_id { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string agreement { get; set; }
        public override string ToString() => agreement + "명 \t " + title;
    }
    class PetitionApiResponse
    {
        public IList<PetitionItem> item { get; set; }
        public int page { get; set; }
        public string total { get; set; }
        public string paging { get; set; }
    }

    static class Request
    {
        public delegate void OnPetitionApiResponseReceive(PetitionItem petitionApiResponse);


        public static bool IsEmpty<T>(this ICollection<T> c) => c?.Count == 0;

        public static async void listAll(OnPetitionApiResponseReceive cb)
        {
            for (int page = 1; ; page++)
            {
        
                var l = await list(page);

                if (l.item.IsEmpty())
                {
                    break;
                }

                foreach (var item in l.item)
                {
                    await Task.Delay(500);
                    cb(item);
                }
            }
        }

        private static async Task<PetitionApiResponse> list(int page)
        {
            return await list(page.ToString());
        }
        private static async Task<PetitionApiResponse> list(string page)
        {
            Debug.WriteLine(page);
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("c", "0"),
                    new KeyValuePair<string, string>("only", "1"),
                    new KeyValuePair<string, string>("page",  page),
                    new KeyValuePair<string, string>("order", "1"),
            });
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
            client.BaseAddress = new Uri("https://www1.president.go.kr");

            var res = await client.PostAsync("/api/petitions/list", content);
            var body = await res.Content.ReadAsStringAsync();
            //Console.WriteLine(body);
            //var bodyStream = await res.Content.ReadAsStreamAsync();

            try
            {
                var p = JsonSerializer.Deserialize<PetitionApiResponse>(body);
                return p;
            }
            catch(Exception e)
            {
                Debug.WriteLine(body);
                throw e;
            }
            
        }


    }



}
