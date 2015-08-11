using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.DAL
{
    public sealed class CommandDAL : DALBase<Command>
    {
        public static readonly new CommandDAL Instance = new CommandDAL();

        private CommandDAL()
        {
        }
    }
}