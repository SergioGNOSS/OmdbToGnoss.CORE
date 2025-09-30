using Gnoss.ApiWrapper;

namespace OmdbToGnoss.CORE
{
    // Configuration.cs
    public class ApiManager
    {
        private static ApiManager _instance;
        private readonly ResourceApi _resourceApi;
        private readonly CommunityApi _communityApi;
        private readonly string _configPath;

        private ApiManager()
        {
            _configPath = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"Config\ConfigOAuth\OAuth.config"));

            _resourceApi = new ResourceApi(_configPath);
            _communityApi = new CommunityApi(_configPath);
        }

        public static ApiManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApiManager();
                }
                return _instance;
            }
        }

        public ResourceApi ResourceApi => _resourceApi;
        public CommunityApi CommunityApi => _communityApi;
        public string ConfigPath => _configPath;
    }
}
