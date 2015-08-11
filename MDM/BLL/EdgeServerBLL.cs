using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MDM.BLL
{
    public class EdgeServerBLL
    {
        public static EdgeServerResponse EdgeServerDilivery(EdgeServerData edgeServer)
        {
            //为客户端业务数据
            if (edgeServer.serviceID == 1)
            {
                return DiliveryClientData(edgeServer.payload);
            }
            //为EdgeServer回告
            else if (edgeServer.serviceID == 2)
            {
                return ProcessResponseData(edgeServer.payload);
            }
            //为客户端登陆数据
            else if (edgeServer.serviceID == 3)
            {
                return ProcessLogonData(edgeServer.payload);
            }
            return new EdgeServerResponse(HttpStatusCode.BadRequest);
        }

        public static EdgeServerResponse DiliveryClientData(object payload)
        {
            try
            {
                ClientData clientData = JsonConvert.DeserializeObject<ClientData>(payload.ToString());
                //string info = "收到客户端数据，terminal为" + clientData.terminalID + "，deviceKey为" + clientData.data.deviceKey;
                //LogHelper.WriteInfoLog(typeof(LogonData), info);

                List<string> router = Utils.GetRouter(clientData.data.interfaceID);

                if (router != null)
                {
                    //获取类型信息
                    Type t = Type.GetType(router[0]);
                    //根据类型创建对象
                    object dObj = Activator.CreateInstance(t);
                    //获取方法的信息
                    MethodInfo method = t.GetMethod(router[1]);
                    //调用方法的一些标志位，这里的含义是Public并且是静态方法，默认值为BindingFlags.Public | BindingFlags.Instance
                    BindingFlags flag = BindingFlags.Public | BindingFlags.Static;
                    //方法的参数
                    object[] parameters = new object[] { clientData };
                    //调用方法，用一个object接收返回值
                    object returnValue = method.Invoke(dObj, flag, Type.DefaultBinder, parameters, null);
                    if ((bool)returnValue == false)
                        return new EdgeServerResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception)
            {
                return new EdgeServerResponse(HttpStatusCode.BadRequest);
            }

            return new EdgeServerResponse(HttpStatusCode.OK);
        }


        public static EdgeServerResponse ProcessResponseData(object payload)
        {
            try
            {
                ResponseData data = JsonConvert.DeserializeObject<ResponseData>(payload.ToString());

                int Result = Convert.ToInt32(data.responseID.Substring(0, 2));

                List<string> Router = Utils.GetResponsRouter(Result);

                if (Router != null)
                {
                    Type t = Type.GetType(Router[0]);

                    object obj = Activator.CreateInstance(t);

                    MethodInfo method = t.GetMethod(Router[1]);

                    BindingFlags flag = BindingFlags.Public | BindingFlags.Static;

                    object[] parameters = new object[] { data };

                    object returnObj = method.Invoke(obj, flag, Type.DefaultBinder, parameters, null);

                    if ((bool)returnObj == false)
                        return new EdgeServerResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception)
            {
                return new EdgeServerResponse(HttpStatusCode.BadRequest);
            }

            return new EdgeServerResponse(HttpStatusCode.OK);
        }

        public static EdgeServerResponse ProcessLogonData(object payload)
        {
            LogonData data = new LogonData();

            try
            {
                data = JsonConvert.DeserializeObject<LogonData>(payload.ToString());
                //LogHelper.WriteInfoLog(typeof(LogonData), "收到登录数据，用户名为" + data.username);
            }
            catch (Exception)
            {
                return new EdgeServerResponse(HttpStatusCode.BadRequest);
            }

            //检查用户名密码是否正确
            LogonResult result = new LogonResult();
            try
            {

                var user = UserBLL.GetUserByUsername(data.username);
                if (user == null)
                {
                    result.result = 1;
                    LogHelper.WriteInfoLog(typeof(LogonData), "当前登录用户不存在，用户名为" + data.username);
                }
                else
                {

                    if (user.Password == data.password)
                    {
                        result.result = 0; //登录成功
                        //检查要登录的该设备是否已经在系统中存在，若存在，则返回其TerminalID，否则生成新的TerminalID
                        var t = TerminalBLL.GetTerminal(data);
                        string Name = "默认策略组";

                        #region 登录成功 TerminalID 操作
                        if (t == null)
                        {
                            Terminal newt = new Terminal();
                            newt.UserId = user.ID;
                            newt.UserGroup = UserBLL.GetUserGroupById(user.ID);
                            newt.User = data.username;
                            newt.IMEI = data.imei;
                            newt.PhoneNumber = data.phoneNumber;
                            newt.AppID = data.appID;
                            newt.DeviceSN = data.deviceSN;
                            newt.Status = 1;
                            StrategyGroup groupModel = StrategyGroupBLL.GetStrategyGroupByName(Name);
                            if (groupModel != null)
                            {
                                newt.policyID = groupModel.ID;
                            }
                            string id = TerminalBLL.Instance.AddAndReturnID(newt);
                            if (id != string.Empty)
                            {
                                result.terminalID = id;
                                if (user.Terminals == null)
                                    user.Terminals = new MongoDB.Bson.BsonArray();
                                user.Terminals.Add(id);
                                UserBLL.Instance.Update(user.ID, user);
                            }
                        }
                        else if (t.User == data.username)
                        {
                            result.terminalID = t.ID;
                        }
                        else
                        {
                            result.result = 4;
                        }
                        #endregion

                        #region 发送策略信息

                        //StrategyItemBLL.SendStrategyItemMsg(result.terminalID);
                        #endregion

                    }
                    else
                    {
                        result.result = 2;
                        LogHelper.WriteInfoLog(typeof(LogonData), "当前登录用户密码错误，用户名为" + data.username);
                    }
                }

            }
            catch (Exception)
            {
                result.result = 3;
            }
            return new EdgeServerResponse(HttpStatusCode.OK, result);
        }

        //向Client发送数据
        public static HttpStatusCode SendMessage2Client(BsonArray terminals, object data, RequestType type, out string Id)
        {
            Send2EdgeServer message = new Models.Send2EdgeServer();
            message.requestID = Utils.ReturnRequestID(type);
            Id = message.requestID;
            message.terminals = terminals;
            message.data = data;
            string json = JsonConvert.SerializeObject(message);
            return Send2EdgeServer("EdgeServerInterface", "POST", json);
        }

        //这部分暂不进行任何改动
        public static HttpStatusCode SendMessage2Client(BsonArray terminals, object data, RequestType type)
        {
            Send2EdgeServer message = new Models.Send2EdgeServer();
            message.requestID = Utils.ReturnRequestID(type);
            message.terminals = terminals;

            message.data = data;
            string json = JsonConvert.SerializeObject(message);
            return Send2EdgeServer("EdgeServerInterface", "POST", json);
        }


        //向EdgeServer发送数据，serviceName为配置文件路径提取参数
        public static HttpStatusCode Send2EdgeServer(string serviceName, string methodType, string json)
        {
            Uri url = RestHelper.GetServiceUrlFromConfig(serviceName);
            HttpStatusCode status = RestHelper.RemoteRequest(url, methodType, json);
            return status;
        }

    }
}