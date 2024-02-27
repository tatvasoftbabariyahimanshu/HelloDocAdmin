using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{
    public class ActionRepository:IActionRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public ActionRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        public bool DeleteDoc(int RequestWiseFileID)
        {
            var data=_context.Requestwisefiles.FirstOrDefault(e=>e.Requestwisefileid==RequestWiseFileID);
            if(data!=null)
            {
                _context.Requestwisefiles.Remove(data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
          
        }
        #region Transfer_Provider
        public async Task<bool> TransferProvider(int RequestId, int ProviderId, string notes)
        {
          
                var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
                request.Physicianid = ProviderId;
                request.Status = 2;
                _context.Requests.Update(request);
                _context.SaveChanges();

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = RequestId;
                rsl.Physicianid = ProviderId;
                rsl.Notes = notes;

                rsl.Createddate = DateTime.Now;
                rsl.Transtophysicianid = ProviderId;
                rsl.Status = 2;
                _context.Requeststatuslogs.Update(rsl);
                _context.SaveChanges();

                return true;

          



        }
        #endregion
    }
}
