﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleSocia.Services.Models.Followers;
using SimpleSocial.Data.Common;
using SimpleSocial.Data.Models;
using SimpleSocial.Services.Mapping;

namespace SimpleSocial.Web.Controllers
{
    public class FollowersController : Controller
    {
        private readonly IRepository<SimpleSocialUser> usersRepository;

        public FollowersController(IRepository<SimpleSocialUser> usersRepository)
        {
            this.usersRepository = usersRepository;
        }
        public IActionResult Index()
        {
            var viewModel = new AddFollowersViewModel();
            viewModel.UsersToFollow = usersRepository.All().Include(u => u.ProfilePicture).To<SimpeUserViewModel>().ToList();

            return View(viewModel);
        }
    }
}