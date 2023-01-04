namespace TimeUtil.Components.EventsTable;
internal class MultiEditResult
{
    public MultiEditResult(IEnumerable<string> categoiesToAdd, IEnumerable<string> categoiesToRemove)
    {
        CategoiesToAdd = categoiesToAdd;
        CategoiesToRemove = categoiesToRemove;
    }

    public IEnumerable<string> CategoiesToAdd { get; } = Enumerable.Empty<string>();
    public IEnumerable<string> CategoiesToRemove { get; } = Enumerable.Empty<string>();
}
