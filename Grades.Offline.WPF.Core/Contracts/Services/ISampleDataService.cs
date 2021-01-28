using System.Collections.Generic;
using System.Threading.Tasks;

using Grades.Offline.WPF.Core.Models;

namespace Grades.Offline.WPF.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
