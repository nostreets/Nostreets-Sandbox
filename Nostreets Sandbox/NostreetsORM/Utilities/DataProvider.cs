using NostreetsORM.Interfaces;

namespace NostreetsORM.Utilities
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
