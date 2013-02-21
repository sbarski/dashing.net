using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace dashing.net.Transforms
{
    public class CoffeeTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            ///response.Content = SassAndCoffee.JavaScript.CoffeeScript.CoffeeScriptCompiler();
        }
    }
}