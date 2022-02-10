using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PetaPoco;
using Login.Models;
using System.Data;

namespace Login.Infrastructure.DataProvider
{
    public class BaseDataProvider
    {
        private readonly Database _db;
        public BaseDataProvider()
        {
            _db = new Database("mycon");
            _db.CommandTimeout = 10000;
        }

        public List<T> GetEntityList<T>(string spname, List<SearchValueData> searchParam = null) where T : class, new()
        {
            return _db.Fetch<T>(";" + GetSPString(spname, searchParam));
        }
        public List<T> GetEntityList<T>(List<SearchValueData> searchParam = null, string customWhere = "", string sortIndex = "", string sortDirection = "") where T : class, new()
        {
            return _db.Query<T>(GetFilterString<T>(searchParam, sortIndex, sortDirection, customWhere)).ToList<T>();
        }

        public List<T> GetEntityList<T>(string sqlQuery) where T : class, new()
        {
            return _db.Query<T>(sqlQuery).ToList<T>();
        }
        private string GetSPString(string spname, List<SearchValueData> searchParam)
        {
            string sp = string.Format("EXEC {0}", spname);

            if (searchParam != null)
            {
                sp = searchParam.Aggregate(sp, (current, searchValueData) => current + string.Format(" @@{0} = '{1}',", searchValueData.Name, searchValueData.Value == null ? searchValueData.Value : searchValueData.Value.Replace("@", "@@").Replace("'", "''")));
                if (searchParam.Any())
                    sp = sp.TrimEnd(',');
            }
            return sp;
        }
        private string GetFilterString<T>(List<SearchValueData> searchParam, string sortIndex = "", string sortDirection = "", string customWhere = "") where T : class, new()
        {
            T item = new T();
            string tableName = GetTableName<T>();
            string select = "SELECT * FROM " + tableName;
            string where = "";

            if (searchParam != null && searchParam.Count > 0)
            {
                where += " WHERE 1=1";
                foreach (SearchValueData val in searchParam)
                {
                    string value = val.Value.Replace("'", "''");
                    PropertyInfo propertyInfo = item.GetType().GetProperty(val.Name);
                    var propertyType = propertyInfo.PropertyType;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }

                    if (propertyType == typeof(string))
                    {
                        if (val.IsEqual)
                            where += " AND " + val.Name + " = '" + value + "'";
                        else if (val.IsNotEqual)
                            where += " AND " + val.Name + " != '" + value + "'";
                        else
                            where += " AND " + val.Name + " LIKE '%" + value + "%'";
                    }
                    else if (propertyType == typeof(bool))
                    {
                        where += " AND " + val.Name + "=" + value;
                    }
                    else if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(decimal) || propertyType == typeof(float))
                    {
                        if (val.IsNotEqual)
                            where += " AND " + val.Name + "!=" + value;
                        else
                            where += " AND " + val.Name + "=" + value;
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        if (searchParam.Count(a => a.Name == val.Name) > 1)
                        {
                            where += " AND ((" + val.Name + " BETWEEN " + searchParam.First(a => a.Name == val.Name).Value + " AND " + searchParam.Last(a => a.Name == val.Name).Value + ") OR (" + val.Name + " BETWEEN " + searchParam.Last(a => a.Name == val.Name).Value + " AND " + searchParam.First(a => a.Name == val.Name).Value + "))";
                        }
                        else
                        {
                            where += " AND " + val.Name + "='" + value + "'";
                        }
                    }
                }
            }

            where += customWhere != "" ? string.IsNullOrEmpty(where) ? " WHERE (" + customWhere + ")" : " AND (" + customWhere + ")" : "";

            if (sortIndex != "")
                where += " ORDER BY " + sortIndex + " " + sortDirection;

            return select + where.Replace("1=1 AND", "").Replace("1=1", "").Replace("@", "@@");
        }
        protected string GetTableName<T>()
        {
            return (typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0] as TableNameAttribute).Value;
        }

        public T GetEntity<T>(Guid id) where T : class, new()
        {
            T item = new T();
            string tableName = GetTableName<T>();
            string primaryKeyName = GetPrimaryKeyName<T>();
            return _db.SingleOrDefault<T>("SELECT * FROM " + tableName + " WHERE " + primaryKeyName + "='" + id + "'") ?? item;
        }

        public T GetEntity<T>(string spname, List<SearchValueData> searchParam = null) where T : class, new()
        {
            return GetEntityList<T>(spname, searchParam).SingleOrDefault();
        }

        public T GetEntity<T>(List<SearchValueData> searchParam = null, string customWhere = "", string sortIndex = "", string sortDirection = "") where T : class, new()
        {
            return _db.SingleOrDefault<T>(GetFilterString<T>(searchParam, sortIndex, sortDirection, customWhere));
        }


        public T GetMultipleEntity<T>(string query, params object[] args) where T : new()
        {
            GridReader grd = _db.QueryMultiple(query, args);
            T obj = new T();
            Type returnType = typeof(T);
            PropertyInfo[] propertyInfo = returnType.GetProperties();
            foreach (var info in propertyInfo)
            {
                bool isFirstorDefault = false;
                Type t;
                if (info.PropertyType.GetGenericArguments().Any())
                {
                    t = info.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    isFirstorDefault = true;
                    t = info.PropertyType;
                }
                var value =
                (typeof(BaseDataProvider).GetMethod("GetValueOf")
                .MakeGenericMethod(t)
                .Invoke(null, new object[] { grd, isFirstorDefault }));
                info.SetValue(obj, value, null);
            }
            _db.CloseSharedConnection();
            return obj;
        }

        public T GetMultipleEntity<T>(string spname, List<SearchValueData> searchParam) where T : new()
        {
            GridReader grd = _db.QueryMultiple(GetSPString(spname, searchParam));
            T obj = new T();
            Type returnType = typeof(T);
            PropertyInfo[] propertyInfo = returnType.GetProperties();
            foreach (var info in propertyInfo)
            {

                if (!info.CanWrite)
                {
                    continue;
                }
                bool isFirstorDefault = false;
                Type t;
                if (info.PropertyType.GetGenericArguments().Any())
                {
                    t = info.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    isFirstorDefault = true;
                    t = info.PropertyType;
                }
                var value =
                (typeof(BaseDataProvider).GetMethod("GetValueOf")
                .MakeGenericMethod(t)
                .Invoke(null, new object[] { grd, isFirstorDefault }));
                info.SetValue(obj, value, null);
            }
            _db.CloseSharedConnection();
            return obj;
        }

        /// <summary>
        /// Reads from a GridReader, Either List of Type T or Object if Type T
        /// </summary>
        /// <param name="reader">GridReader from which result set will be read.</param>
        /// <param name="isFirstorDefault">If true function returns first row or default value as object of type T else return List of T"></typeparam></param>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <returns>An enumerable collection of result records</returns>
        public static object GetValueOf<T>(GridReader reader, bool isFirstorDefault)
        {
            IEnumerable<T> lst = reader.Read<T>();
            if (isFirstorDefault)
                return lst.FirstOrDefault();
            return lst.ToList();
        }

        protected string GetPrimaryKeyName<T>()
        {
            return (typeof(T).GetCustomAttributes(typeof(PrimaryKeyAttribute), true)[0] as PrimaryKeyAttribute).Value;
        }
        public T SaveEntity<T>(T item) where T : class
        {
            if (_db.IsNew(item))
            {
                _db.Insert(item);
            }
            else
            {
                _db.Update(item);
            }
            return item;
        }
        public DataSet GetDataSet(string spname, List<SearchValueData> searchParam = null)
        {
            string qry = GetSPString(spname, searchParam);
            DataSet ds = _db.GetDataSet(qry);

            return ds;
        }
        public object ExecQuery(string query)
        {
            return _db.Execute(query);
        }
    }
}