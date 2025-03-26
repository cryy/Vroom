using AutoMapper;
using Vroom.Dto;
using Vroom.Service.Database.Entities;

namespace Vroom.Mappers;

public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        CreateMap<VehicleMake, VehicleMakeDto>();
        // prevent manual id override
        CreateMap<CreateVehicleMakeDto, VehicleMake>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateVehicleMakeDto, VehicleMake>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<VehicleModel, VehicleModelDto>()
            .ForMember(dest => dest.MakeName, opt => opt.MapFrom(src => src.Make.Name));
        // prevent manual id override
        CreateMap<CreateVehicleModelDto, VehicleModel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateVehicleModelDto, VehicleModel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<VehicleModelDto, UpdateVehicleModelDto>();
    }
}
