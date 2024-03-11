﻿using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IActionRepository
    {
        public string GetFileName(int RequestWiseFileID);
        public bool DeleteDoc(int RequestWiseFileID);
        public  Task<bool> TransferProvider(int RequestId, int ProviderId, string notes);
        public Healthprofessional SelectProfessionlByID(int VendorID);
        public bool SendOrder(ViewSendOrderModel data);
        public bool CloseCase(int RequestID);
        public bool ClearCase(int RequestID);
        public ViewCloseCaseModel CloseCaseData(int RequestID);
        public bool EditForCloseCase(ViewCloseCaseModel model);
        public bool SendAllMailDoc(string path, int RequestID);


        public EncounterViewModel GetEncounterDetailsByRequestID(int RequestID);
        public bool EditEncounterDetails(EncounterViewModel Data, string id);
        public bool CaseFinalized(EncounterViewModel model, string id);
    }
}
