using kurcova_test_1_bot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kurcova_test_1_bot.Clients
{
    public class HistoricalEventsClient
    {
        public static async Task<List<HistoricalEvents>> GetHistoricalEventsAsync(string text/*, int year*/)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost:7124/HistoricalEvents?text={text}"),
                Headers =
                {
                    { "X-RapidAPI-Key", "34d7f3c379mshf511c2d5acb645fp1ddca2jsn939b03ca067e" },
                    { "X-RapidAPI-Host", "historical-events-by-api-ninjas.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                List<HistoricalEvents> historicalEvents = JsonConvert.DeserializeObject<List<HistoricalEvents>>(json);
                return historicalEvents;
                //var body = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(body);
            }
        }
    }
}
