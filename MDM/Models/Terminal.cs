using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class Terminal : ModelBase
    {
        /// <summary>
        /// 用户名称
        /// </summary>        
        public string User { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
         
        /// <summary>
        /// 设备IMEI
        /// </summary>        
        public string IMEI { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>        
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>        
        public string AppID { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>        
        public string DeviceSN { get; set; }

        /// <summary>
        /// 操作系统类型（0：Android，1：IOS，2：WP）
        /// </summary>
        public string OSType { get; set; }

        /// <summary>
        /// 操作系统版本号
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// 内核版本号
        /// </summary>
        public string KernelVersion { get; set; }

        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Wifi Mac地址
        /// </summary>
        public string WifiMac { get; set; }

        /// <summary>
        /// 蓝牙地址
        /// </summary>
        public string BlueTooth { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 电量
        /// </summary>
        public string Power { get; set; }

        /// <summary>
        /// 手机总内存
        /// </summary>
        public string TotalRomSpace { get; set; }

        /// <summary>
        /// 手机剩余内存
        /// </summary>
        public string AvailRomSpace { get; set; }

        /// <summary>
        /// SD卡总内存
        /// </summary>
        public string TotalSDSpace { get; set; }

        /// <summary>
        /// SD卡剩余内存
        /// </summary>
        public string AvailSDSpace { get; set; }

        /// <summary>
        /// 当前的终端状态   1 在用  0 停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 策略组关联id
        /// </summary>
        public string policyID { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 终端所属用户组
        /// </summary>
        public List<string> UserGroup { get; set; }

        public override bool search(string ss)
        {
            return Pattern.verify(new List<object>() { 
            this.DeviceName,
            this.PhoneNumber,
            this.IMEI
            }, ss);
        }

    }
}