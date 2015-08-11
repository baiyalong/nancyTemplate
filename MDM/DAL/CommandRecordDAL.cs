using MDM.Models;
using MongoDB.Bson;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.DAL
{
    public class CommandRecordDAL:DALBase<CommandRecord>
    {
        public static readonly new CommandRecordDAL Instance = new CommandRecordDAL();

        private CommandRecordDAL()
        {
            
        }
    }
}