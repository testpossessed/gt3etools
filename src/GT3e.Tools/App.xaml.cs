using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using GT3e.Tools.Services;
using GT3e.Tools.ViewModels;
using NLog;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;

namespace GT3e.Tools;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs eventArgs)
    {
        base.OnStartup(eventArgs);

        this.InitialiseApp();

        this.MainWindow = new MainWindow
        {
            DataContext = new MainViewModel()
        };
        this.MainWindow.Show();

        LogWriter.Info("GT3e Tools started");
    }

    private void EnsureAppDataFolderExists()
    {
        if(!Directory.Exists(PathProvider.AppDataFolderPath))
        {
            Directory.CreateDirectory(PathProvider.AppDataFolderPath);
        }
    }

    private void EnsureConfigExists()
    {
        const string appSettingsContent = @"{
        ""Logging"" : {
                ""LogLevel"" : {
                        ""Default"" : ""Debug"",
                        ""System"" : ""Information"",
                        ""Microsoft"" : ""Information""
                    }
            },
        ""NLog"" : {
                ""internalLogLevel"" : ""Info"",
                ""internalLogFile"" : ""C:\\NLog\\internal-nlog.txt"",
                ""targets"" : {
                        ""file"" : {
                                ""type"" : ""File"",
                                ""fileName"" : ""${var:appDataFolder}\\Logs\\log-${shortdate}.log"",
                                ""layout"" :
                                    ""${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}""
                            },
                        ""console"" : {
                                ""type"" : ""Console""
                            }
                    },
                ""rules"" : [
                        {
                            ""logger"" : ""*"",
                            ""minLevel"" : ""Trace"",
                            ""writeTo"" : ""console, file""
                        }
                    ]
            },
        ""CarFolders"" : {
                ""porsche_991_gt3_r"" : ""Porsche 991 GT3 R"",
                ""porsche_991ii_gt3_r"" : ""Porsche 991 ii GT3 R"",
                ""porsche_991ii_gt3_cup"" : ""Porsche 991 ii GT3 Cup"",
                ""porsche_718_cayman_gt4_mr"" : ""Porsche 718 Cayman GT4 MR"",
                ""nissan_gt_r_gt3_2018"" : ""Nissan GT-R Nismo GT3 (2018)"",
                ""nissan_gt_r_gt3_2017"" : ""Nissan GT-R Nismo GT3 (2017)"",
                ""mercedes_amg_gt4"" : ""Mercedes-AMG GT4"",
                ""mercedes_amg_gt3_evo"" : ""Mercedes-AMG GT3 Evo"",
                ""mercedes_amg_gt3"" : ""Mercedes-AMG GT3"",
                ""mclaren_720s_gt3"" : ""McLaren 720s GT3"",
                ""mclaren_650s_gt3"" : ""McLaren 650s GT3"",
                ""mclaren_570s_gt4"" : ""McLaren 570s GT4"",
                ""mazerati_mc_gt4"" : ""Mazerati Grand Tourismo MC GT4"",
                ""lexus_rc_f_gt3"" : ""Lexus RC F GT3"",
                ""lamborghini_huracan_st"" : ""Lamborghini Huracan ST"",
                ""lamborghini_huracan_gt3_evo"" : ""Lamborghini Huracan GT3 Evo"",
                ""lamborghini_huracan_gt3"" : ""Lamborghini Huracan GT3"",
                ""lamborghini_gallardo_rex"" : ""Lamborghini Gallardo REX"",
                ""ktm_xbow_gt4"" : ""KTM X-Bow GT4"",
                ""jaguar_g3"" : ""Emily Frey Jaguar GT3"",
                ""honda_nsx_gt3_evo"" : ""Honda NSX GT3 Evo"",
                ""honda_nsx_gt3"" : ""Honda NSX GT3"",
                ""ginetta_g55_gt4"" : ""Ginetta G55 GT4"",
                ""ferrari_488_gt3_evo"" : ""Ferrari 488 GT3 Evo"",
                ""ferrari_488_gt3"" : ""Ferrari 488 GT3"",
                ""chevrolet_camaro_gt4r"" : ""Chevrolet Camaro GT4R"",
                ""bmw_m6_gt3"" : ""BMW M6 GT3"",
                ""bmw_m4_gt4"" : ""BMW M4 GT4"",
                ""bentley_continental_gt3_2018"" : ""Bentley Continental GT3 (2018)"",
                ""bentley_continental_gt3_2016"" : ""Bentley Continental GT3 (2016)"",
                ""audi_r8_lms_evo"" : ""Audi R8 LMS Evo"",
                ""audi_r8_lms"" : ""Audi R8 LMS"",
                ""audi_r8_gt4"" : ""Audi R8 GT4"",
                ""amr_v8_vantage_gt4"" : ""AMR V8 Vantage GT4"",
                ""amr_v8_vantage_gt3"" : ""AMR V8 Vantage GT3"",
                ""amr_v12_vantage_gt3"" : ""AMR V12 Vantage GT3"",
                ""alpine_a110_gt4"" : ""Alpine A110 GT4""
            },
        ""TrackFolders"" : {
                ""Zolder"" : ""Zolder"",
                ""Zandvoort"" : ""Zandvoort"",
                ""Suzuka"" : ""Suzuka"",
                ""Spa"" : ""Spa Francorchamps"",
                ""snetterton"" : ""Snetterton"",
                ""silverstone"" : ""Silverstone"",
                ""Paul_Ricard"" : ""Paul Ricard"",
                ""oulton_park"" : ""Oulton Park"",
                ""nurburgring"" : ""Nurburgring"",
                ""mount_panorama"" : ""Mount Panorama"",
                ""monza"" : ""Monza"",
                ""misano"" : ""Misano"",
                ""Laguna_Seca"" : ""Laguna Seca"",
                ""Kyalami"" : ""Kyalami"",
                ""Imola"" : ""Imola"",
                ""Hungaroring"" : ""Hungaroring"",
                ""donington"" : ""Donington"",
                ""brands_hatch"" : ""Brands Hatch"",
                ""Barcelona"" : ""Barcelona""
            }
    }";
        if(File.Exists(PathProvider.AppSettingsFilePath))
        {
            return;
        }

        File.WriteAllText(PathProvider.AppSettingsFilePath, appSettingsContent);
    }

    private void InitialiseApp()
    {
        SyncfusionLicenseProvider.RegisterLicense(
            "NTk0NDg0QDMxMzkyZTM0MmUzMFJGNngySWViaG8rWmlVdTQwVldJNHU4ZUJaM2V6YVhMaGd0UTJub0FGVzg9");
        SfSkinManager.ApplyStylesOnApplication = true;
        this.EnsureAppDataFolderExists();
        this.EnsureConfigExists();
        Configuration.Init();
        LogWriter.Init();
        this.SetupExceptionHandling();
    }

    private void LogUnhandledException(Exception exception, string source)
    {
        var message = $"Unhandled exception ({source})";
        try
        {
            var assemblyName = Assembly.GetExecutingAssembly()
                                       .GetName();
            message = $"Unhandled exception in {assemblyName.Name} v{assemblyName.Version}";
        }
        catch(Exception ex)
        {
            LogWriter.Error(ex, "Exception in LogUnhandledException");
        }
        finally
        {
            LogWriter.Error(exception, message);
        }
    }

    private void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException +=
            (s, e) => this.LogUnhandledException((Exception) e.ExceptionObject,
                "AppDomain.CurrentDomain.UnhandledException");

        this.DispatcherUnhandledException += (s, e) =>
                                             {
                                                 this.LogUnhandledException(e.Exception,
                                                     "Application.Current.DispatcherUnhandledException");
                                                 e.Handled = true;
                                             };

        TaskScheduler.UnobservedTaskException += (s, e) =>
                                                 {
                                                     this.LogUnhandledException(e.Exception,
                                                         "TaskScheduler.UnobservedTaskException");
                                                     e.SetObserved();
                                                 };
    }
}