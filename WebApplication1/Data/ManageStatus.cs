﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("ManageStatus")]
    public partial class ManageStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string Description { get; set; }

        public List<Order> Orders { get; set; }
        public List<Table> Tables { get; set; }
    }
}
