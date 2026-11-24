using SevenSpikes.Nop.Plugins.AjaxFilters.Domain.Enums;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public class FilterItemStateManager
{
    public static FilterItemState GetNewStateBaseOnOptionAvailability(FilterItemState currentState, bool optionAvailable)
    {
        return GetNewStateBaseOnOptionAvailabilityInternal(currentState, optionAvailable);
    }

    private static FilterItemState GetNewStateBaseOnOptionAvailabilityInternal(FilterItemState currentState, bool optionAvailable)
    {
        FilterItemState availabilityInternal = currentState;
        switch (currentState)
        {
        case FilterItemState.Unchecked:
            if (!optionAvailable)
            {
                availabilityInternal = FilterItemState.Disabled;
            }
            break;
        case FilterItemState.Checked:
            if (!optionAvailable)
            {
                availabilityInternal = FilterItemState.CheckedDisabled;
            }
            break;
        case FilterItemState.CheckedDisabled:
            if (optionAvailable)
            {
                availabilityInternal = FilterItemState.Checked;
            }
            break;
        case FilterItemState.Disabled:
            if (optionAvailable)
            {
                availabilityInternal = FilterItemState.Unchecked;
            }
            break;
        }
        return availabilityInternal;
    }
}
