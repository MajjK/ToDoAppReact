﻿using System;
using AutoMapper;
using ToDoApp.ViewModel.Tasks;
using ToDoApp.ViewModel.Users;
using ToDoApp.ViewModel.Auth;
using ToDoApp.DB.Model;

namespace ToDoApp.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DbTask, TaskViewModel>().ReverseMap();
            CreateMap<DbUser, UserViewModel>().ReverseMap();
            CreateMap<RegisterViewModel, DbUser>().ReverseMap();
        }
    }

}
