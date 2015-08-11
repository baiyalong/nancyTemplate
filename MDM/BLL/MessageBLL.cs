using MDM.Models;
using MongoDB.Bson;
using Nancy;
using System;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Driver.Builders;
using System.Linq;
using MDM.Helpers;
using MongoDB.Driver;

namespace MDM.BLL
{
    public class MessageBLL : BLLBase<MessageRecord>
    {
        public static readonly new MessageBLL Instance = new MessageBLL();

        public MessageBLL()
        {

        }

        //消息下发  
        public bool SendMessageToTerminals(string msg,string title, IEnumerable<string> tids,string currentUser)
        {
            var res = true;

            try
            {
                string Id;

                string[] ids = tids.ToArray();

                #region 消息项
                SendMessage msgs = new SendMessage();
                msgs.message = msg;
                msgs.title = title;
                #endregion


                #region 客户端
                Send2Client clientMsg = new Send2Client();
                clientMsg.commandID = "3";
                clientMsg.pfData = msgs; 
                #endregion

                BsonArray terminals = new BsonArray();

                for (int i = 0; i < ids.Length; i++)
                {
                    terminals.Add(ids[i]);
                }

                HttpStatusCode result = EdgeServerBLL.SendMessage2Client(terminals, clientMsg, RequestType.Message, out Id);

                if (result != HttpStatusCode.OK)
                {
                    res = false;
                    LogHelper.WriteInfoLog(typeof(MessageBLL), "消息下发发送到edgs失败");
                }
                else
                {

                    if (false == MessageBLL.Instance.InsertRecord(terminals, Id, msg, title, currentUser))
                    {
                        res = false;
                    }

                }

            }
            catch (Exception ex)
            {

                LogHelper.WriteInfoLog(typeof(MessageBLL), "异常：消息下发到edgs异常========" + ex.Message + "  " + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return res;
        }


        //消息入库
        public bool InsertRecord(BsonArray terminals, string id,string content,string title,string currentUser)
        {
            bool res = true;

            #region 命令发送成功执行操作
            for (int i = 0; i < terminals.Count; i++)
            {
                MessageRecord Record = new MessageRecord();
                Terminal t;

                Record.title = title;
                Record.MessageContent = content;
                Record.requestId = id;
                Record.terminalId = terminals[i].ToString().Trim();
                #region 获取命令发送终端信息
                if ((true == TerminalBLL.Instance.GetByQuery(Query<Terminal>.EQ(p => p.ID, terminals[i].ToString()), out t)))
                {
                    Record.user = t.User;
                    Record.phoneNumber = t.PhoneNumber;
                    Record.deviceName = t.DeviceName;
                    Record.imei = t.IMEI;
                }
                #endregion
                Record.status = (int)SendStaus.Sending;
                Record.statusName = Utils.GetDictText((int)SendStaus.Sending, DictType.Status);

                Record.SendTime = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                Record.UpdateTime = null;
                Record.currentUser = currentUser;

                try
                {

                    if (false ==MessageBLL.Instance.Add(Record))
                    {
                        res = false;
                        LogHelper.WriteInfoLog(typeof(MessageBLL), "插入消息下发记录失败" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteInfoLog(typeof(MessageBLL), "异常：插入消息下发记录异常 ========" + ex.Message + "   " + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }

            }

            #endregion

            return res;
        }



        
        #region 消息下发的回告
        public static bool ResponseMsgRecord(ResponseData content)
        {
            bool res = true;
            MessageRecord msg;

            try
            {
              

                if (true == MessageBLL.Instance.GetByQuery(Query.And(Query<MessageRecord>.EQ(p => p.requestId, content.responseID),
                                                             Query<MessageRecord>.EQ(p => p.terminalId, content.terminalID)), out msg))
                {

                    if (false == MessageBLL.Instance.UpdateRecord(content, msg))
                    {
                        res = false;

                    }

                }
                else
                {
                    LogHelper.WriteInfoLog(typeof(MessageBLL), "查询发送消息记录" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(MessageBLL), "异常：插入发送消息========" + ex.Message + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));

            }


            return res;

        } 
        #endregion


        
       
        public bool UpdateRecord(ResponseData data, MessageRecord msg)
        {
            bool res = true;

            
            msg.status = data.status;
            msg.statusName = Utils.GetDictText(data.status, DictType.Status);
            msg.UpdateTime = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                if (false == MessageBLL.Instance.Update(Query.And(
                                                             Query<MessageRecord>.EQ(p => p.requestId, msg.requestId),
                                                             Query<MessageRecord>.EQ(p => p.terminalId, msg.terminalId)), msg))
                {
                    res = false;
                    LogHelper.WriteInfoLog(typeof(MessageBLL), "更新消息回告失败" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(MessageBLL), "异常：更新消息回告异常========" + ex.Message + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return res;
        } 
        


    }
}