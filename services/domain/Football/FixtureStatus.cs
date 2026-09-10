namespace Mis.Domain.Football;

public enum FixtureStatus
{
    Scheduled = 1,
    PreMatch = 2,
    Live = 3,
    HalfTime = 4,
    Finished = 5,
    Postponed = 6,
    Cancelled = 7,
    Unknown = 99,
}
