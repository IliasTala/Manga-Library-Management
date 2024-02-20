namespace ProjectBase.View;

public partial class FormPage : ContentPage
{
	public FormPage()
	{
		InitializeComponent();
		BindingContext = new FormViewModel();
	}
}