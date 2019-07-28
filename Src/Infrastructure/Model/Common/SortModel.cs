namespace Infrastructure.Model.Common
{
    public class SortModel
    {
        public string Field { get; set; }
        public bool Desc { get; set; }

        public SortModel() { }
        public SortModel(string field, bool desc)
        {
            Field = field;
            Desc = desc;
        }

        public static SortModel Default()
            => new SortModel("CreateTime", true);
        public static SortModel Check(SortModel sort)
            => sort == null || string.IsNullOrWhiteSpace(sort.Field)
                ? Default()
                : sort;
    }
}
