namespace Infrastructure.Entity.AppLanguage
{
    public class Language : AppEntity, IAppEntity
    {
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Code { get; set; }
    }
}
