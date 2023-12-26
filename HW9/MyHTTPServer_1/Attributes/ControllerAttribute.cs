using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer_1.Attributes
{
    public class ControllerAttribute: Attribute
    {
        public ControllerAttribute(string controllerName) => ControllerName = controllerName;

        public string ControllerName { get; }
    }
}
