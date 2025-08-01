﻿using AutoMapper;
using LocalShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LocalShop.Services.Mapping
{
    public class AutoMapperProfile: Profile 
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, DTOs.ProductDto>()
                .ReverseMap();
        }
    }
}
