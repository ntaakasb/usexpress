﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsExpress.Transport.Web.FrontEnd.Models
{
    public class CustomJsonResult
    {
        public string Message { get; set; }

        public object Result { get; set; }

        public object Optional { get; set; }
    }
}