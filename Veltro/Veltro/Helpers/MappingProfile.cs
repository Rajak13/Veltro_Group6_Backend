using AutoMapper;
using Veltro.DTOs.Request.Part;
using Veltro.DTOs.Request.Vendor;
using Veltro.DTOs.Response.Notification;
using Veltro.DTOs.Response.Part;
using Veltro.DTOs.Response.Vendor;
using Veltro.Models;

namespace Veltro.Helpers;

/// <summary>AutoMapper profile defining all DTO ↔ Model mappings for the Veltro application.</summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Part
        CreateMap<Part, PartResponseDto>()
            .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Vendor != null ? s.Vendor.Name : string.Empty));
        CreateMap<CreatePartDto, Part>();

        // Vendor
        CreateMap<Vendor, VendorResponseDto>();
        CreateMap<CreateVendorDto, Vendor>();

        // Notification
        CreateMap<Notification, NotificationResponseDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));
    }
}
