using System.Runtime.CompilerServices;
using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]

// Vì đang dùng constructor internal MainWindowViewModel(string bestTimesFilePath) trong test,
// nên project test chưa access được constructor internal nếu không thêm vào AssemblyInfo hoặc .csproj của App:
[assembly: InternalsVisibleTo("MineSweeper.Tests")]
