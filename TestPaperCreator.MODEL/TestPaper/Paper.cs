using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.MODEL.TestPaper
{
    public class Paper
    {
        //题型
        public PaperType papertype { get; set; }
        //数量
        public int count { get; set; }
    }
    public class PaperType
    {
        //题型
        public int questiontype { get; set; }
        //章节
        public int section { get; set; }
        //课程
        public int course { get; set; }
        //难易度
        public int difficulty { get; set; }
    }
}
