global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Maui;
global using System.Data;

global using ProjectBase.ViewModel;
global using ProjectBase.View;
global using ProjectBase.Model;
global using ProjectBase.Services;

global using System.Text.Json;
global using System.Diagnostics;
global using System.Collections.ObjectModel;
global using System.Collections;

public class Globals
{
   public static DataSet userSet = new();
   public static List<MangaModel> MyMangaList = new List<MangaModel>();
}