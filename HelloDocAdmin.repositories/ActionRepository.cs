using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HelloDocAdmin.Repositories.PatientInterface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HelloDocAdmin.Repositories
{
    public class ActionRepository : IActionRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public ActionRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }

        public bool SendAllMailDoc(string path, int RequestID)
        {

            List<int> pathList = path.Split(',').Select(int.Parse).ToList();


            string senderEmail = "practicetatvasoft@outlook.com";
            string senderPassword = "Tatvasoft@123";

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };


            string? email = _context.Requestclients.FirstOrDefault(x => x.Requestid == RequestID).Email;
            var requestWiseFile = from requestFiles in _context.Requestwisefiles
                                  where pathList.Contains(requestFiles.Requestwisefileid)
                                  select new Requestwisefile
                                  {
                                      Requestwisefileid = requestFiles.Requestwisefileid,
                                      Filename = requestFiles.Filename,
                                      Requestid = requestFiles.Requestid,
                                  };
            string message = $@"<html>
                                <body>  
                                <h1>All Documents</h1>
                                </body>
                                </html>";
            if (email != null)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "hallodoc@gmail.com"),
                    Subject = "Documents",
                    Body = message,
                    IsBodyHtml = true
                };
                foreach (var item in requestWiseFile)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/req_" + item.Requestid + "/" + item.Filename);
                    Attachment attachment = new Attachment(filePath);
                    mailMessage.Attachments.Add(attachment);
                }
                mailMessage.To.Add(email)
;
                client.Send(mailMessage);
                foreach (var attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }
                return true;
            }
            return false;
        }
        public string GetFileName(int RequestWiseFileID)
        {
            var data = _context.Requestwisefiles.FirstOrDefault(e => e.Requestwisefileid == RequestWiseFileID);
            if (data != null)
            {


                return data.Filename;
            }
            else
            {
                return "";
            }

        }
        public bool DeleteDoc(int RequestWiseFileID)
        {
            var data = _context.Requestwisefiles.FirstOrDefault(e => e.Requestwisefileid == RequestWiseFileID);
            if (data != null)
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                data.Isdeleted = bt;
                _context.Requestwisefiles.Update(data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool ClearCase(int RequestID,string id)
        {
            try
            {
                var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 10;
                    requestData.Modifieddate = DateTime.Now;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestID,


                        Status = 10,
                        Adminid=admindata.Adminid,
                        Createddate = DateTime.Now,
                        
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public Healthprofessional SelectProfessionlByID(int VendorID)
        {
            return _context.Healthprofessionals.FirstOrDefault(e => e.Vendorid == VendorID);
        }
        public bool SendOrder(ViewSendOrderModel data,string id)
        {
           

            try
            {
                Orderdetail od = new Orderdetail
                {
                    Requestid = data.RequestID,
                    Vendorid = data.VendorID,
                    Faxnumber = data.FaxNumber,
                    Email = data.Email,
                    Businesscontact = data.BusinessContact,
                    Prescription = data.Prescription,
                    Noofrefill = data.NoOFRefill,
                    Createddate = DateTime.Now,
                    Createdby = id


                };
                _context.Orderdetails.Add(od);
                _context.SaveChanges(true);
                var req = _context.Requests.FirstOrDefault(e => e.Requestid == data.RequestID);
                _email.SendMail(data.Email, "New Order arrived", data.Prescription + "Request name" + req.Firstname +"By Admin:"+id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
        #region Transfer_Provider
        public async Task<bool> TransferProvider(int RequestId, int ProviderId, string notes, string id)
        {
            var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
            request.Physicianid = ProviderId;
            request.Status = 2;
            request.Modifieddate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = RequestId;
            rsl.Physicianid = ProviderId;
       
            rsl.Notes = notes;
            rsl.Createddate = DateTime.Now;
            rsl.Adminid = admindata.Adminid;
            rsl.Createddate = DateTime.Now;
            rsl.Transtophysicianid = ProviderId;
            rsl.Status = 2;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            return true;





        }
        #endregion

        public ViewCloseCaseModel CloseCaseData(int RequestID)
        {
            ViewCloseCaseModel alldata = new ViewCloseCaseModel();

            var result = from requestWiseFile in _context.Requestwisefiles
                         join request in _context.Requests on requestWiseFile.Requestid equals request.Requestid
                         join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.Adminid equals admin.Adminid into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.Requestid == RequestID
                         select new
                         {

                             Uploader = requestWiseFile.Physicianid != null ? phys.Firstname :
                             (requestWiseFile.Adminid != null ? adm.Firstname : request.Firstname),
                             requestWiseFile.Filename,
                             requestWiseFile.Createddate,
                             requestWiseFile.Requestwisefileid

                         };
            List<Documents> doc = new List<Documents>();
            foreach (var item in result)
            {
                doc.Add(new Documents
                {
                    Createddate = item.Createddate,
                    Filename = item.Filename,
                    Uploader = item.Uploader,
                    RequestWiseFileID = item.Requestwisefileid

                });

            }
            alldata.documentslist = doc;
            Request req = _context.Requests.FirstOrDefault(r => r.Requestid == RequestID);

            alldata.Firstname = req.Firstname;
            alldata.RequestID = req.Requestid;
            alldata.ConfirmationNumber = req.Confirmationnumber;
            alldata.Lastname = req.Lastname;

            var reqcl = _context.Requestclients.FirstOrDefault(e => e.Requestid == RequestID);

            alldata.RC_Email = reqcl.Email;
            alldata.RC_Dob = new DateTime((int)reqcl.Intyear, DateTime.ParseExact(reqcl.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)reqcl.Intdate);
            alldata.RC_FirstName = reqcl.Firstname;
            alldata.RC_LastName = reqcl.Lastname;
            alldata.RC_PhoneNumber = reqcl.Phonenumber;
            return alldata;
        }
        public bool EditForCloseCase(ViewCloseCaseModel model)
        {
            try

            {

                Requestclient client = _context.Requestclients.FirstOrDefault(E => E.Requestid == model.RequestID);

                if (client != null)
                {
                    client.Phonenumber = model.RC_PhoneNumber;
                    client.Email = model.RC_Email;
                    _context.Requestclients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool CloseCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 9;
                    requestData.Modifieddate = DateTime.Now;

                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestID,


                        Status = 9,
                        Createddate = DateTime.Now

                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }




        #region Encounter
        public EncounterViewModel GetEncounterDetailsByRequestID(int RequestID)
        {
            var datareq=_context.Requestclients.FirstOrDefault(e=>e.Requestid == RequestID);
            var Data=_context.Encounterforms.FirstOrDefault(e=>e.Requestid == RequestID);

            if (Data != null)
            {
                EncounterViewModel enc = new EncounterViewModel
                {
                    ABD = Data.Abd,
                    EncounterID=Data.Encounterformid,
                    Allergies = Data.Allergies,
                    BloodPressureD = Data.Bloodpressurediastolic,
                    BloodPressureS = Data.Bloodpressurediastolic,
                    Chest = Data.Chest,
                    CV = Data.Cv,
                    DOB = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate),
                    Date = DateTime.Now,
                    Diagnosis = Data.Diagnosis,
                    Hr = Data.Hr,
                    HistoryOfMedical = Data.Medicalhistory,
                    Heent = Data.Heent,
                    Extr = Data.Extremities,
                    PhoneNumber = datareq.Phonenumber,
                    Email = datareq.Email,
                    HistoryOfP = Data.Historyofpresentillnessorinjury,
                    FirstName = datareq.Firstname,
                    LastName = datareq.Lastname,
                    Followup = Data.Followup,
                    Location = datareq.Location,
                    Medications = Data.Medications,
                    MedicationsDispensed = Data.Medicaldispensed,
                    Neuro = Data.Neuro,
                    O2 = Data.O2,
                    Other = Data.Other,
                    Pain = Data.Pain,
                    Procedures = Data.Procedures,
                   Isfinalize=Data.Isfinalize,
                    Requesid = RequestID,
                    Rr = Data.Rr,
                    Skin = Data.Skin,
                    Temp = Data.Temp,
                    Treatment = Data.TreatmentPlan





                };
                return enc;
            }
            else
            {
                if(datareq!=null)
                {
                    EncounterViewModel enc = new EncounterViewModel
                    {
                        FirstName = datareq.Firstname,
                        PhoneNumber = datareq.Phonenumber,
                        LastName = datareq.Lastname,
                        Location = datareq.Location,
                        DOB = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate),
                        Date = DateTime.Now,
                        Requesid = RequestID,

                        Email = datareq.Email,
                    };
                    return enc;
                }
                else
                {
                    return new EncounterViewModel();
                }
               
              
            }
           
           
           
        }



        public bool EditEncounterDetails(EncounterViewModel Data,string id)
        {
            //try
            //{
                var admindata=_context.Admins.FirstOrDefault(e=>e.Aspnetuserid == id);
                if(Data.EncounterID==0)
                {
                    Encounterform enc = new Encounterform
                    {
                        Abd = Data.ABD,
                        Encounterformid = Data.EncounterID,
                        Allergies = Data.Allergies,
                        Bloodpressurediastolic = Data.BloodPressureD,
                        Bloodpressuresystolic = Data.BloodPressureS,
                        Chest = Data.Chest,
                        Cv = Data.CV,
                        Diagnosis = Data.Diagnosis,
                        Hr = Data.Hr,
                        Medicalhistory = Data.HistoryOfMedical,
                        Heent = Data.Heent,
                        Extremities = Data.Extr,
                        Historyofpresentillnessorinjury = Data.HistoryOfP,
                        Followup = Data.Followup,                     
                        Medications = Data.Medications,
                        Medicaldispensed = Data.MedicationsDispensed,
                        Neuro = Data.Neuro,
                        O2 = Data.O2,
                        Other = Data.Other,
                        Pain = Data.Pain,
                        Procedures = Data.Procedures,                      
                        Requestid = Data.Requesid,
                        Rr = Data.Rr,
                        Skin = Data.Skin,
                        Temp = Data.Temp,
                        TreatmentPlan = Data.Treatment,
                        Adminid=admindata.Adminid,
                        Created=DateTime.Now,
                        Modified=DateTime.Now,  
                      

                    };
                    _context.Encounterforms.Add(enc);
                    _context.SaveChanges();
                   
                    return true;

                }
                else
                {
                    var encdetails = _context.Encounterforms.FirstOrDefault(e => e.Encounterformid == Data.EncounterID);
                    if (encdetails != null)
                    {
                        encdetails.Abd = Data.ABD;
                        encdetails.Encounterformid = Data.EncounterID;
                        encdetails.Allergies = Data.Allergies;
                        encdetails.Bloodpressurediastolic = Data.BloodPressureD;
                        encdetails.Bloodpressuresystolic = Data.BloodPressureS;
                        encdetails.Chest = Data.Chest;
                        encdetails.Cv = Data.CV;
                        encdetails.Diagnosis = Data.Diagnosis;
                        encdetails.Hr = Data.Hr;
                        encdetails.Medicalhistory = Data.HistoryOfMedical;
                        encdetails.Heent = Data.Heent;
                        encdetails.Extremities = Data.Extr;
                        encdetails.Historyofpresentillnessorinjury = Data.HistoryOfP;
                        encdetails.Followup = Data.Followup;
                        encdetails.Medications = Data.Medications;
                        encdetails.Medicaldispensed = Data.MedicationsDispensed;
                        encdetails.Neuro = Data.Neuro;
                        encdetails.O2 = Data.O2;
                        encdetails.Other = Data.Other;
                        encdetails.Pain = Data.Pain;
                        encdetails.Procedures = Data.Procedures;
                        encdetails.Requestid = Data.Requesid;
                        encdetails.Rr = Data.Rr;
                        encdetails.Skin = Data.Skin;
                        encdetails.Temp = Data.Temp;
                        encdetails.TreatmentPlan = Data.Treatment;
                        encdetails.Adminid = admindata.Adminid;
                        encdetails.Modified = DateTime.Now;
                        _context.Encounterforms.Update(encdetails);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


                return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
          
        }


        public bool CaseFinalized(EncounterViewModel model, string id)
        {
            try
            {
                var data = _context.Encounterforms.FirstOrDefault(e => e.Encounterformid == model.EncounterID);
                data.Modified = DateTime.Now;
                data.Isfinalize = true;
                _context.Encounterforms.Update(data);
                _context.SaveChanges();


                var final = _context.Requests.FirstOrDefault(e => e.Requestid == model.Requesid);
                final.Modifieddate = DateTime.Now;
                final.Status = 6;
                _context.Requests.Update(final);
                _context.SaveChanges();

                var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);
                Requeststatuslog rs = new Requeststatuslog
                {
                    Requestid = final.Requestid,
                    Status = 6,
                    Createddate = DateTime.Now,
                    Adminid= admindata.Adminid


                };
                _context.Requeststatuslogs.Add(rs);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
          
           
        }

        #endregion
    }
}
