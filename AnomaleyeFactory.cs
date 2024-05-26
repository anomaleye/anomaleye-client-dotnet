using Anomaleye.DirectClient;

namespace Anomaleye
{
    public class AnomaleyeFactory
    {
        private string _apiBaseUrl = "http://localhost:8080/public";
        private string _apiKey;

        public AnomaleyeFactory(string apiKey, string apiBaseUrl = null)
        {
            this._apiKey = apiKey;
            
            if (apiBaseUrl != null)
            {
                this._apiBaseUrl = apiBaseUrl;
            }
        }

        public IAnomaleye CreateInstanceForRecordingSession(string systemId, string systemVersionId, string recordingSessionId, IClock clock = null)
        {
            return new AnomaleyeImpl(
                new AnomaleyeRestApiClient(this._apiBaseUrl, this._apiKey),
                systemId,
                systemVersionId,
                recordingSessionId,
                clock);
        }

        public static IAnomaleye CreateNoOpInstance()
        {
            return new NoOpImpl();
        }
        
    }
}