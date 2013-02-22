using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace dashing.net.Transforms
{
    public class ScssTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var compiler = new SassAndCoffee.Ruby.Sass.SassCompiler();

            response.ContentType = "text/css";
            response.Content = string.Empty;

            foreach (var fileInfo in response.Files)
            {
                if (fileInfo.Extension.Equals(".sass", StringComparison.Ordinal) || fileInfo.Extension.Equals(".scss", StringComparison.Ordinal))
                {
                    response.Content += compiler.Compile(fileInfo.FullName, false, new List<string>());
                }
                else if (fileInfo.Extension.Equals(".css", StringComparison.Ordinal))
                {
                    response.Content += File.ReadAllText(fileInfo.FullName);
                }
            }

        }
    }
}