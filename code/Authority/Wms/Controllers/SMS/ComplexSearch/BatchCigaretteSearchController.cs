using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Security;

namespace Wms.Controllers.SMS.ComplexSearch
{
    [TokenAclAuthorize]
    public class BatchCigaretteSearchController : Controller
    {
        //
        // GET: /BatchCigaretteSearch/

        public ActionResult Index()
        {
            return View();
        }

    }
}
