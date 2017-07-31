using NostreetsSandbox.Providers.Interfaces;

namespace NostreetsSandbox.Providers.Utilities
{
    public sealed class DataProvider
    {
        private DataProvider() { }

        public static IDao SqlInstance
        {
            get
            {
                return SqlDao.Instance;
            }
        }

    }
}
