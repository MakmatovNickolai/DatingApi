namespace DataAccess.Entities.Base;

internal interface ITrackableEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
