namespace TimeUtil.Shared;
public class EventUpdate
{
    public EventUpdate(Event @event, string[] categories)
    {
        Event = @event;
        NewCategories = categories;
    }

    public Event Event { get; }
    public string[] NewCategories { get; }
}
