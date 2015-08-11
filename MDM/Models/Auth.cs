using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.Models
{
    public class Admin : ModelBase, IUserIdentity
    {
        /// <summary>
        /// Gets or sets the name of the current user.
        /// </summary>
        public string UserName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Gets or set the claims of the current user.
        /// </summary>
        public IEnumerable<string> Claims { get; set; }

    }
}