using Formservicecrud.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.IO;
namespace Formservicecrud.Models
{
    public class services
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
        Imagesrc imgs = new Imagesrc();

        public List<Dropdown> GetDataList()
        {
            List<Dropdown> lst = new List<Dropdown>();
            SqlCommand cmd = new SqlCommand("sp_select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            foreach (DataRow dr in dt.Rows) //Dml command like insert , update, delete is use ExecuteNonQuery & select command use ExecuteReader(DDL).
            {
                lst.Add(new Dropdown
                {
                    id = Convert.ToInt32(dr[0]),
                    dropdown = Convert.ToString(dr[1]),
                    Gender = Convert.ToString(dr[2]),
                    Date=  Convert.ToString(dr[3]),
                    Image = Convert.ToString(dr[4])

            });
            }


            return lst;

        }    

        
        

        public bool DeleteData(int id)
        {
            int i;
            SqlCommand cmd = new SqlCommand("DELETE  Formcrud WHERE id=@id", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}