using SQLite;


namespace BloodMoon.Repository
{
    internal class SelectedDayEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Indexed(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
