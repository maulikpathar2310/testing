using Innovagic.Models;
using innovegic.Models.ViewModel;
using Login.Infrastructure.DataProvider;
using Login.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace innovegic.Infrastructure.DataProvider
{
    public class FaqDataProvider :BaseDataProvider, IFaqDataProvider 
    {   
        public string SaveFaqData(FaqViewModel cfd)
        {
            string msg = "";
            try
            {
                Faq obj = new Faq();
                var searchList = new List<SearchValueData>
                {
                    new SearchValueData { Name = "FaqHeading", Value = cfd.faqobj.FaqHeading },
                    new SearchValueData { Name = "FaqContent", Value = cfd.faqobj.FaqContent },
                };
                DataSet savedata = GetDataSet("sp_Faq_Save", searchList);
                msg = "Faq Insert";
            }
        
            catch(Exception ex)
            {
                msg = "Not Inserted";
            }
           
            return (msg);
        }
        public FaqViewModel GetFaqSavedData()
        {
            FaqViewModel list = new FaqViewModel();
            DataSet ds = GetDataSet("sp_Faq_Get", null);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                FaqViewModel faqlist = new FaqViewModel();
                faqlist.faqobj.FaqId = Guid.Parse(Convert.ToString(ds.Tables[0].Rows[i]["FaqId"]));
                faqlist.faqobj.FaqHeading = Convert.ToString(ds.Tables[0].Rows[i]["FaqHeading"]);
                faqlist.faqobj.FaqContent = Convert.ToString(ds.Tables[0].Rows[i]["FaqContent"]);
                list.faqlist.Add(faqlist.faqobj);
            }
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                FaqViewModel SliderList = new FaqViewModel();
                SliderList.PosterObj.PosterId = Guid.Parse(Convert.ToString(ds.Tables[1].Rows[i]["PosterId"]));
                SliderList.PosterObj.Poster = Convert.ToString(ds.Tables[1].Rows[i]["Poster"]);
                list.PosterList.Add(SliderList.PosterObj);
            }
            return list;
        }
        /*====== Edit Or Update =====*/
        public Faq GetFaqById(Guid FaqId)
        {
            var searchList = new List<SearchValueData>
                {
                    new SearchValueData { Name = "FaqId", Value = Convert.ToString(FaqId) }
                };
            Faq getinfo = GetEntity<Faq>("sp_Get_Faq_Id", searchList);
            return getinfo;
        }
        public string EditFaq(Faq obj)
        {
            string msg = "";
            try
            {
                var searchList = new List<SearchValueData>
                {
                   new SearchValueData { Name = "FaqId", Value = Convert.ToString(obj.FaqId) },
                   new SearchValueData { Name = "FaqHeading", Value = obj.FaqHeading },
                   new SearchValueData { Name = "FaqContent", Value = obj.FaqContent },
                };
                DataSet userList = GetDataSet("sp_Faq_Update", searchList);
                msg = "Record Updated";
            }
            catch (Exception Ex)
            {
                msg = "Record Update Failed";
            }
            return msg;
        }

        /*========= Delete Method ========*/
        public Guid DeleteFaq(Guid Id)
        {
            ExecQuery("UPDATE tbl_Faq SET IsActive = 0  WHERE FaqId='" + Id + "'");
            return Id;
        }
    }
}