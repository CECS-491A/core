﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Point
    {
        public Point()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Index("PointLocation", 1)]
        public float Longitude { get; set; }
        [Required]
        [Index("PointLocation", 2)]
        public float Latitude { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
