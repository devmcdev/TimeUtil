using System.Text.Json.Serialization;

namespace TimeUtil.Shared;

public class Event
{
    [JsonInclude, JsonPropertyName(name: "Subject")]
    public string? EventSubject { get; init; }

    [JsonInclude, JsonPropertyName(name: "Start Date")]
    public DateOnly StartDate { get; init; }

    [JsonInclude, JsonPropertyName(name: "End Date")]
    public DateOnly EndDate { get; init; }

    [JsonInclude, JsonPropertyName(name: "Start Time")]
    public TimeOnly StartTime { get; init; }

    [JsonInclude, JsonPropertyName(name: "End Time")]
    public TimeOnly EndTime { get; init; }

    [JsonInclude, JsonPropertyName(name: "Categories")]
    public string[] Categories { get; init; } = Array.Empty<string>();

    [JsonInclude, JsonPropertyName(name: "All day event")]
    public bool AllDayEvent { get; init; }

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

    [JsonIgnore]
    private bool? _isUncategorised;
    [JsonIgnore]
    public bool IsUncategorised
    {
        get
        {
            if (!_isUncategorised.HasValue)
            {
                _isUncategorised = !Categories.Any();
            }
            return _isUncategorised.Value;
        }
    }
}
