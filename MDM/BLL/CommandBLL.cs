using MDM.DAL;
using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver.Builders;
using System.Configuration;

namespace MDM.BLL
{
    public class CommandBLL : BLLBase<Command>
    {
        public static readonly new CommandBLL Instance = new CommandBLL();
        
        public CommandBLL()
        {

        }
      
        //发送命令消息
        public  bool SendCommandToTerminals(string cid, IEnumerable<string> tids,string currentUser)
        {
            var res = true;

            try
            {
                
                string[] ids = tids.ToArray();

                string Id;

                #region 命令项信息
                PfRequest pfrequest = new PfRequest();
                pfrequest.pfRequestID = Guid.NewGuid().ToString();
                pfrequest.action = GetCommand(cid).Code;
                #endregion

                #region 客户端命令项
                Send2Client clientMsg = new Send2Client();
                clientMsg.commandID = "2";
                clientMsg.pfData = pfrequest; 
                #endregion

                BsonArray terminals = new BsonArray();

                for (int i = 0; i < ids.Length; i++)
                {
                    terminals.Add(ids[i]);
                }

                HttpStatusCode result = EdgeServerBLL.SendMessage2Client(terminals, clientMsg, RequestType.Command, out Id);

                if (result != HttpStatusCode.OK)
                {
                    res = false;
                    LogHelper.WriteErrorLog(typeof(CommandBLL), "发送命令到EdgeServer失败" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                     PfRequest msg= clientMsg.pfData as PfRequest;
                     
                    if (false == CommandRecordBLL.Instance.InsertRecord(terminals, Id, GetCommand(cid),msg.pfRequestID,currentUser))
                    {
                        res = false;
                    }

                }

                return res;


            }
            catch (Exception ex)
            {

                LogHelper.WriteErrorLog(typeof(CommandBLL), "异常：发送命令消息到edgs异常========" + ex.Message + "  " + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return res;
        }


         public static Command GetCommand( string id)
        {
            
            string Code = string.Empty;
            Command cmd;

            bool res= CommandBLL.Instance.GetByQuery(Query.And(Query<Command>.EQ(p => p.ID, id)), out cmd);

            if(cmd==null)
            {
                LogHelper.WriteInfoLog(typeof(CommandBLL), "未到相关获取命令信息" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }
            
            return cmd;   
                
        }
       

        //回告信息
        public static bool ResponseMsg( ResponseData content )
        {
            bool res = true;
            CommandRecord commandRecord;

            try
            {
                
                if( (true== CommandRecordBLL.Instance.GetByQuery(Query.And(Query<CommandRecord>.EQ(p => p.requestId, content.responseID),
                                                             Query<CommandRecord>.EQ(p => p.terminalId, content.terminalID)),    out commandRecord)) &&(commandRecord!=null) )
                {
                    

                    if (false == CommandRecordBLL.Instance.UpdateRecord(content, commandRecord))
                    {
                        res = false;

                    }
                   
                }
                else
                {
                    LogHelper.WriteInfoLog(typeof(CommandBLL), "未查询到此命令发送记录" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(CommandBLL), "异常：插入发送命令========" + ex.Message + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                
            }

            return res;

        }

       
    }

    public class CommandRouter
    {
        public static bool CommandReportMsg(ClientData clientData)
        {
            bool res = true;
            CommandRecord record;
            try
            {
                CommandMessage msg = JsonConvert.DeserializeObject<CommandMessage>(clientData.data.pfData.ToString());

                if ((true == CommandRecordBLL.Instance.GetByQuery(Query.And(Query<CommandRecord>.EQ(p => p.reportId, msg.responseID),
                                                             Query<CommandRecord>.EQ(p => p.terminalId, clientData.terminalID)), out record)) && (record != null))
                {
                    //1. 把命令的状态进行修改   
                    //2. 把terminal的状态进行修改 
                    record.status = Convert.ToInt32(msg.result.Trim());
                    record.statusName = Utils.GetDictText(record.status, DictType.CommandStatus);
                    record.UpdateTime = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

                    try
                    {
                        if(false == CommandRecordBLL.Instance.Update(Query.And(
                                                                     Query<CommandRecord>.EQ(p => p.reportId, msg.responseID),
                                                                     Query<CommandRecord>.EQ(p => p.terminalId, clientData.terminalID )), record))
                        {
                            res = false;
                            LogHelper.WriteInfoLog(typeof(CommandBLL), "更新命令下达回复上告失败");
                        }

                        
                        Terminal ter;
                        if( (false== TerminalBLL.Instance.GetByQuery( Query<Terminal>.EQ(p=>p.ID,clientData.terminalID),out 

    ter)) &&(ter==null) )
                        {
                            res = false;
                            LogHelper.WriteInfoLog(typeof(CommandBLL), "未找到相关终端信息,终端id是"+clientData.terminalID);
                        }
                        else
                        {
                            if (record.CommandId == "gps")  
                            {
                                ter.Location = msg.parameter;

                                if(false==TerminalBLL.Instance.Update(Query<Terminal>.EQ(p=>p.ID,clientData.terminalID),ter))
                                {
                                    res = false;
                                    LogHelper.WriteInfoLog(typeof(CommandBLL), "更新终端地理位置信息失败");
                                }
                            }
                            
                            
                        }
                        

                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteInfoLog(typeof(CommandBLL), "异常：更新命令下达回复上告异常========" + ex.Message);
                    }

                }
                else
                {
                    LogHelper.WriteInfoLog(typeof(CommandBLL), "未查询到此命令发送记录" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(CommandBLL), "命令下达回复上告失败" + ex.Message);

                res = false;
            }
          
            return res;
        }


    }
}