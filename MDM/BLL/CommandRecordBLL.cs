using MDM.DAL;
using MDM.Helpers;
using MDM.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.BLL
{
    public class CommandRecordBLL:BLLBase<CommandRecord>
    {
        public static readonly new CommandRecordBLL Instance = new CommandRecordBLL();

        private CommandRecordBLL()
        {
            
        }

        //命令入库
        public  bool InsertRecord(BsonArray terminals,string id,Command cmd,string reportid,string currentUser)
        {
            bool res = true;

           
            for (int i = 0; i < terminals.Count; i++)
            {
                CommandRecord Record = new CommandRecord();
                Terminal t;
                Record.CommandId = cmd.Code;    //这里已纠正 
                Record.requestId = id;
                Record.CommandName = cmd.Description; 
                Record.terminalId = terminals[i].ToString();
                Record.reportId = reportid;
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
                Record.sendCommndUser = currentUser;

              
                try
                {
                    if (false == CommandRecordBLL.Instance.Add(Record))
                    {
                        res = false;
                        LogHelper.WriteInfoLog(typeof(CommandRecordBLL), "插入命令记录失败" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                   

                }
                catch (Exception ex)
                {
                    LogHelper.WriteInfoLog(typeof(CommandRecordBLL), "异常：插入发送命令异常 ========" + ex.Message + "   " + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }

            }


            return res;
        }

        //命令回告记录更正
        public bool UpdateRecord(ResponseData data,CommandRecord commandRecord)
        {
            bool res = true;
            
            commandRecord.status = data.status;
            commandRecord.statusName = Utils.GetDictText(commandRecord.status, DictType.Status);
            commandRecord.UpdateTime = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            
            try
            {
                if (false == CommandRecordBLL.Instance.Update(Query.And(
                                                             Query<CommandRecord>.EQ(p => p.requestId, commandRecord.requestId),
                                                             Query<CommandRecord>.EQ(p => p.terminalId, commandRecord.terminalId)), commandRecord))
                {
                    res = false;
                    LogHelper.WriteInfoLog(typeof(CommandRecordBLL), "更新命令回告失败" + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteInfoLog(typeof(CommandRecordBLL), "异常：更新命令回告异常========" + ex.Message + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return res;
        }

        

      
        
        
    }
}