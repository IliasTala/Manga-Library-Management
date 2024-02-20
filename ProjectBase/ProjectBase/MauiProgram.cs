using Microsoft.Extensions.DependencyInjection;
using ProjectBase.View;
using ProjectBase.ViewModel;

namespace ProjectBase;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Manga-Font.ttf", "MangaFont");
				fonts.AddFont("Manga-Italic.ttf", "MangaItalic");
            });


		builder.Services.AddSingleton<MainViewModel>(); // unique et permanente
		builder.Services.AddSingleton<MainPage>();

        builder.Services.AddTransient<DetailsViewModel>(); // transitoire 
        builder.Services.AddTransient<DetailsPage>();
		
		builder.Services.AddTransient<MangaService>();
		builder.Services.AddTransient<CreateUserTables>();
        builder.Services.AddTransient<UserManagementServices>();

		builder.Services.AddTransient<FormViewModel>();
		builder.Services.AddTransient<FormPage>();

        builder.Services.AddTransient<UserViewModel>();
        builder.Services.AddTransient<UserPage>();


        return builder.Build();
	}
}
