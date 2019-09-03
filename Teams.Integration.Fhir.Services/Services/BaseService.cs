

namespace Teams.Integration.Fhir.Services
{
    public abstract class BaseService
    {
        public string GetUrl(string url)
        {
            try
            {
                return url.Split('?')[0];
            }
            catch
            {
                return url;
            }
        }
    }
}
