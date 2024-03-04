using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;

namespace XperienceCommunity.CSP.UIFormComponents;

public class CspDirectivesDataProvider : IGeneralSelectorDataProvider
{
    private static readonly int _pageSize = 20;

    public Task<PagedSelectListItems<string>> GetItemsAsync(string searchTerm, int pageIndex, CancellationToken cancellationToken)
    {
        var options = GetAllOptions();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            options = options.Where(x => x.Text.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase));
        }

        options = options.Skip(pageIndex * _pageSize);

        return Task.FromResult(new PagedSelectListItems<string>()
        {
            NextPageAvailable = options.Skip(_pageSize).Any(),
            Items = options.Take(_pageSize)
        });
    }

    public Task<IEnumerable<ObjectSelectorListItem<string>>> GetSelectedItemsAsync(IEnumerable<string> selectedValues, CancellationToken cancellationToken)
    {
        var allOptions = GetAllOptions();

        var selectedOptions = selectedValues.ToList();

        var options = allOptions.Where(x => selectedOptions.Contains(x.Value));

        return Task.FromResult(options);
    }

    private static IEnumerable<ObjectSelectorListItem<string>> GetAllOptions()
    {
        var options = new List<string>
        {
            "default-src",
            "base-uri",
            "child-src",
            "connect-src",
            "font-src",
            "form-action",
            "frame-ancestors",
            "frame-src",
            "img-src",
            "manifest-src",
            "media-src",
            "object-src",
            "prefetch-src",
            "script-src",
            "script-src-elem",
            "style-src",
            "style-src-attr",
            "style-src-elem",
            "worker-src",
        };

        return options.Select(option => new ObjectSelectorListItem<string>
        {
            Value = option,
            Text = option,
            IsValid = true
        });
    }
}

