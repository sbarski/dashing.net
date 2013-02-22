using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using dashing.net.Transforms;

namespace dashing.net.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var application = new ScriptBundle("~/bundles/application-js")
                            .Include("~/Scripts/jquery-1.9.1.js")
                            .Include("~/Scripts/es5-shim.js")
                            .Include("~/Scripts/gridster/jquery.leanModal.min.js")
                            .Include("~/Scripts/gridster/jquery.gridster.js")
                            .Include("~/Scripts/batman.js")
                            .Include("~/Scripts/batman.jquery.js")
                            .Include("~/Scripts/dashing.coffee")
                            .Include("~/Scripts/dashing.gridster.coffee")
                            .Include("~/Scripts/application.coffee");

            application.Transforms.Add(new CoffeeTransform());
            bundles.Add(application);

            var styles = new Bundle("~/bundles/application-css")
                .Include("~/Assets/stylesheets/font-awesome.css")
                .Include("~/Assets/stylesheets/jquery.gridster.css")
                .Include("~/Assets/stylesheets/application.scss");

            styles.Transforms.Add(new ScssTransform());
            bundles.Add(styles);

            BundleTable.EnableOptimizations = true;
        }
    }
}