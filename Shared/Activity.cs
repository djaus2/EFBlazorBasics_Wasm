using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EFBlazorBasics_Wasm.Shared
{
    public class Helper
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        [Required]
        public string Name { get; set; }
    }

    public class Round
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("No")]
        [Required]
        public int No { get; set; }

        [JsonIgnore]
        public IList<Activity> Activitys { get; } = new List<Activity>();
    }

    public class Activity
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Round")]
        [Required]
        public Round Round { get; set; }

        [Column("Helper")]
        public Helper Helper { get; set; }

        [Column("Task")]
        [Required]
        public string Task { get; set; }

    }
}


