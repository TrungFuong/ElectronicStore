using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tokens
    {
        [Key]
        public long Id { get; set; }
        public string AccountId { get; set; }
        public Account? Account { get; set; }
        public string HashToken { get; set; } = string.Empty;
    }
}
