using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.Helpers;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MDM.Models;
using MongoDB.Bson;
using Nancy;

namespace MDM.BLL
{
    public class PunishmentBLL : BLLBase<Punishment>
    {
         public static readonly new PunishmentBLL Instance = new PunishmentBLL();
         private PunishmentBLL()
        {
        }


    }
}