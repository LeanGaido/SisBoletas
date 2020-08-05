using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("tblParametrosEmpresas")]
    public class ParametroEmpresa
    {
        public int ID { get; set; }

        public int IdEmpresa { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
