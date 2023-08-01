﻿using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class ReportToProfile:Profile
    {
        public ReportToProfile()
        {
            CreateMap<GetEmployeesForReportsToResult, ReportToResult>();
        }
    }
}
