﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Authorize]
    public class UserInfoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}