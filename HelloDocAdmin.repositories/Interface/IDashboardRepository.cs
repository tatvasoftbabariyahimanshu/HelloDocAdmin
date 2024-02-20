using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{

    public interface IDashboardRepository
    {
        List<DashboardRequestModel> GetRequests(short status);
        int GetRequestNumberByStatus(short status);
    }
}
