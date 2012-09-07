using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkPad.Core
{
    public interface ISource
    {
        Task<bool> Login();
        Task<IEnumerable<Document>> Open();
        Task<IEnumerable<Document>> Restore();
        Task Save(Document document);
    }
}
