using Microsoft.Maui.Graphics;

namespace MauiCameraSettings.Models;

public class CollectionItem : BaseModel
{
    int index = 0;
    public int Index
    {
        get { return index; }
        set { SetProperty(ref index, value); }
    }

    string code = string.Empty;
    public string Code
    {
        get { return code; }
        set {
            if (SetProperty(ref code, value))
            {
                OnPropertyChanged(nameof(FullDescription));
            }                
        }
    }

    string description = string.Empty;
    public string Description
    {
        get { return description; }
        set
        {
            if (SetProperty(ref description, value))
            {
                OnPropertyChanged(nameof(FullDescription));
            }
        }
    }

    string shortDescription = string.Empty;
    public string ShortDescription
    {
        get { return shortDescription; }
        set
        {
            SetProperty(ref shortDescription, value);                
        }
    }
    
    string notes = string.Empty;
    public string Notes
    {
        get { return notes; }
        set
        {
            SetProperty(ref notes, value);
        }
    }

    string message = string.Empty;
    public string Message
    {
        get { return message; }
        set
        {
            SetProperty(ref message, value);
        }
    }

    string iconGlyph = string.Empty;
    public string IconGlyph
    {
        get { return iconGlyph; }
        set
        {
            SetProperty(ref iconGlyph, value);
        }
    }
    
    Color color = Colors.Transparent;
    public Color Color
    {            
        get { return color; }
        set
        {
            SetProperty(ref color, value);
        }
    }

    bool hideCode = false;
    public bool HideCode
    {
        get { return hideCode; }
        set
        {
            SetProperty(ref hideCode, value);
        }
    }


    public string TemplateName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public object Source { get; set; }
    
    bool isSelected = false;
    public bool IsSelected
    {
        get { return isSelected; }
        set { SetProperty(ref isSelected, value); }
    }
   
    public string FullDescription
    {
        get
        {
            if (!string.IsNullOrEmpty(Code) || !string.IsNullOrEmpty(Description))
            {
                return Code + " - " + Description;
            }
            else
            {
                return string.Empty;
            }

        }
    }

    int count = 0;
    public int Count
    {
        get { return count; }
        set { SetProperty(ref count, value); }
    }
    
    public class Templates
    {
        public const string DESCR_ONLY_TEMPLATE = "DecriptionOnlyTemplate";
    }

    public CollectionItem()
    {

    }

    public void UpdateValues(CollectionItem item)
    {
        this.Index = item.Index;
        this.Code = item.Code;
        this.Description = item.Description;
        this.ShortDescription = item.ShortDescription;
        this.IconGlyph = this.IconGlyph;
        this.Color = this.Color;
        this.HideCode = this.HideCode;
        this.TemplateName = this.TemplateName;
        this.TypeName = this.TypeName;
        this.Source = this.Source;
        this.IsSelected = this.IsSelected;
        this.Count = item.Count;
        
    }
}