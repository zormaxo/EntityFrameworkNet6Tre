using EntityFrameworkNet6Tre.Data;
using EntityFrameworkNet6Tre.Domain;
using EntityFrameworkNet6Tre.Domain.Models;
using Microsoft.EntityFrameworkCore;

FootballLeageDbContext context = new();

/* Simple Insert Operation Methods */
//await AddNewLeague();
//await AddNewTeamsWithLeague();

/* Simple Select Queries */
//await SimpleSelectQuery();

/* Queries With Filters */
//await QueryFilters();

/* Aggregate Functions */
//await AdditionalExecutionMethods();

/*Alternative LINQ Syntax*/
//await AlternativeLinqSyntax();

/* Perform Update */
//await SimpleUpdateLeagueRecord();
//await SimpleUpdateTeamRecord();

/* Perform Delete */
//await SimpleDelete();
//await DeleteWithRelationship();

/*Tracking vs No-Tracking*/
//await TrackingVsNoTracking();

/*Adding Records with relationships*/
//Adding OneToMany Related Records
//await AddNewTeamsWithLeague();
//await AddNewTeamWithLeagueId();
//await AddNewLeagueWithTeams();

/*Adding ManyToMany Records*/
//await AddNewMatches();

/*Adding OneToOne Records*/
//await AddNewCoach();

/* Including Related Data - Eager Loading*/
//await QueryRelatedRecords();

/* Projections to Other Data Types or Ananymous Types */
//await SelectOneProperty();
//await AnonymousProjection();
//await StronglyTypedProjection();

/* Filter Based on Related Data */
//await FilteringWithRelatedData();

Console.WriteLine("Press Any Key To End....");
Console.ReadKey();

async Task FilteringWithRelatedData()
{
    var leagues = await context.Leagues.Where(q => q.Teams.Any(x => x.Name.Contains("Bay"))).ToListAsync();
}

async Task SelectOneProperty()
{
    var teams = await context.Teams.Select(q => q.Name).ToListAsync();
}

async Task AnonymousProjection()
{
    var teams = await context.Teams.Include(q => q.Coach).Select(
        q =>
        new
        {
            TeamName = q.Name,
            CoachName = q.Coach.Name
        }
        ).ToListAsync();

    foreach (var item in teams)
    {
        Console.WriteLine($"Team: {item.TeamName} | Coach: {item.CoachName}");
    }
}

async Task StronglyTypedProjection()
{
    var teams = await context.Teams.Include(q => q.Coach).Include(q => q.League).Select(
        q =>
        new TeamDetail
        {
            Name = q.Name,
            CoachName = q.Coach.Name,
            LeagueName = q.League.Name
        }
        ).ToListAsync();

    foreach (var item in teams)
    {
        Console.WriteLine($"Team: {item.Name} | Coach: {item.CoachName} | League: {item.LeagueName}");
    }
}

async Task QueryRelatedRecords()
{
    // Get Many Related Records - Leagues -> Teams
    var leagues = await context.Leagues.Include(q => q.Teams).ToListAsync();
    var teamsOmer = await context.Teams.Include(q => q.League).ToListAsync();     //inner join when int leagueId, left join when ?int

    // Get One Related Record - Team -> Coach
    var team = await context.Teams
        .Include(q => q.Coach)
        .FirstOrDefaultAsync(q => q.Id == 4);

    // Get 'Grand Children' Related Record - Team -> Matches -> Home/Away Team
    var teamsWithMatchesAndOpponents = await context.Teams
        .Include(q => q.AwayMatches).ThenInclude(q => q.HomeTeam).ThenInclude(q => q.Coach)
        .Include(q => q.HomeMatches).ThenInclude(q => q.AwayTeam).ThenInclude(q => q.Coach)
        .FirstOrDefaultAsync(q => q.Id == 4);

    // Get Includes with filters
    var teams = await context.Teams
        .Where(q => q.HomeMatches.Count > 0)
        .Include(q => q.Coach)
        .ToListAsync();
}

async Task TrackingVsNoTracking()
{
    var withTracking = await context.Teams.FirstOrDefaultAsync(q => q.Id == 2);
    var withNoTracking = await context.Teams.AsNoTracking().FirstOrDefaultAsync(q => q.Id == 8);

    if (withTracking != null) withTracking.Name = "Inter Milan";
    if (withNoTracking != null) withNoTracking.Name = "Rivoli United";

    var entriesBeforeSave = context.ChangeTracker.Entries();

    await context.SaveChangesAsync();

    var entriesAfterSave = context.ChangeTracker.Entries();
}

async Task SimpleDelete()
{
    var league = await context.Leagues.FindAsync(4);
    context.Leagues.Remove(league ?? throw new InvalidOperationException());
    await context.SaveChangesAsync();
}

async Task DeleteWithRelationship()
{
    var league = await context.Leagues.FindAsync(2);
    if (league != null) context.Leagues.Remove(league);
    await context.SaveChangesAsync();
}

async Task SimpleUpdateTeamRecord()
{
    var team = new Team
    {
        Id = 7,
        Name = "Seba United FC",
        LeagueId = 2
    };
    context.Teams.Update(team);
    await context.SaveChangesAsync();
}

async Task GetRecord()
{
    //Retrieve Record
    var league = await context.Leagues.FindAsync(2);
    if (league != null) Console.WriteLine($"{league.Id} - {league.Name}");
}

async Task SimpleUpdateLeagueRecord()
{
    //Retrieve Record
    var league = await context.Leagues.FindAsync(2);
    //Make Record Changes
    if (league != null) league.Name = "Scottish Premiership";
    //Save Changes
    await context.SaveChangesAsync();

    await GetRecord();
}

async Task AlternativeLinqSyntax()
{
    Console.Write("Enter Team Name (Or Part Of): ");
    var teamName = Console.ReadLine();
    var teams = await (from i in context.Teams
                       where EF.Functions.Like(i.Name, $"%{teamName}%")
                       select i).ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Id} - {team.Name}");
        Console.WriteLine();
    }
}

async Task AdditionalExecutionMethods()
{
    // These methods also have non-async
    var leagues = context.Leagues;
    _ = await leagues.ToListAsync();
    _ = await leagues.FirstAsync();
    _ = await leagues.FirstOrDefaultAsync();
    //var single = await leagues.SingleAsync();
    //var singleOrDefault = await leagues.SingleOrDefaultAsync();

    _ = await leagues.CountAsync();
    _ = await leagues.LongCountAsync();
    _ = await leagues.MinAsync(x => x.Id);
    _ = await leagues.MaxAsync(x => x.Id);

    // DbSet Method that will execute
    var league = await leagues.FindAsync(1);
}

async Task QueryFilters()
{
    Console.Write("Enter League Name (Or Part Of): ");
    var leagueName = Console.ReadLine();
    var exactMatches = await context.Leagues.Where(q => q.Name.Equals(leagueName)).ToListAsync();
    foreach (var league in exactMatches)
    {
        Console.WriteLine($"{league.Id} - {league.Name}");
    }

    //var partialMatches = await context.Leagues.Where(q => q.Name.Contains(leagueName)).ToListAsync();
    var partialMatches = await context.Leagues.Where(q => EF.Functions.Like(q.Name, $"%{leagueName}%")).ToListAsync();
    foreach (var league in partialMatches)
    {
        Console.WriteLine($"{league.Id} - {league.Name}");
    }
}

async Task SimpleSelectQuery()
{
    // Smartest most efficient way to get results
    var leagues = await context.Leagues.ToListAsync();
    foreach (var league in leagues)
    {
        Console.WriteLine($"{league.Id} - {league.Name}");
    }

    //Inefficient way to get results.Keeps connection open until completed and might create lock on table
    //foreach (var league in context.Leagues)
    //{
    //    Console.WriteLine($"{league.Id} - {league.Name}");
    //}
}

async Task AddNewLeague()
{
    // Adding a new League Object
    var league = new League { Name = "Seria A" };
    await context.Leagues.AddAsync(league);
    //await context.SaveChangesAsync();

    // Function To add new teams related to the new league object.
    await AddTeamsWithLeague(league);
    await context.SaveChangesAsync();
}

async Task AddTeamsWithLeague(League league)
{
    var teams = new List<Team>
    {
        new()
        {
            Name = "Juventus",
            League = league
        },
        new()
        {
            Name = "AC Milan",
            League = league
        },
        new()
        {
            Name = "AS Roma",
            League = league
        }
    };

    //// Operation to add multiple objects to database in one call.
    await context.AddRangeAsync(teams);
}

async Task AddNewTeamsWithLeague()
{
    var league = new League { Name = "Bundesliga" };
    var team = new Team { Name = "Bayern Munich", League = league };
    await context.AddAsync(team);
    await context.SaveChangesAsync();
}

async Task AddNewTeamWithLeagueId()
{
    var team = new Team { Name = "Fiorentina", LeagueId = 4 };
    await context.AddAsync(team);
    await context.SaveChangesAsync();
}

async Task AddNewLeagueWithTeams()
{
    var teams = new List<Team> {
                new()
                {
                    Name = "Rivoli United"
                },
                new()
                {
                    Name = "Waterhouse FC",
                },
            };
    var league = new League { Name = "CIFA", Teams = teams };
    await context.AddAsync(league);
    await context.SaveChangesAsync();
}

async Task AddNewMatches()
{
    var matches = new List<Match>
            {
                new()
                {
                    AwayTeamId = 4, HomeTeamId = 9, Date = new DateTime(2021, 10, 28)
                },
                new()
                {
                    AwayTeamId = 9, HomeTeamId = 4, Date = DateTime.Now
                },
                new()
                {
                    AwayTeamId = 9, HomeTeamId = 12, Date = DateTime.Now
                }
            };
    await context.AddRangeAsync(matches);
    await context.SaveChangesAsync();
}

async Task AddNewCoach()
{
    var coach1 = new Coach { Name = "Jose Mourinho", TeamId = 12 };

    await context.AddAsync(coach1);

    var coach2 = new Coach { Name = "Antonio Conte" };

    await context.AddAsync(coach2);
    await context.SaveChangesAsync();
}