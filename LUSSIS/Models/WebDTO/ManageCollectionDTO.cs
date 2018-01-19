﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.WebDTO
{
    public class ManageCollectionDTO
    {
        public CollectionPoint CollectionPoint { get; set; }

        public string CollectionName
        {
            get
            {
                return CollectionName + "  ";
            }
        }

        public IEnumerable<CollectionPoint> GetAll { get; set; }

    }
}