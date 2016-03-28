using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public string owner { get; set; }
        public string task { get; set; }
        public bool completed { get; set; }
        public DateTime date { get; set; }
        public int useless { get; set; }
    }
}
