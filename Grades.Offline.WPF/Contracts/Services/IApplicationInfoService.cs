using System;

namespace Grades.Offline.WPF.Contracts.Services
{
    public interface IApplicationInfoService
    {
        Version GetVersion();
    }
}
