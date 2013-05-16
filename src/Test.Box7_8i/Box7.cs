using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Box7_8i
{
    public class Box7
    {
        public Box7()
        {
            Box8.AwesomeClass.Message = "Belongs to Box7";
        }

        public string Message { get { return Box8.AwesomeClass.Message; } }
    }
}
