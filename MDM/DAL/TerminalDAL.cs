using MDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.DAL
{
    public sealed class TerminalDAL : DALBase<Terminal>
    {
        public static readonly new TerminalDAL Instance = new TerminalDAL();

        private TerminalDAL()
        {
        }
    }
}