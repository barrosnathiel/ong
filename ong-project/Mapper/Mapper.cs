using ong_project.Common;
using ong_project.Domain;
using ong_project.DTO;

namespace ong_project.Mapper;

public static class Mapper
{
    public static List<UserResponseListDTO> MapToDTO(List<User> users)
    {
        return users.Select(x => new UserResponseListDTO
        {
            Name = x.Name,
            Address = x.Address,
            Email = x.Email,
            ProfileImage = x.ProfileImage,
            Identity = x.Cpf,
            PhoneNumber = x.PhoneNumber,
            Type = GetUserType(x.UserType),
            Token = x.Token
        }).ToList();
    }

    public static List<CourseResponseListDTO> MapToDto(List<Course> courses)
    {
        return courses.Select(x => new CourseResponseListDTO
        {
            Name = x.Name,
            Begin = x.Begin.ToString("dd/MM/yyyy"),
            End = x.End.ToString("dd/MM/yyyy"),
            CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy"),
            Token = x.Token,
            TotalHours = x.TotalHours,
            Title = x.Title,
            Description = x.Description
        }).ToList();
    }

    private static string GetUserType(string type)
    {
        return type switch
        {
            Constants.USER_ROLE => "Student",
            Constants.USER_PROFESSOR => "Professor",
            Constants.ADMIN_ROLE => "Admin",
            _ => ""
        };
    }
}