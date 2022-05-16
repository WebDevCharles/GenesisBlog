using System.ComponentModel.DataAnnotations;

namespace GenesisBlog.Enums
{
    public enum BlogPostState
    {
        [Display(Name = "Ready To Post")]
        ProductionReady,
        [Display(Name = "In Development")]
        InDevelopment,
        [Display(Name = "In Preview")]
        InPreview

    }
}
