using MauiCameraSettings.Models;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Selectors;

public class CollectionItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate DefaultTemplate { get; set; }
    public DataTemplate DescriptionOnlyTemplate { get; set; }
    

    public CollectionItemDataTemplateSelector()
    {
    }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        DataTemplate dataTemplate = DefaultTemplate;
        if (item != null && item is CollectionItem collectionItem)
        {
            if (collectionItem.TemplateName == CollectionItem.Templates.DESCR_ONLY_TEMPLATE)
            {
                dataTemplate = DescriptionOnlyTemplate;    
            }
        }
        return dataTemplate;
    }
}