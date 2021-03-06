using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.DB.Model
{
    [Table("Tasks")]
    public class DbTask
    {
        [Key]
        [Column("task_id")]
        public int TaskId { get; set; }

        [ForeignKey(nameof(User))]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("objective")]
        [Required, StringLength(255)]
        public string Objective { get; set; }

        [Column("description")]
        [StringLength(50)]
        public string Description { get; set; }

        [Column("addition_date")]
        [Required]
        public DateTime? AdditionDate { get; set; }

        [Column("closing_date")]
        public DateTime? ClosingDate { get; set; }

        [Column("finished")]
        [Required]
        public bool Finished { get; set; }

        public virtual DbUser User { get; set; }
    }
}
