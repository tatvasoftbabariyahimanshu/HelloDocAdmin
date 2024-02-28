using HelloDocAdmin.Entity.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewNotesModel
    {
       
        public int? Requestnotesid { get; set; }

  
        public int? Requestid { get; set; }

   
        public string? Strmonth { get; set; }

       
        public int? Intyear { get; set; }

      
        public int? Intdate { get; set; }

       
        public string? Physiciannotes { get; set; }

       
        public string? Adminnotes { get; set; }

     
        public string? Createdby { get; set; } = null!;

       
        public DateTime? Createddate { get; set; }

      
        public string? Modifiedby { get; set; }

      
        public DateTime? Modifieddate { get; set; }

        public short Status { get; set; }
        
        public string? Ip { get; set; }

         
        public string? Administrativenotes { get; set; }

        
        public virtual Request Request { get; set; } = null!;


        public List<TransfernotesModel> transfernotes { get; set; } = null!;
             public List<TransfernotesModel> cancelnotes { get; set; } = null!;
    }
}
