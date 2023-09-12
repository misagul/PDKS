using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDKS.Models
{
    public class Log
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime DateTime { get; set; }
        public int Shift { get; set; }
        public bool OnTime { get; set; }

        public Log()
        {
            DateTime = DateTime.Now;
        }
    }
}
