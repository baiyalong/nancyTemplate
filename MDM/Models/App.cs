using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class App : ModelBase
    {
        /// <summary>
        /// 应用标识
        /// </summary>
        public string appID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string appName { get; set; }

        public string appSize { get; set; }

        public string iconUrl { get; set; }

        public string downloadUrl { get; set; }

        public string appDetailImageUrl { get; set; }

        public int downloadNum { get; set; }

        public string version { get; set; }

        public string classification { get; set; }

        public string authorizedby { get; set; }

        public string producers { get; set; }

        /// <summary>
        /// 应用描述
        /// </summary>
        public string description { get; set; }

        public bool isMandatory { get; set; }

        public bool isRecommended { get; set; }

        public string classify { get; set; }

        public string Template { get; set; }

        public string packageName { get; set; }

        public int Status { get; set; }

        public string[] UserGroup { get; set; }
        public override void View()
        {
            //this.appDetailImageUrl = null;
        }
        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() {
            this.appName,
            this.authorizedby,
            this.classification,
            this.description,
            this.producers
            }, ss);
        }
    }


    public class AppModel
    {
        /// <summary>
        /// 应用标识
        /// </summary>
        public string appID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string appName { get; set; }

        public string appSize { get; set; }

        public string iconUrl { get; set; }

        public string downloadUrl { get; set; }

        public string packageName { get; set; }

        public string version { get; set; }

        public void App2AppModel(App app)
        {

            string StrUrl = ConfigurationManager.AppSettings["StrUrl"].ToString();
            this.appID = app.ID;
            this.appName = app.appName;
            this.appSize = app.appSize;
            this.packageName = app.packageName;
            this.version = app.version;

            if (app.iconUrl != null)
            {
                this.iconUrl = StrUrl + app.iconUrl.Replace("\"", "");
            }
            else
            {
                this.iconUrl = string.Empty;
            }
            
            if (app.downloadUrl != null)
            {
                this.downloadUrl = StrUrl + app.downloadUrl.Replace("\"", "");
            }
            else
            {
                this.downloadUrl = string.Empty;
            }
           
            
            
        }
    }

    public class AppDetailModel : AppModel
    {
        public string[] appDetailImageUrl { get; set; }

        public int downloadNum { get; set; }

        public string version { get; set; }

        public string classification { get; set; }

        public string authorizedby { get; set; }

        public string producers { get; set; }

        /// <summary>
        /// 应用描述
        /// </summary>
        public string description { get; set; }

        public void App2AppDetailModel(App app)
        {
            string StrUrl = ConfigurationManager.AppSettings["StrUrl"].ToString();
            this.appID = app.ID;  

            this.appName = app.appName;
            this.appSize = app.appSize;
            this.packageName = app.packageName;
            if (app.iconUrl != null)
            {
                this.iconUrl = StrUrl + app.iconUrl.Replace("\"", "");
            }
            else
            {
                this.iconUrl = string.Empty;
            }

            if (app.downloadUrl != null && app.downloadUrl!="" )
            {
                this.downloadUrl = StrUrl + app.downloadUrl.Replace("\"", "");
            }
            else
            {
                this.downloadUrl = string.Empty;
            }
           
            this.authorizedby = app.authorizedby;
            this.classification = app.classification;
            this.description = app.description;
            this.downloadNum = app.downloadNum;
            this.producers = app.producers;
            this.version = app.version;
            if(app.appDetailImageUrl!=null && app.appDetailImageUrl!="[]")
            {
                string[] strArray = app.appDetailImageUrl.Replace("[", "").Replace("]", "").Replace("\"","").Split(',');
                string[] NewStrArray = new string[strArray.Length];
                for (int i = 0; i < strArray.Length; i++)
                {
                    NewStrArray[i] = StrUrl + strArray[i];
                }

                this.appDetailImageUrl = NewStrArray;
            }
            else
            {
                this.appDetailImageUrl = new string[] { };
            }
            
        }
    }

}