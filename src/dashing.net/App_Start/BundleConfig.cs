using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace dashing.net.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/application")
                            .Include("~/Scripts/jquery-1.9.1.js")
                            .Include("~/Scripts/es5-shim.js")
                            .Include("~/Scripts/gridster/jquery.leanModal.min.js")
                            .Include("~/Scripts/gridster/jquery.gridster.js")
                            .Include("~/Scripts/batman.js")
                            .Include("~/Scripts/batman.jquery.js")
                            .Include("~/Scripts/dashing.coffee"));
 
            BundleTable.EnableOptimizations = true;
        }
    }
}