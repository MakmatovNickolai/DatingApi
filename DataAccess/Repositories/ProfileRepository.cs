using DataAccess.Dto;
using DataAccess.Entities;
using DataAccess.Entities.Base;
using DataAccess.Entities.Identity;
using DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static DataAccess.Repositories.ProfileRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.Repositories;

public interface IProfileRepository : IRepository<Profile, Guid>
{
    Task<List<ProfileD>> GetClosestAsync(Guid voterId, double longitude, double latitude, double radius, int limit);
}

public class ProfileRepository : Repository<Profile, Guid>, IProfileRepository
{
    private readonly ApplicationDbContext _context;
    public ProfileRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ProfileD>> GetClosestAsync(Guid voterId, double longitude, double latitude, double radius, int limit)
    {
        string longitudeStr = longitude.ToString(CultureInfo.InvariantCulture);
        string latitudeStr = latitude.ToString(CultureInfo.InvariantCulture);
        var profiles = await RawSqlExecutor.ExecuteAsync(_context, $@"SELECT x.* FROM(
            SELECT 
                p.""Id"" as ""ProfileId"",
		        p.""Name"",
		        p.""BirthDate"",
		        p.""Description"",
		        p.""RecentOn"",
		        g.""Name"" as ""Gender"",
		        i.""Id"" as ""ImageId"",
                u.""Id"" as ""UserId"",
                (6371 *
                    acos(cos(radians({latitudeStr})) *
                        cos(radians(""Latitude"")) *
                        cos(radians(""Longitude"") - radians({longitudeStr})) +
                        sin(radians({latitudeStr})) *
                        sin(radians(""Latitude""))
                )) AS ""Distance"" 
            FROM 
                ""Profile"" p
                INNER JOIN ""User"" u ON u.""Id"" = p.""UserId""
                RIGHT JOIN ""Image"" i on p.""Id"" = i.""ProfileId""
                INNER JOIN ""Gender"" g on g.""Id"" = p.""GenderId""
            WHERE
                u.""IsBlocked"" = false AND
                p.""Id"" != '{voterId}' AND 
                p.""Id"" NOT IN (
                    SELECT ""ReceiverId"" 
                    FROM ""Vote"" 
                    WHERE ""VoterId"" = '{voterId}'
                )
            ) x
            WHERE ""Distance"" < {radius}
            ORDER BY ""Distance""
            LIMIT {limit}", x => new ProfileD
        {
            Id  = (Guid)x[0],
            Name = (string)x[1],
            DateOfBirth = ((DateTime)x[2]).ToString("o"),
            Description = (string)x[3],
            RecentOn = ((DateTime)x[4]).ToString("o"),
            Gender = (string)x[5],
            ImageId = (Guid)x[6],
            UserId = (Guid)x[7],
            DistanceInMeters = Convert.ToInt32((double)x[8]),            
        });
        
        return profiles;
    }

    public class ProfileD
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Description { get; set; }
        public string RecentOn { get; set; }
        public string Gender { get; set; }
        public Guid ImageId { get; set; }
        public Guid? UserId { get; set; }
        public int DistanceInMeters { get; set; }       
    }
}