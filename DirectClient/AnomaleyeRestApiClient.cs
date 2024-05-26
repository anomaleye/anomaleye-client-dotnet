using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Anomaleye.DirectClient.Models;
using Newtonsoft.Json;

namespace Anomaleye.DirectClient
{
    internal class AnomaleyeRestApiClient : IAnomaleyeClient
    {
        private HttpClient _httpClient;

        public AnomaleyeRestApiClient(string apiServerBaseUrl, string apiKey)
        {
            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri(apiServerBaseUrl);
        }

        public async Task RecordEventsAsync(string systemId, string systemVersionId, string recordingSessionId, List<EventRecord> events)
        {
            object requestBody = new
            {
                events = events,
            };
            string bodyJson = JsonConvert.SerializeObject(requestBody);
            var requestContent = new StringContent(
                bodyJson,
                System.Text.Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await this._httpClient.PostAsync(
                $"public/systems/{systemId}/version/{systemVersionId}/recording-session/{recordingSessionId}/events", requestContent);
            
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            response.EnsureSuccessStatusCode();
        }
    }
}