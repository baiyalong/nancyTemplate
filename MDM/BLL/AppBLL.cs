using MDM.Models;
using MongoDB.Bson;
using Nancy;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Driver.Builders;
using System.Linq;
using System.Web;
using MDM.Helpers;
using MongoDB.Driver;
using System.Threading;
using System.Configuration;

namespace MDM.BLL
{
    public class AppBLL : BLLBase<App>
    {
        public static readonly new AppBLL Instance = new AppBLL();
        private AppBLL()
        {
        }

        //装填应用详情
        public static AppDetail detail = null;
        //装填应用列表   
        public static Apps appmodellist = null;


        #region 获取应用分类信息
        public bool GetAllClassifyApps(string cid, out List<App> apps)
        {
            bool res = true;

            AppClassify appclassify;
            List<App> list = new List<App>();
            List<App> resultlist = new List<App>();

            try
            {
                if (true == AppClassifyBLL.Instance.GetByQuery(Query<AppClassify>.EQ(x => x.ID, cid), out appclassify))
                {
                    foreach (string appid in appclassify.apps)
                    {
                        App app;
                        if (true == AppBLL.Instance.GetByQuery(Query<App>.EQ(x => x.ID, appid), out app))
                        {
                            if (app != null)
                            {
                                list.Add(app);
                            }
                        }
                    }

                    foreach (App appmodel in list.ToArray())
                    {
                        if (appmodel.classify == cid)
                        {
                            resultlist.Add(appmodel);
                        }
                    }

                }

            }
            catch (Exception)
            {
                res = false;

            }
            apps = resultlist;

            return res;
        }
        #endregion


        //获取指定分类下应用列表
        public static bool GetClassifyAppList(string terminalId, string classifyId)
        {
            bool res = false;

            AppClassify appclassify;
            List<App> list = new List<App>();
            List<AppModel> appList = new List<AppModel>();
            Apps appsmsg = new Apps();

            if ((true == AppClassifyBLL.Instance.GetByQuery(Query<AppClassify>.EQ(x => x.ID, classifyId), out appclassify)) && (appclassify != null))
            {
                #region 获取分类列表信息
                foreach (string appid in appclassify.apps)
                {
                    App app;
                    Terminal t;
                    if ((true == AppBLL.Instance.GetByQuery(Query<App>.EQ(x => x.ID, appid), out app)) && (app != null) && (app.UserGroup != null)
                        && (true == TerminalBLL.Instance.GetByQuery(Query<Terminal>.EQ(x => x.ID, terminalId), out t)) && (t != null) && (t.UserGroup != null))
                    {
                        //已发布 0  待发布1  待编辑2
                        if (app.Status == 0)
                        {
                            foreach (string t_ug in t.UserGroup)
                            {
                                if (app.UserGroup.Contains(t_ug))
                                { 
                                    list.Add(app);
                                    break;
                                }
                            }
                            res = true;
                        }
                    }
                }
                list.Distinct();
                foreach (App appmodel in list)
                {
                    AppModel model = new AppModel();
                    model.App2AppModel(appmodel);
                    if (appmodel.classify == classifyId)
                    {
                        appList.Add(model);
                    }
                }
                appsmsg.classifyID = classifyId;
                appsmsg.apps = appList;
                #endregion

                #region 重构发送信息
                Send2Client clientMsg = new Send2Client();
                clientMsg.commandID = "6";
                clientMsg.pfData = appsmsg;

                BsonArray terminals = new BsonArray();
                terminals.Add(terminalId);
                #endregion

                //使用线程池发送信息
                //ThreadPoolWork(terminals, clientMsg);  
                //之前执行方式 发送
                SendAppsMsg(terminals, clientMsg);
            }
            return res;
        }

        //通过线程池来执行发送消息任务
        public static void ThreadPoolWork(BsonArray terminals, Send2Client msg)
        {
            bool Flag = false;
            int MaxCount = 1;
            ManualResetEvent eventX = new ManualResetEvent(false);
            Dowork WorkTread = new Dowork(MaxCount);
            WorkTread.eventX = eventX;
            try
            {
                ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WorkTread.Work), new SomeParameter());
                Flag = true;
            }
            catch (NotSupportedException)
            {
                Flag = false;
            }

            if (Flag)
            {

                //for (int iItem = 1; iItem < MaxCount; iItem++)
                //{
                //    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WorkTread.Work), new Parameters(5, "dtrrefdsfs"));
                //}
                ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WorkTread.Work), new SomeParameter(terminals, msg));
                eventX.WaitOne(Timeout.Infinite, true);
            }

        }

        //发送应用 
        public static void SendAppsMsg(BsonArray terminals, Send2Client msg)
        {
            HttpStatusCode result = EdgeServerBLL.SendMessage2Client(terminals, msg, RequestType.defal);
            //后续添加result不为ok时的处理
            if (result != HttpStatusCode.OK)
                LogHelper.WriteInfoLog(typeof(AppBLL), "发送应用列表到EdgeServer失败");
        }

        //获取应用详情
        public static bool GetAppMsg(string terminalId, string appId)
        {
            bool res = false;

            try
            {
                App appDetail = new App();

                if (true == AppBLL.Instance.GetByQuery(Query<App>.EQ(u => u.ID, appId), out appDetail))
                {
                    res = true;
                }

                if (detail == null)
                {
                    detail = new AppDetail();
                    detail.appDetail = new AppDetailModel();
                }
                if (appDetail != null)
                {
                    detail.appDetail.App2AppDetailModel(appDetail);
                }

                #region 重构应用详情
                Send2Client clientMsg = new Send2Client();
                clientMsg.commandID = "5";
                clientMsg.pfData = detail;

                BsonArray terminals = new BsonArray();
                terminals.Add(terminalId);
                #endregion

                //这里同获取应用的分类信息雷同  测试后确认 


                //发送详情  
                SendMsg(terminals, clientMsg);
            }
            catch (Exception)
            {

            }

            return res;
        }

        //发送应用详情
        public static void SendMsg(BsonArray tids, Send2Client detail)
        {

            HttpStatusCode result = EdgeServerBLL.SendMessage2Client(tids, detail, RequestType.defal);
            //后续添加result不为ok时的处理
            if (result != HttpStatusCode.OK)
                LogHelper.WriteInfoLog(typeof(AppBLL), "发送应用详情到EdgeServer失败" + result.ToString());
        }

        //建立应用及特定分类关联
        public bool SetAppClassify(string appid, string classify)
        {
            var res = false;

            App app;
            AppClassify app_classify;

            if (appid != null)
            {
                if ((true == AppBLL.Instance.GetByQuery(Query<App>.EQ(p => p.ID, appid), out app)) && (app != null))
                {
                    app.classify = classify;
                    if (true == AppBLL.Instance.Update(Query.And(Query<App>.EQ(p => p.ID, appid)), app))
                    {
                        res = true;

                    }
                }
            }

            if (classify != null)
            {
                if ((true == AppClassifyBLL.Instance.GetByQuery(Query.And(Query<AppClassify>.EQ(p => p.ID, classify)), out app_classify)) && (app_classify != null))
                {
                    if (app_classify.apps == null)
                        app_classify.apps = new BsonArray();

                    if (!app_classify.apps.Contains(appid))
                    {
                        app_classify.apps.Add(appid);
                    }

                    if (true == AppClassifyBLL.Instance.Update(Query.And(Query<AppClassify>.EQ(p => p.ID, classify)), app_classify))
                    {
                        res = true;
                    }
                }

            }

            return res;
        }

        //取消应用及应用分类关联
        public bool CancleAppClassifyRelation(string appid, string classifyid)
        {
            bool res = false;

            AppClassify t;
            if ((true == AppClassifyBLL.Instance.GetByQuery(Query<AppClassify>.EQ(x => x.ID, classifyid), out t)) && (t != null))
            {
                if (t.apps.Contains(appid))
                {
                    t.apps.Remove(appid);

                    if (true == AppClassifyBLL.Instance.Update(Query<AppClassify>.EQ(x => x.ID, classifyid), t))
                    {
                        res = true;
                    }
                }
                else
                {
                    res = true;
                }
            }

            App p;
            if ((true == AppBLL.Instance.GetByQuery(Query<App>.EQ(x => x.ID, appid), out p)) && (p != null))
            {
                p.classify = string.Empty;
                if ((true == AppBLL.Instance.Update(Query<App>.EQ(x => x.ID, appid), p)))
                {
                    res = true;
                }
            }

            return res;
        }

        public bool DeleteAppClassifyRelation(string appid, string tid, string cid)
        {
            bool res = false;

            AppTemplate temp;
            AppClassify classify;

            if ((true == AppClassifyBLL.Instance.GetByQuery(Query<AppClassify>.EQ(x => x.ID, cid), out classify)) && (classify != null))
            {
                if (classify.apps.Contains(appid))
                {
                    classify.apps.Remove(appid);

                    if (true == AppClassifyBLL.Instance.Update(Query<AppClassify>.EQ(x => x.ID, cid), classify))
                    {
                        if ((true == AppTemplateBLL.Instance.GetByQuery(Query<AppTemplate>.EQ(p => p.ID, tid), out temp)) && (temp != null))
                        {
                            if (temp.apps.Contains(appid))
                            {
                                temp.apps.Remove(appid);

                                if (true == AppTemplateBLL.Instance.Update(Query<AppTemplate>.EQ(p => p.ID, tid), temp))
                                {
                                    //删除appid 是自动生成id
                                    if (true == AppBLL.Instance.Delete(appid))
                                    {
                                        res = true;
                                    }

                                }
                            }

                        }
                    }
                }



            }

            return res;
        }

        public bool DeleteAppRelation(string appid)
        {
            bool res = false;

            App app;

            if ((true == AppBLL.Instance.GetByQuery(Query<App>.EQ(p => p.ID, appid), out app)) && (app != null))
            {
                try
                {
                    if (app.classify != null)
                    {
                        AppClassify classify;
                        if ((true == AppClassifyBLL.Instance.GetByQuery(Query<AppClassify>.EQ(p => p.ID, app.classify), out classify)) && (classify != null))
                        {
                            if (classify.apps != null)
                            {
                                if (classify.apps.Contains(appid))
                                {
                                    classify.apps.Remove(appid);
                                }
                            }

                            if (true == AppClassifyBLL.Instance.Update(Query<AppClassify>.EQ(p => p.ID, app.classify), classify))
                            {
                                res = true;
                            }

                        }
                        res = false;
                    }


                    if (app.Template != null)
                    {
                        AppTemplate template;
                        if ((true == AppTemplateBLL.Instance.GetByQuery(Query<AppTemplate>.EQ(p => p.ID, app.Template), out template)) && (template != null))
                        {
                            if (template.apps != null)
                            {
                                if (template.apps.Contains(appid))
                                {
                                    template.apps.Remove(appid);
                                }
                            }

                            if (true == AppTemplateBLL.Instance.Update(Query<AppTemplate>.EQ(p => p.ID, app.Template), template))
                            {
                                res = true;
                            }
                            res = false;
                        }
                    }

                    if (true == AppBLL.Instance.Delete(appid))
                    {
                        res = true;
                    }

                }
                catch (Exception ex)
                {
                    res = false;
                    LogHelper.WriteErrorLog(typeof(AppBLL), "删除App异常信息：" + ex.Message);

                }

            }

            return res;
        }


        public static bool SetRalationBetweenAppAndUgerGroups(string appid, IEnumerable<string> groups)
        {
            bool res = false;
            List<string> usergroups = new List<string>();
            List<string> terminals = new List<string>();
            BsonArray array = new BsonArray();
            string msg = string.Empty;
            string appcontent = string.Empty;
            string StrUrl = ConfigurationManager.AppSettings["StrUrl"].ToString();
            foreach (var g in groups)
            {
                usergroups.Add(g);
            }

            App app;
            if ((true == AppBLL.Instance.GetByQuery(Query<App>.EQ(x => x.ID, appid), out app)) && (app != null))
            {
                res = true;
                app.UserGroup = usergroups.ToArray();

                if (true == AppBLL.Instance.Update(Query<App>.EQ(x => x.ID, appid), app))
                {
                    terminals = GetBsonArray(usergroups);
                    if (terminals.Count != 0)
                    {
                        if (app.isMandatory == true)
                        {
                            msg = "appPush";
                            appcontent = app.ID + "@" + 0 + "@" + app.appName + "@" + app.packageName + "@" + app.version + "@" + StrUrl + app.downloadUrl.Replace("\"", "");

                        }
                        else if (app.isRecommended == true)
                        {
                            msg = "appPush";
                            appcontent = app.ID + "@" + 1 + "@" + app.appName + "@" + app.packageName + "@" + app.version + "@" + StrUrl + app.downloadUrl.Replace("\"", "");
                        }


                        for (int i = 0; i < terminals.Count; i++)
                        {
                            if (!array.Contains(terminals[i].ToString()))
                            {
                                array.Add(terminals[i].ToString());
                            }

                        }

                        SendTerminalMsg(array, msg, appcontent);
                    }
                    else
                    {
                        LogHelper.WriteErrorLog(typeof(AppBLL), "无获取终端信息,不发送app应用信息");
                    }

                }
            }

            return res;
        }

        private static List<string> GetBsonArray(List<string> groups)
        {

            List<string> tids = new List<string>();

            IEnumerable<Terminal> oul;

            Pattern pattern = new Pattern();
            pattern.pageNum = 1;
            pattern.pageSize = 500;

            if ((true == TerminalBLL.Instance.GetList(pattern, out oul)) && (oul != null))
            {
                foreach (var t in oul)
                {
                    if (t.UserGroup != null)
                    {
                        for (int i = 0; i < t.UserGroup.Count(); i++)
                        {
                            if (groups.Contains(t.UserGroup[i].ToString()))
                            {
                                tids.Add(t.ID);
                            }
                        }
                    }

                }
            }

            return tids;
        }

        private static bool SendTerminalMsg(BsonArray tids, string action, string content)
        {
            var res = true;

            #region 命令项信息
            PfRequest pfrequest = new PfRequest();
            pfrequest.pfRequestID = null;
            pfrequest.action = action;
            pfrequest.extraInfo = content;
            #endregion

            #region 客户端命令项
            Send2Client clientMsg = new Send2Client();
            clientMsg.commandID = "2";
            clientMsg.pfData = pfrequest;
            #endregion

            HttpStatusCode result = EdgeServerBLL.SendMessage2Client(tids, clientMsg, RequestType.Command);

            if (result != HttpStatusCode.OK)
            {
                res = false;
                LogHelper.WriteErrorLog(typeof(AppBLL), "发送命令到EdgeServer失败" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return res;
        }

    }

    public class AppRouter
    {
        public AppRouter() { }

        public static AppDetail detail = null;

        //获取指定分类下应用列表
        public static bool GetAppList(ClientData clientData)
        {
            //暂不做获取应用的权限设置，clientdata的pfdata字段为空，后续可以添加省份等条件
            bool res = false;
            ClassifyMessage data = null;

            try
            {
                if (data == null)
                {
                    data = new ClassifyMessage();
                }

                data = JsonConvert.DeserializeObject<ClassifyMessage>(clientData.data.pfData.ToString());

                if (true == AppBLL.GetClassifyAppList(clientData.terminalID, data.classifyID))
                {
                    res = true;
                }

            }
            catch (Exception)
            {

            }

            return res;
        }

        //获取应用详情 
        public static bool GetAppDetail(ClientData clientData)
        {
            bool res = false;
            AppMessage data = null;
            try
            {
                if (data == null)
                {
                    data = new AppMessage();
                }
                data = JsonConvert.DeserializeObject<AppMessage>(clientData.data.pfData.ToString());
            }
            catch (Exception)
            {
            }
            res = true;

            if (false == AppBLL.GetAppMsg(clientData.terminalID, data.AppId))
            {
                res = false;

            }

            return res;
        }




    }


    /// <summary>
    /// 工作类
    /// </summary>
    public class Dowork
    {

        public ManualResetEvent eventX;
        public static int iCount = 0;
        public static int iMaxCount = 0;

        public Dowork(int str)
        {
            iMaxCount = str;
        }

        /// 线程池里的线程将调用 work()
        /// </summary>
        /// <param name="state"></param> 
        public void Work(Object state)
        {
            if (((SomeParameter)state).msg != null)
            {
                HttpStatusCode result = EdgeServerBLL.SendMessage2Client(((SomeParameter)state).terminals, ((SomeParameter)state).msg, RequestType.defal);

                if (result != HttpStatusCode.OK)
                    LogHelper.WriteInfoLog(typeof(AppBLL), "发送应用列表到EdgeServer失败");

                Thread.Sleep(3000);

                Interlocked.Increment(ref iCount);

                if (iCount == iMaxCount)
                {
                    eventX.Set();
                }

            }

        }
    }

    public class SomeParameter
    {
        public BsonArray terminals;

        public Send2Client msg;

        public SomeParameter()
        {

        }

        public SomeParameter(BsonArray str, Send2Client strs)
        {
            terminals = str;
            msg = strs;
        }
    }





}