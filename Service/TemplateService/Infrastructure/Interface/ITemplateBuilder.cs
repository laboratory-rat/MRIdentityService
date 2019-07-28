using System.Threading.Tasks;

namespace TemplateService.Infrastructure.Interface
{
    public interface ITemplateBuilder
    {
        Task<string> Render<TModel>(string view, TModel model);
        Task<string> Render(string view, object model);
    }
}
