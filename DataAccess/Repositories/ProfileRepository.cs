using DataAccess.Entities;
using DataAccess.Helpers;
using DataAccess.Models;
using DataAccess.Models.Dto;
using System.Globalization;

namespace DataAccess.Repositories;

public interface IProfileRepository : IRepository<Profile, Guid>
{
    Task<List<ClosestProfileDto>> GetClosestAsync(SearchOptions searchOptions);
}

public class ProfileRepository : Repository<Profile, Guid>, IProfileRepository
{
    private readonly ApplicationDbContext _context;
    public ProfileRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ClosestProfileDto>> GetClosestAsync(SearchOptions searchOptions)
    {
        string longitudeStr = searchOptions.Longitude.ToString(CultureInfo.InvariantCulture);
        string latitudeStr = searchOptions.Latitude.ToString(CultureInfo.InvariantCulture);
        const double maximumEarthDistanceKm = 20004;
        var distance = searchOptions.Distance == 510 ? maximumEarthDistanceKm : searchOptions.Distance;
        var profiles = await RawSqlExecutor.ExecuteAsync(_context, $@"SELECT x.* FROM(
            SELECT 
                p.""Id"" as ""ProfileId"",
		        p.""Name"",
		        p.""BirthDate"",
		        p.""Description"",
		        p.""RecentOn"",
		        g.""Name"" as ""Gender"",
		        i.""Id"" as ""ImageId"",
                i.""OrderIndex"" as ""OrderIndex"",
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
                p.""Id"" != '{searchOptions.ProfileId}' AND 
                p.""Id"" NOT IN (
                    SELECT ""ReceiverId"" 
                    FROM ""Vote"" 
                    WHERE ""VoterId"" = '{searchOptions.ProfileId}'
                ) AND
                p.""BirthDate"" BETWEEN 
                    CURRENT_DATE - INTERVAL '1 year' * {searchOptions.AgeMax} AND 
                    CURRENT_DATE - INTERVAL '1 year' * {searchOptions.AgeMin}
            ) x
            WHERE ""Distance"" < {distance}
            ORDER BY ""Distance""
            LIMIT {searchOptions.Limit}", x => new ClosestProfileDto  // todo filter by gender and age, todo birthdate inclusive range
        {
            Id  = (Guid)x[0],
            Name = (string)x[1],
            DateOfBirth = DateOnly.FromDateTime((DateTime)x[2]).ToString("o"),
            Description = (string)x[3],
            RecentOn = (DateTime)x[4] == DateTime.MinValue ? null : ((DateTime)x[4]).ToString("o"),
            Gender = (string)x[5],
            ImageId = (Guid)x[6],
            OrderIndex = (int)x[7],
            UserId = (Guid)x[8],
            DistanceKm = Convert.ToInt32((double)x[9]),            
        });
        
        return profiles;
    }
}