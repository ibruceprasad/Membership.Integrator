using Mapster;
using Registration.Contracts;

namespace Integrator.Utils;

public class RegistrationMappingRegister : IRegister
{
    private const string DefaultSource = "MILKYWAY";

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegistrationRequest, Member>()
            .Map(d => d.GivenName, s => s.FirstName)
            .Map(d => d.FamilyName, s => s.LastName)
            .Map(d => d.Contact, s => new Contact { Email = s.Email })
            .Map(d => d.PlanCode, s => ToPlanCode(s.MembershipType))
            .Map(d => d.Source, s => DefaultSource);

        config.NewConfig<RegistrationRequest, MembershipRequest>()
            .Map(d => d.Member, s => s);
    }

    public static char ToPlanCode(string type)
    {
        Enum.TryParse<MembershipType>(type, out var membershipType);
        return membershipType switch
        {
            MembershipType.Single => 'S',
            MembershipType.Family => 'F',
            MembershipType.Couple => 'C',
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown membership type.")
        };
    }
}




// using AutoMapper;
// using Integrator.Contracts;
//  
// namespace Integrator.Requests;
//
// public class RegistrationProfile : Profile
// {
//     private const string DefaultSource = "MILKYWAY";
//
//     public RegistrationProfile()
//     {
//         // CreateMap<RegistrationRequest, Member>()
//         //     .ForMember(d => d.GivenName, o => o.MapFrom(s => s.FirstName))
//         //     .ForMember(d => d.FamilyName, o => o.MapFrom(s => s.LastName))
//         //     .ForMember(d => d.Contact, o => o.MapFrom(s => new Contact { Email = s.Email }))
//         //     .ForMember(d => d.PlanCode, o => o.MapFrom(s => ToPlanCode(s.MembershipType)))
//         //     .ForMember(d => d.Source, o => o.MapFrom(_ => DefaultSource));
//
//         CreateMap<RegistrationRequest, MembershipRequest>()
//             .ForMember(d => d.Member, o => o.MapFrom(s => s));
//     }
//
//     private static char ToPlanCode(MembershipType type) => type switch
//     {
//         MembershipType.Single => 'S',
//         MembershipType.Family => 'F',
//         MembershipType.Couple => 'C',
//         _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown membership type.")
//     };
// }