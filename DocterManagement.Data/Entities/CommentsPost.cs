﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Entities
{
    public class CommentsPost
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool CheckLevel { get; set; }
        public bool CheckComentId { get; set; }
        public Guid UserId { get; set; }
        public AppUsers AppUsers { get; set; }
        public Guid PostId { get; set; }
        public Posts Posts { get; set; }
    }
}
