using SQLite;


namespace BloodMoon.Repository
{
    internal class DBManager
    {
        private string _dbFileName = $"BloodMoonDB.db3";
        private SQLiteOpenFlags _creationFlags =
            SQLiteOpenFlags.ReadWrite |// open the database in read/write mode
            SQLiteOpenFlags.Create |// create the database if it doesn't exist
            SQLiteOpenFlags.SharedCache;// enable multi-threaded database access
        private string _dbFullPath;
        private SQLiteAsyncConnection _connection;

        public DBManager()
        {
            _dbFullPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), _dbFileName);//в винде: C:\Users\UsernameX\AppData\Roaming
            InitConnect();

            _connection.CreateTableAsync<SelectedDayEntity>();
        }
        private void InitConnect()
        {
            if (_connection == null)
                _connection = new SQLiteAsyncConnection(_dbFullPath, _creationFlags);
        }

        public async Task<bool> AddToSelected(DateTime date)
        {
            InitConnect();
            try
            {
                var inserted = await _connection.InsertAsync(
                    new SelectedDayEntity()
                    {
                        Date = date
                    });

                return inserted == 1;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> RemoveFromSelected(DateTime date)
        {
            InitConnect();
            try
            {
                var deleted = await _connection.Table<SelectedDayEntity>()
                    .Where(d => d.Date == date)
                    .DeleteAsync();

                return deleted == 1;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> ContainsInSelected(DateTime date)
        {
            InitConnect();

            var finded = await _connection.Table<SelectedDayEntity>()
                    .Where(d => d.Date == date)
                    .FirstOrDefaultAsync();

            return finded != null;
        }

        public async Task<List<DateTime>> GetAllSelectedAsync()
        {
            List<DateTime> result = new();
            InitConnect();
            try
            {
                result = await _connection.QueryScalarsAsync<DateTime>(
                    "SELECT Date FROM SelectedDayEntity");
            }
            catch (Exception ex) { }

            return result;
        }
    }
}
