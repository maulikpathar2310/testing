using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;
using Formservicecrud.Models;
using System.IO;

namespace Formservicecrud.Controllers
{
    public class HomeController : Controller
    {
        services src = new services();

        public ActionResult SelectCategory()
        {
            ModelState.Clear();
            return View(src.GetDataList());

        }

        // Get
        public ActionResult Insertcategory()
        {
            return View();

        }

        // Post
        [HttpPost]
        public ActionResult Insertcategory(Dropdown dd, HttpPostedFileBase file)
        {
            string conn = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            SqlConnection con = new SqlConnection(conn);

            int i;
            SqlCommand cmd = new SqlCommand("sp_insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@id", dd.id);
            cmd.Parameters.AddWithValue("@dropdown", dd.dropdown);
            cmd.Parameters.AddWithValue("@Gender", dd.Gender);
            cmd.Parameters.AddWithValue("@Date", dd.Date);
            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string imgpath = Path.Combine(Server.MapPath("../Content/"), filename);
                file.SaveAs(imgpath);
            }
            cmd.Parameters.AddWithValue("@Image", "../Content/" + file.FileName);

            con.Open();
            i = cmd.ExecuteNonQuery();
            con.Close();

            ViewData["Msg"] = "User record is save successfully.";

            return RedirectToAction("SelectCategory", "Home");
        }   

        // Get
        public ActionResult Updatecategory(int id)
        {
            return View(src.GetDataList().Find(dd=>dd.id==id));

        }

        // Post
        [HttpPost]
        public ActionResult Updatecategory( Dropdown dd, HttpPostedFileBase file)
        {
            string conn = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            SqlConnection con = new SqlConnection(conn);

            int i;
            SqlCommand cmd = new SqlCommand("sp_update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", dd.id);
            cmd.Parameters.AddWithValue("@dropdown", dd.dropdown);
            cmd.Parameters.AddWithValue("@Gender", dd.Gender);
            cmd.Parameters.AddWithValue("@Date", dd.Date);
            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string imgpath = Path.Combine(Server.MapPath("/Content/"),filename);
                file.SaveAs(imgpath);
            }
            cmd.Parameters.AddWithValue("@Image","/Content/" + file.FileName);

            con.Open();
            i = cmd.ExecuteNonQuery();
            con.Close();

            ViewData["Msg"] = "User record is save successfully.";

            return RedirectToAction("SelectCategory", "Home");


        }

        public ActionResult Deletedata(int id)
        {
            try
            {
                if (src.DeleteData(id))
                {
                    ViewBag.Message = "Data deleted";
                }
                return RedirectToAction("SelectCategory");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}