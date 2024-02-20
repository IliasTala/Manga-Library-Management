namespace ProjectBase;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
		Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
		Routing.RegisterRoute(nameof(FormPage), typeof(FormPage));
    }
}
