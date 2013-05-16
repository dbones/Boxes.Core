using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Box4_8i
{
    public class Box4
    {
        public Box4()
        {
            Box8.AwesomeClass.Message = "Belongs to Box4";
        }

        public string Message { get { return Box8.AwesomeClass.Message; } }
    }
}
