using System.Text.Json.Serialization;

namespace TimeUtil.Shared;

public class Event
{
    public event Action<EventUpdateArgs>? OnEventUpdated;

    private static readonly string[] placeHolderCategory = { "Uncategorised" };

    [JsonInclude, JsonPropertyName(name: "Subject")]
    public string? EventSubject { get; private set; }

    [JsonInclude, JsonPropertyName(name: "Start Date")]
    public DateOnly StartDate { get; private set; }

    [JsonInclude, JsonPropertyName(name: "End Date")]
    public DateOnly EndDate { get; private set; }

    [JsonInclude, JsonPropertyName(name: "Start Time")]
    public TimeOnly StartTime { get; private set; }

    [JsonInclude, JsonPropertyName(name: "End Time")]
    public TimeOnly EndTime { get; private set; }

    [JsonInclude, JsonPropertyName(name: "Categories")]
    public IEnumerable<string> Categories { get; private set; } = Enumerable.Empty<string>();

    [JsonInclude, JsonPropertyName(name: "All day event")]
    public bool AllDayEvent { get; private set; }

    private TimeSpan? _Eventduration;

    [JsonIgnore]
    public TimeSpan Eventduration
    {
        get
        {
            if (!_Eventduration.HasValue)
            {
                TimeSpan dif = FullEndDateTime - FullStartDateTime;

                _Eventduration = dif;
            }
            return _Eventduration.Value;
        }
    }
    private DateTime? _fullStartDateTime;
    private DateTime? _fullEndDateTime;
    [JsonIgnore]

    public DateTime FullStartDateTime => _fullStartDateTime ??= StartDate.ToDateTime(StartTime);
    [JsonIgnore]
    public DateTime FullEndDateTime => _fullEndDateTime ??= EndDate.ToDateTime(EndTime);

    public void UpdateEvent(string[] categories)
    {
        if (categories.Length < 1)
        {
            categories = placeHolderCategory;
        }

        var addedCategories = categories.Except(Categories).ToArray();
        var removedCategories = Categories.Except(categories).ToArray();

        Categories = categories;

        EventUpdateArgs args = new(this, addedCategories, removedCategories);
        OnEventUpdated?.Invoke(args);
    }
}

public class EventUpdateArgs
{
    public EventUpdateArgs(Event sender, string[] addedCategories, string[] removedCategories)
    {
        Sender = sender;
        AddedCategories = addedCategories;
        RemovedCategories = removedCategories;
    }

    public Event Sender { get; }
    public string[] AddedCategories { get; }
    public string[] RemovedCategories { get; }
}
