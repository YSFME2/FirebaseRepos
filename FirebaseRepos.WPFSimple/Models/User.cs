using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseRepos.WPFSimple.Models
{
    public class User : IFireBaseClass
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
