using System.ComponentModel.DataAnnotations;

namespace CheeseMVC.ViewModels
{
    public class AddCategoryViewModel
    {
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}
