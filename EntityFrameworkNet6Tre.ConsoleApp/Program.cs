// See https://aka.ms/new-console-template for more information
using EntityFrameworkNet6Tre.Data;
using EntityFrameworkNet6Tre.Domain;
using Microsoft.EntityFrameworkCore;

FootballLeageDbContext context = new FootballLeageDbContext();

/* Simple Insert Operation Methods */
////await AddNewLeague();
////await AddNewTeamsWithLeague();

/* Simple Select Queries */
////await SimpleSelectQuery();

/* Queries With Filters */
////await QueryFilters();

/* Aggregate Functions */
////await AdditionalExecutionMethods();

/*Alternative LINQ Syntax*/
////await AlternativeLinqSyntax();

/* Perform Update */
await SimpleUpdateLeagueRecord();
////await SimpleUpdateTeamRecord();

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
    ////Retrieve Record
    var league = await context.Leagues.FindAsync(2);
    Console.WriteLine($"{league.Id} - {league.Name}");
}

async Task SimpleUpdateLeagueRecord()
{
    ////Retrieve Record
    var league = await context.Leagues.FindAsync(2);
    ///Make Record Changes
    league.Name = "Scottish Premiership";
    ///Save Changes
    await context.SaveChangesAsync();

    await GetRecord();
}

async Task AlternativeLinqSyntax()
{
    Console.Write($"Enter Team Name (Or Part Of): ");
    var teamName = Console.ReadLine();
    var teams = await (from i in context.Teams
                       where EF.Functions.Like(i.Name, $"%{teamName}%")
                       select i).ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Id} - {team.Name}");
    }
}

async Task AdditionalExecutionMethods()
{
    //// These methods also have non-async
    var leagues = context.Leagues;
    var list = await leagues.ToListAsync();
    var first = await leagues.FirstAsync();
    var firstOrDefault = await leagues.FirstOrDefaultAsync();
    var single = await leagues.SingleAsync();
    var singleOrDefault = await leagues.SingleOrDefaultAsync();

    var count = await leagues.CountAsync();
    var longCount = await leagues.LongCountAsync();
    var min = await leagues.MinAsync();
    var max = await leagues.MaxAsync();

    //// DbSet Method that will execute
    var league = await leagues.FindAsync(1);
}

async Task QueryFilters()
{
    Console.Write($"Enter League Name (Or Part Of): ");
    var leagueName = Console.ReadLine();
    var exactMatches = await context.Leagues.Where(q => q.Name.Equals(leagueName)).ToListAsync();
    foreach (var league in exactMatches)
    {
        Console.WriteLine($"{league.Id} - {league.Name}");
    }

    ////var partialMatches = await context.Leagues.Where(q => q.Name.Contains(leagueName)).ToListAsync();
    ////var partialMatches = await context.Leagues.Where(q => EF.Functions.Like(q.Name, $"%{leagueName}%")).ToListAsync();
    ////foreach (var league in partialMatches)
    ////{
    ////    Console.WriteLine($"{league.Id} - {league.Name}");
    ////}
}

async Task SimpleSelectQuery()
{
    //// Smartest most efficient way to get results
    //var leagues = await context.Leagues.ToListAsync();
    //foreach (var league in leagues)
    //{
    //    Console.WriteLine($"{league.Id} - {league.Name}");
    //}

    //// Inefficient way to get results. Keeps connection open until completed and might create lock on table
    ////var s = await context.Leagues.ToListAsync();
    ////foreach (var league in context.Leagues.ToList())
    ////{
    ////    Console.WriteLine($"{league.Id} - {league.Name}");
    ////}

}

async Task AddNewLeague()
{
    //// Adding a new League Object
    var league = new League { Name = "Seria A" };
    await context.Leagues.AddAsync(league);
    //await context.SaveChangesAsync();

    //// Function To add new teams related to the new league object.
    await AddTeamsWithLeague(league);
    await context.SaveChangesAsync();
}

async Task AddTeamsWithLeague(League league)
{
    var teams = new List<Team>
    {
        new Team
        {
             Name = "Juventus",
             League = league
        },
        new Team
        {
            Name = "AC Milan",
            League = league
        },
        new Team
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