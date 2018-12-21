﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleSocia.Services.Models.Account;
using SimpleSocia.Services.Models.Posts;
using SimpleSocial.Data.Common;
using SimpleSocial.Data.Models;


namespace SimpleSocial.Services.DataServices.Account
{
    public class MyProfileServices : IMyProfileServices
    {
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<Wall> wallRepository;
        private readonly IRepository<ProfilePicture> profilePicturesRepository;
        private readonly UserManager<SimpleSocialUser> userManager;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IRepository<SimpleSocialUser> userRepository;

        public MyProfileServices(
            IRepository<Post> postRepository,
            IRepository<Wall> wallRepository,
            IRepository<ProfilePicture> profilePicturesRepository,
            UserManager<SimpleSocialUser> userManager,
            IHostingEnvironment hostingEnvironment,
            IRepository<SimpleSocialUser> userRepository
            )
        {
            this.postRepository = postRepository;
            this.wallRepository = wallRepository;
            this.profilePicturesRepository = profilePicturesRepository;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.userRepository = userRepository;
        }

        public string GetWallId(ClaimsPrincipal user)
        {
            var userId = userManager.GetUserId(user);
            var posts = wallRepository.All().FirstOrDefault(w => w.UserId == userId)?.Id;

            return posts;
        }

        public ProfilePicture GetProfilePicture(ClaimsPrincipal user)
        {
            var userId = userManager.GetUserId(user);
            var profilePicture = profilePicturesRepository.All().FirstOrDefault(x => x.UserId == userId);
            var wwwRootPath = hostingEnvironment.WebRootPath;
            if (File.Exists(wwwRootPath + $"/profile-pictures/{profilePicture?.FileName}"))
            {
                return profilePicture;
            }
            
            return new ProfilePicture
            {
                FileName = "default.jpg",
                UserId = userId,
            };
        }

        public void UploadProfilePicture(ClaimsPrincipal user, UploadProfilePictureInputModel inputModel, string imgExtension)
        {
            var currentProfilePictureFile = this.GetProfilePicture(user);
            var userId = this.userManager.GetUserId(user);


            if (currentProfilePictureFile != null && currentProfilePictureFile.FileName != "default.jpg")
            {
                System.IO.File.Delete(this.hostingEnvironment.WebRootPath + "/profile-pictures/" + currentProfilePictureFile.FileName);
            }

            var imageName = this.hostingEnvironment.WebRootPath + "/profile-pictures/" + userId + imgExtension;

            using (var fileStream = new FileStream(imageName, FileMode.Create))
            {
                inputModel.UploadImage.CopyToAsync(fileStream).GetAwaiter().GetResult();
                var currentProfilePicture = profilePicturesRepository.All().FirstOrDefault(x => x.UserId == userId);
                while (currentProfilePicture != null)
                {
                    profilePicturesRepository.Delete(currentProfilePicture);
                    profilePicturesRepository.SaveChangesAsync().GetAwaiter().GetResult();
                    currentProfilePicture = profilePicturesRepository.All().FirstOrDefault(x => x.UserId == userId);
                }

                var newProfilePic = new ProfilePicture
                {
                    FileName = userId + imgExtension,
                    UserId = userId
                };
                profilePicturesRepository.AddAsync(newProfilePic).GetAwaiter().GetResult();

                var userProfilePicToChange = userManager.GetUserAsync(user).GetAwaiter().GetResult();

                profilePicturesRepository.SaveChangesAsync().GetAwaiter().GetResult();

                userProfilePicToChange.ProfilePictureId = newProfilePic.Id;

                userRepository.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }
    }
}
