namespace TimeUtil.Shared
{
    public class OutlookCalendar : IDisposable
    {
        private readonly Event[] _events;
        private readonly Dictionary<string, HashSet<Event>> _eventLookup;
        private string[]? _categories;
        private DateTime? _firstEventDate;
        private DateTime? _lastEventDate;

        public OutlookCalendar(IEnumerable<Event> events)
        {
            _events = events.ToArray();

            foreach (var @event in _events)
            {
                @event.OnEventUpdated += HandleEventUpdate;
            }

            _eventLookup = PopulateEventLookup();
        }

        public event Action? OnUpdated;

        public IReadOnlyCollection<Event> Events => _events;
        public string[] Categories => _categories ??= _events.SelectMany(e => e.Categories).Distinct().ToArray();
        public DateTime FirstEventDate => _firstEventDate ??= _events.Min(x => x.FullStartDateTime);
        public DateTime LastEventDate => _lastEventDate ??= _events.Max(x => x.FullEndDateTime);

        private Dictionary<string, HashSet<Event>> PopulateEventLookup()
        {
            Dictionary<string, HashSet<Event>> eventLookup = new();

            foreach (var category in Categories)
            {
                eventLookup.Add(category, _events.Where(ev => ev.Categories.Contains(category)).ToHashSet());
            }

            return eventLookup;
        }

        public TimeSpan TotalEventTimeSpan()
        {
            return TotalEventTimeSpan(_events);
        }

        public TimeSpan TotalEventTimeSpan(IEnumerable<string> categories)
        {
            return TotalEventTimeSpan(FilterEvents(categories).ToArray());
        }

        public static TimeSpan TotalEventTimeSpan(IList<Event> events)
        {
            TimeSpan total = TimeSpan.Zero;

            for (int i = 0; i < events.Count; i++)
            {
                total += events[i].Eventduration;
            }

            return total;
        }

        public static double TimeUtilisationPercentage(double targetHours, IEnumerable<Event> events)
        {
            double totalHours = TotalEventTimeSpan(events.ToArray()).TotalHours;
            double timeUtilPercentage = totalHours / targetHours * 100;
            return timeUtilPercentage;
        }

        public IEnumerable<Event> FilterEvents(IEnumerable<string> categories)
        {
            return FilterEvents(categories, null, null);
        }

        public IEnumerable<Event> FilterEvents(IEnumerable<string>? categories = null, DateOnly? startDate = null, DateOnly? endDate = null, string? eventSubject = null)
        {
            IEnumerable<Event> local = categories is null ? _events : FilterEventsByCategories(categories);

            if (!string.IsNullOrWhiteSpace(eventSubject))
            {
                local = local.Where(e => e.EventSubject?.Contains(eventSubject, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (startDate is not null)
            {
                local = local.Where(e => e.StartDate >= startDate);
            }

            if (endDate is not null)
            {
                local = local.Where(e => e.EndDate <= endDate);
            }

            return local;
        }

        private IEnumerable<Event> FilterEventsByCategories(IEnumerable<string> categories)
        {
            List<Event> events = new();

            foreach (var category in categories)
            {
                if (_eventLookup.TryGetValue(category, out var value))
                {
                    events.AddRange(value);
                }
            }

            return events.Distinct();
        }

        private void HandleEventUpdate(EventUpdateArgs args)
        {
            foreach (string key in args.RemovedCategories)
            {
                _eventLookup[key].Remove(args.Sender);
            }

            foreach (string key in args.AddedCategories)
            {
                _eventLookup[key].Add(args.Sender);
            }

            OnUpdated?.Invoke();
        }

        public void Dispose()
        {
            foreach (var @event in _events)
            {
                @event.OnEventUpdated -= HandleEventUpdate;
            }

            GC.SuppressFinalize(this);
        }
    }
}
