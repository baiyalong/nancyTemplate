using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace MDM.BLL
{
    public class AppTemplateBLL : BLLBase<AppTemplate>
    {
        public static readonly new AppTemplateBLL Instance = new AppTemplateBLL();
        private AppTemplateBLL()
        {
        }

        public bool SetAppTemplate(string tid, IEnumerable<string> appids)
        {
            bool res = false;

            AppTemplate temp = new AppTemplate();

            if ((true == AppTemplateBLL.Instance.GetByQuery(Query<AppTemplate>.EQ(p => p.ID, tid), out temp)) && (temp != null))
            {
                BsonArray apps = new BsonArray();
                foreach (string app in appids)
                {
                    apps.Add(app);
                }

                temp.apps = apps;

                if (true == AppTemplateBLL.Instance.Update(Query<AppTemplate>.EQ(p => p.ID, tid), temp))
                {
                    res = true;
                }

            }
            return res;
        }

        public bool SetAppTemplate(string tid, string appid)
        {
            bool res = false;

            AppTemplate temp = new AppTemplate();

            if ((true == AppTemplateBLL.Instance.GetByQuery(Query<AppTemplate>.EQ(p => p.ID, tid), out temp)) && (temp != null))
            {
                if (temp.apps == null)
                {
                    temp.apps = new BsonArray();
                }
                if(!temp.apps.Contains(appid))
                {
                    temp.apps.Add(appid);
                }

                if (true == AppTemplateBLL.Instance.Update(Query<AppTemplate>.EQ(p => p.ID, tid), temp))
                {
                    res = true;
                }

            }

            App app=new App();

            if((true==AppBLL.Instance.GetByQuery(Query<App>.EQ(p=>p.ID,appid),out app))&&(app!=null))
            {
                app.Template = tid;
                if(true==AppBLL.Instance.Update(Query<App>.EQ(p=>p.ID,appid),app))
                {
                    res = true;

                }
            }
            return res;
        }


        public bool DelAppFormTemplate(string tid, string appid)
        {
            bool res = false;
            AppTemplate AppTemp;

            if ((true == AppTemplateBLL.Instance.GetByQuery(Query<AppTemplate>.EQ(p => p.ID, tid), out AppTemp)) && (AppTemp != null))
            {
                if (AppTemp.apps.Contains(appid))
                {
                    AppTemp.apps.Remove(appid);

                    if (true == AppTemplateBLL.Instance.Update(Query<AppTemplate>.EQ(p => p.ID, tid), AppTemp))
                    {
                        res = true;
                    }
                }
                else
                {   //在应用模版中未找到应用信息 
                    res = true;
                }
            }

            App app;

            if((true==AppBLL.Instance.GetByQuery(Query<App>.EQ(p=>p.ID,appid),out app))&&(app!=null))
            {
                if(app.Template==tid)
                {
                    app.Template = string.Empty;
                    
                }
                if(true==AppBLL.Instance.Update(Query<App>.EQ(p=>p.ID,appid), app))
                {
                    res = true;
                }
                
            }


            return res;
        }

        public bool UpdateRelationBetweenTandA(string tid, string appid)
        {
            return false;
        }

        public bool GetAllAppMsg(string tid, out List<App> list)
        {
            bool res = true;
            List<App> apps = new List<App>();
            AppTemplate temp;

            try
            {
                if ((true == AppTemplateBLL.Instance.GetByQuery(Query<AppTemplate>.EQ(p => p.ID, tid), out temp)) && (temp != null))
                {
                    if (temp.apps != null)
                    {
                        for (int i = 0; i < temp.apps.Count; i++)
                        {
                            App app;
                            if ((true == AppBLL.Instance.GetByQuery(Query<App>.EQ(p => p.ID, temp.apps[i].ToString()), out app)))
                            {
                                if (app != null)
                                {
                                    apps.Add(app);
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
                res = false;
            }

            list = apps;
            return res;
        }
    }
}