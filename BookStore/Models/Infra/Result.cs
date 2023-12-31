﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models.Infra
{
	public class Result
	{
        public bool IsSuccess { get; private set;}
        public string ErrorMessage { get; private set; }

        public static Result Success() =>new Result { IsSuccess = true ,ErrorMessage=null};
        public static Result Fail(string errorMessage)
            => new Result { IsSuccess = false, ErrorMessage = errorMessage };
    }
}